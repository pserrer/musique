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

	public ParticleController ParticleController;

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

		StartNewMelody();
	}


	// Update is called once per frame
	void Update()
	{
		foreach (var keyCode in _keys)
		{
			// Keyboard Down
			if (Input.GetKeyDown(keyCode))
			{
				var note = MelodyNote.FromKeyCode(keyCode);
				if (note != null)
				{
					NoteDown(note);
				}
			}

			// Keyboard Up
			if (Input.GetKeyUp(keyCode))
			{
                MelodyNote note;
                _activeNotes.TryGetValue(MelodyNote.FromKeyCode(keyCode).GetHashCode(), out note);
                if (note != null)
				{
					NoteUp(note);
				}
			}
		}
	}

	private void NoteUp(MelodyNote note)
	{
		if (!_activeNotes.ContainsKey(note.GetHashCode()))
		{
			return;
		}

		note.Duration = Time.time - note.Start - _currentMelody.Start;
        _activeNotes.Remove(note.GetHashCode());
        Invoke("StartNewMelody", SilenceBetweenMelodies);
    }

	private void NoteDown(MelodyNote note)
	{
        CancelInvoke("StartNewMelody");

        _lastInput = Time.time;
		if (!_activeNotes.ContainsKey(note.GetHashCode()))
		{
            if (!_currentMelody.IsEmpty)
                note.Start = Time.time - _currentMelody.Start;
            else
            {
                note.Start = 0;
                _currentMelody.Start = Time.time;
            }
            _activeNotes.Add(note.GetHashCode(), note);
			_currentMelody.Notes.Add(note);
		}
	}

	private void StartNewMelody()
	{
        if (!_currentMelody.IsEmpty)
        {
            _currentMelody.IsFinished = true;
            ParticleController.AddMelody(_currentMelody);
        }
        _currentMelody = new Melody
		{
			Start = Time.time
		};
	}
}
