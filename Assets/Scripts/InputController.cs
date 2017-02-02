using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using MidiJack;

public class InputController : MonoBehaviour
{

	private IEnumerable<KeyCode> _keys;
	private readonly Dictionary<int, MelodyNote> _activeNotes = new Dictionary<int, MelodyNote>();

	public Melody _currentMelody;
	private float _lastInput;

	public float SilenceBetweenMelodies = 8; // Seconds
	public float MaxTimeBetweenNotesForChord = 0.05f;
	

	// Use this for initialization
	void Start()
	{

		_keys = Enumerable.Range((int)KeyCode.A, 26).Select(x => (KeyCode)x);

		MidiMaster.noteOnDelegate += (channel, midiNote, velocity) =>
		{
			var note = MelodyNote.FromMidiNote(midiNote);
			note.Velocity = velocity;
			NoteDown(note);
		};

		MidiMaster.noteOffDelegate += (channel, note) =>
		{
			var midiNote = MelodyNote.FromMidiNote(note);
			NoteUp(midiNote);
		};

	}


	// Update is called once per frame
	void Update()
	{
		foreach (var keyCode in _keys)
		{
			// Keyboard Down
			if (Input.GetKeyDown(keyCode))
			{
				NoteDown(MelodyNote.FromKeyCode(keyCode));
			}

			// Keyboard Up
			if (Input.GetKeyUp(keyCode))
			{
				NoteUp(MelodyNote.FromKeyCode(keyCode));
			}
		}
	}

	private void NoteUp(MelodyNote note)
	{
		if (!_activeNotes.ContainsKey(note.GetHashCode()))
		{
			return;
		}

		note.Duration = Time.time - note.Start;
		_activeNotes.Remove(note.GetHashCode());
	}

	private void NoteDown(MelodyNote note)
	{
		// Start a new Melody, because pause was long enough
		if (Time.time - _lastInput > SilenceBetweenMelodies)
		{
			StartNewMelody();
		}

		_lastInput = Time.time;
		if (!_activeNotes.ContainsKey(note.GetHashCode()))
		{
			note.Start = Time.time - _currentMelody.Start;
			_activeNotes.Add(note.GetHashCode(), note);
			_currentMelody.Notes.Add(note);
		}
	}

	private void StartNewMelody()
	{
		_currentMelody = new Melody
		{
			Start = Time.time
		};
		ParticleController.NewMelody(_currentMelody);
	}
}
