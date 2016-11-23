using System;
using System.Collections;
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

	public ParticleSystem DefaultSystem;

	public Transform Parent;
	private MidiDriver _midiDriver;

	// Use this for initialization
	void Start()
	{
		_keys = Enumerable.Range((int)KeyCode.A, 26).Select(x => (KeyCode)x);
		_currentMelody = new Melody {Start = Time.time};
		_midiDriver = MidiDriver.Instance;

		MidiMaster.noteOnDelegate += (channel, midiNote, velocity) =>
		{
			var octave = 0;
			var note = MidiNoteToNote(midiNote, out octave);
			NoteDown(note, octave);
		};

		MidiMaster.noteOnDelegate += (channel, midiNote, velocity) =>
		{
			var octave = 0;
			var note = MidiNoteToNote(midiNote, out octave);
			NoteUp(note, octave);
		};

	}

	protected Note MidiNoteToNote(int note, out int octave)
	{
		octave = note/12;
		var noteInOctave = note - octave*12;
		switch (noteInOctave)
		{
			case 0:
				return new Note("C");
			case 1:
				return new Note("C#");
			case 2:
				return new Note("D");
			case 3:
				return new Note("D#");
			case 4:
				return new Note("E");
			case 5:
				return new Note("F");
			case 6:
				return new Note("F#");
			case 7:
				return new Note("G");
			case 8:
				return new Note("G#");
			case 9:
				return new Note("A");
			case 10:
				return new Note("A#");
			case 11:
				return new Note("B");
		}
		throw new Exception("Invalid Note");
	}

	// Update is called once per frame
	void Update()
	{
		foreach (var keyCode in _keys)
		{
			// Keyboard Down
			if (Input.GetKeyDown(keyCode))
			{
				NoteDown(FromKeyCode(keyCode));
			}

			// Keyboard Up
			if (Input.GetKeyUp(keyCode))
			{
				NoteUp(FromKeyCode(keyCode));
			}
		}
	}

	private void NoteUp(Note midiNote, int octave = 4)
	{
		var key = midiNote.PositionInOctave + octave*12;
		if (!_activeNotes.ContainsKey(key))
		{
			return;
		}

		var note = _activeNotes[key];
		note.Duration = Time.time - note.Start;
		StartCoroutine(RemoveParticleSystem(note.ParticleSystem, 5f));
		_activeNotes.Remove(key);

		var chord = _lastNote as MelodyChord;
		if (chord != null)
		{
			// set end time of chord
			chord.Duration = Time.time - chord.Start;
		}
	}

	private void NoteDown(Note midiNote, int octave = 4)
	{
		// Start a new Melody, because pause was long enough
		if (Time.time - _lastInput > SilenceBetweenMelodies)
		{
			StartNewMelody();
		}

		_lastInput = Time.time;
		var key = midiNote.PositionInOctave + octave*12;
		if (!_activeNotes.ContainsKey(key))
		{
			var note = new MelodyNote(midiNote.Letter, midiNote.Accidental)
			{
				Start = _currentMelody.Start,
				Octave = octave
			};

			var system = (ParticleSystem) Instantiate(DefaultSystem, Vector3.zero, Quaternion.identity);
			ModifyParticleSystem(system);
			system.Play();

			note.ParticleSystem = system;

			system.transform.position = PositionForNote(note);

			_activeNotes.Add(key, note);
			system.transform.parent = Parent;

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
							Start = _lastNote.Start,
							ParticleSystem = null // Maybe add an extra effect for chord?
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


	private static Note FromKeyCode(KeyCode key)
	{
		switch (key)
		{
			case KeyCode.A:
				return new Note('C');
			case KeyCode.W:
				return new Note('C', 1);
			case KeyCode.S:
				return new Note('D');
			case KeyCode.E:
				return new Note('D', 1);
			case KeyCode.D:
				return new Note('E');
			case KeyCode.F:
				return new Note('F');
			case KeyCode.T:
			case KeyCode.R:
				return new Note('F', 1);
			case KeyCode.G:
				return new Note('G');
			case KeyCode.Z:
				return new Note('G', 1);
			case KeyCode.H:
				return new Note('A');
			case KeyCode.U:
				return new Note('A', 1);
			case KeyCode.J:
				return new Note('B');
		}

		// Invalid Input
		throw new Exception();
	}

	private Vector3 PositionForNote(MelodyNote note)
	{
		var speed = 10f;
		var radius = 5f;
		var center = Parent.transform.position;
		var rotation = Quaternion.Euler(0, Time.time * speed % 360, 0);
		return center + rotation * (Vector3.forward * radius);
	}

	private static void ModifyParticleSystem(ParticleSystem ps)
	{
		var col = ps.colorBySpeed;
		col.enabled = true;

		Gradient grad = new Gradient();
		grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

		col.color = new ParticleSystem.MinMaxGradient(grad);

	}

	protected IEnumerator RemoveParticleSystem(ParticleSystem system, float wait)
	{
		system.Stop();
		if (wait > 0)
		{
			yield return new WaitForSeconds(wait);
		}

		Destroy(system.gameObject);
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
