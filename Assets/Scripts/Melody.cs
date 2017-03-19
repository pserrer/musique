using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[Serializable]
	public class Melody
	{
		public List<MelodyNote> Notes = new List<MelodyNote>();
        public bool IsFinished = false;
        public float Duration = 0f;
        public float AverageMidiNote;

		public bool IsEmpty
		{
			get { return Notes.Count == 0; }
		}

		public float Start { get; set; }

        public void Analyze()
        {
            MelodyNote LastNote = Notes[Notes.Count-1];
            MelodyNote FirstNote = Notes[0];
            this.Duration = LastNote.Start + LastNote.Duration;
            Debug.Log("Dauer der Melodie : " + this.Duration);

            float SumOfMidiNotes = 0f;
            float SumOfNoteDurations = 0f;
            foreach (MelodyNote note in Notes)
            {
                SumOfNoteDurations += note.Duration;
                SumOfMidiNotes += note.Duration * note.getMidiNote();
                Debug.Log("Note: " + note.getMidiNote() + " von " + note.Start + " - " + (note.Start + note.Duration) + " mit Stärke: " + note.Velocity);
            }
            this.AverageMidiNote = SumOfMidiNotes / SumOfNoteDurations;
            Debug.Log("AverageMidiNote: " + this.AverageMidiNote);
        }

        public float getAverageMidiNoteLastSeconds(float duration, float time)
        {
            float MidiNoteSum = 0;
            List<MelodyNote> FilteredList = Notes.FindAll(x => x.Start < time && x.Start + x.Duration > time - duration);
            string NoteStarts = "";
            foreach (MelodyNote Note in FilteredList)
            {
                NoteStarts += Note.Start.ToString() + ", ";
                MidiNoteSum += Note.getMidiNote();
            }
            if (NoteStarts != "")
                Debug.Log("NoteStarts: " + NoteStarts);
            return MidiNoteSum / FilteredList.Count;
        }
	}
}