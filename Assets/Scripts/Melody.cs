using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	[Serializable]
	public class Melody
	{
		public List<MelodyNote> Notes = new List<MelodyNote>();
        public bool IsFinished = false;
        public float Duration;
        public float AverageMidiNote;
        public float AverageMidiLeftHand;
        public float AverageMidiRightHand;

		public bool IsEmpty
		{
			get { return Notes.Count == 0; }
		}

		public float Start { get; set; }

		public AudioClip Audio { get; set; }

		public void Analyze()
        {
	        if (IsEmpty)
	        {
		        return;
	        }

            var lastNote = Notes[Notes.Count-1];
	        Duration = lastNote.Start + lastNote.Duration;
            Debug.Log("Dauer der Melodie : " + Duration);

            var sumOfMidiNotes = 0f;
            var sumOfNoteDurations = 0f;
            foreach (var note in Notes)
            {
                sumOfNoteDurations += note.Duration;
                sumOfMidiNotes += note.Duration * note.GetMidiNote();
                //Debug.Log("Note: " + note.GetMidiNote() + " von " + note.Start + " - " + (note.Start + note.Duration) + " mit Stärke: " + note.Velocity);
            }
            AverageMidiNote = sumOfMidiNotes / sumOfNoteDurations;
            //Debug.Log("AverageMidiNote: " + AverageMidiNote);
        }

        public float GetAverageMidiNoteLastSeconds(float duration, float time)
        {
	        var filteredList = Notes.FindAll(x => x.Start < time && x.Start + x.Duration > time - duration);
	        var midiNoteSum = filteredList.Aggregate<MelodyNote, float>(0, (current, note) => current + note.GetMidiNote());
	        return midiNoteSum / filteredList.Count;
        }

        public float[] GetAverageMidiForEachHand(float duration, float time)
        {
            var filteredList = Notes.FindAll(x => x.Start < time && x.Start + x.Duration > time - duration);

            var filteredListLeft = filteredList.FindAll(x => x.GetMidiNote() <= 63);
            var midiNoteSumLeft = filteredListLeft.Aggregate<MelodyNote, float>(0, (current, note) => current + note.GetMidiNote());

            var filteredListRight = filteredList.FindAll(x => x.GetMidiNote() > 63);
            var midiNoteSumRight = filteredListRight.Aggregate<MelodyNote, float>(0, (current, note) => current + note.GetMidiNote());

            float[] midiForEachHand = { midiNoteSumLeft / filteredListLeft.Count, midiNoteSumRight / filteredListRight.Count };

            return midiForEachHand;
        }
    }
}