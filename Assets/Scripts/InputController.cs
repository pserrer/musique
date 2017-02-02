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
	private readonly List<Melody> _melodies = new List<Melody>();
	private float _lastInput;
	private MelodyNote _lastNote;

	public float SilenceBetweenMelodies = 8; // Seconds
	public float MaxTimeBetweenNotesForChord = 0.05f;
	

	// Use this for initialization
	void Start()
	{

		_keys = Enumerable.Range((int)KeyCode.A, 26).Select(x => (KeyCode)x);
		_currentMelody = new Melody {Start = Time.time};

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

		var chord = _lastNote as MelodyChord;
		if (chord != null)
		{
			// set end time of chord
			chord.Duration = Time.time - chord.Start;
		}
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

			// Group Notes into chord
			if (_lastNote != null)
			{
				var chord = _lastNote as MelodyChord;
				if (Time.time - _lastNote.Start < MaxTimeBetweenNotesForChord)
				{
					if (chord == null)
					{
						chord = new MelodyChord()
						{
							Start = _lastNote.Start
						};
						// replace last entry with chord
						_currentMelody.Notes.Remove(_lastNote);
						_currentMelody.Notes.Add(chord);

						// add first note to chord
						chord.AddNote(_lastNote);
					}
					chord.AddNote(note);
					_lastNote = chord;
				}
				else
				{
					_currentMelody.Notes.Add(note);
					_lastNote = note;
				}
			}
			else
			{
				_currentMelody.Notes.Add(note);
				_lastNote = note;
			}
		}
	}

	private void StartNewMelody()
	{
		_currentMelody = new Melody
		{
			Start = Time.time
		};
		_melodies.Add(_currentMelody);
	}
}
