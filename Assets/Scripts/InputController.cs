using System;
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

	public string MicrophoneDevice = string.Empty;
	public int MicrophoneFreq = 48000;

	// Use this for initialization
	void Start()
	{
		_keys = Enumerable.Range((int)KeyCode.A, 26).Select(x => (KeyCode)x);

		if (string.IsNullOrEmpty(MicrophoneDevice))
		{
			MicrophoneDevice = Microphone.devices.FirstOrDefault(x => !string.IsNullOrEmpty(x));
			if (string.IsNullOrEmpty(MicrophoneDevice))
			{
				Debug.Log("No Microphone found :(");
			}
		}

		int minFreq, maxFreq;
		Microphone.GetDeviceCaps(MicrophoneDevice, out minFreq, out maxFreq);
		MicrophoneFreq = (minFreq + maxFreq)/2;

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
        Invoke("StopMelody", SilenceBetweenMelodies);
    }

	private void NoteDown(MelodyNote note)
	{
        CancelInvoke("StopMelody");

		if (!_activeNotes.ContainsKey(note.GetHashCode()))
		{
			// there's currently no input - so we should start recording now.
			// we assume, that the melody isn't longer than 10 minutes
			if (CurrentMelody.IsEmpty)
			{
				CurrentMelody.Audio = Microphone.Start(MicrophoneDevice, true, 600, MicrophoneFreq);
			}

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
        CurrentMelody = new Melody
		{
			Start = Time.time
		};
	}

	public void StopMelody()
	{
		Debug.Log("Finished Melody");
		if (!CurrentMelody.IsEmpty)
		{
			Debug.Log("End Microphone: " + Microphone.GetPosition(MicrophoneDevice));
			var pos = Microphone.GetPosition(MicrophoneDevice);
			Microphone.End(MicrophoneDevice);

			// trim the audio clip
			var clip = CurrentMelody.Audio;
			var data = new float[pos];
			var trimmedClip = AudioClip.Create("Trimmed Microphone", pos,
				clip.channels, clip.frequency, false);
			clip.GetData(data, 0);
			trimmedClip.SetData(data, 0);
			CurrentMelody.Audio = trimmedClip;

			CurrentMelody.IsFinished = true;
			ParticleController.AddMelody(CurrentMelody);
		}

		// we immediatelly start a new melody
		// the recording starts with first key press
		StartNewMelody();
	}
}
