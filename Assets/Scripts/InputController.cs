using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using MidiJack;

public class InputController : MonoBehaviour
{

	private IEnumerable<KeyCode> _keys;
	private readonly Dictionary<int, MelodyNote> _activeNotes = new Dictionary<int, MelodyNote>();

	public Melody CurrentMelody;

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
			var note = MelodyNote.FromKeyCode(keyCode);
			if (note == null)
			{
				continue;
			}
			// Keyboard Down
			if (Input.GetKeyDown(keyCode))
			{
				NoteDown(note);
			}

			// Keyboard Up
			if (Input.GetKeyUp(keyCode))
			{
				MelodyNote activeNote;
                _activeNotes.TryGetValue(note.GetHashCode(), out activeNote);
                if (activeNote != null)
				{
					NoteUp(activeNote);
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

		note.Duration = Time.time - note.Start - CurrentMelody.Start;
        _activeNotes.Remove(note.GetHashCode());
        Invoke("StartNewMelody", SilenceBetweenMelodies);
    }

	private void NoteDown(MelodyNote note)
	{
        CancelInvoke("StartNewMelody");

		if (!_activeNotes.ContainsKey(note.GetHashCode()))
		{
            if (!CurrentMelody.IsEmpty)
                note.Start = Time.time - CurrentMelody.Start;
            else
            {
                note.Start = 0;
                CurrentMelody.Start = Time.time;
            }
            _activeNotes.Add(note.GetHashCode(), note);
			CurrentMelody.Notes.Add(note);
		}
	}

	private void StartNewMelody()
	{
        if (!CurrentMelody.IsEmpty)
        {
            CurrentMelody.IsFinished = true;
            ParticleController.AddMelody(CurrentMelody);
        }
        CurrentMelody = new Melody
		{
			Start = Time.time
		};
	}
}
