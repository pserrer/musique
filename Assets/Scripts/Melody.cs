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
        public int AverageOctaveMidiLeftHand;
        public int AverageOctaveMidiRightHand;

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

            var lastNote = this.Notes[this.Notes.Count - 1];
            this.Duration = lastNote.Start + lastNote.Duration;
            Debug.Log("Dauer der Melodie : " + Duration);

            var keysPerOctave = 12;
            var averages = GetAverageForEachHand(this.Duration, this.Duration, "midi");
            if (averages[0] != -1f)
            {
                this.AverageOctaveMidiLeftHand = (int)(averages[0] / keysPerOctave);
            } else
            {
                this.AverageOctaveMidiLeftHand = -1;
            }
            if (averages[1] != -1f)
            {
                this.AverageOctaveMidiRightHand = (int)(averages[1] / keysPerOctave);
            } else
            {
                this.AverageOctaveMidiRightHand = -1;
            }

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

        public float GetAverageMidiNoteLastSeconds(float duration, float time, string mode)
        {
	        var filteredList = Notes.FindAll(x => x.Start < time && x.Start + x.Duration > time - duration);
            return this.GetAverage(filteredList, mode);
        }

        public float[] GetAverageForEachHand(float duration, float time, string mode)
        {
            float[] averageForEachHand = {0f , 0f};
            var filteredList = Notes.FindAll(x => x.Start < time && x.Start + x.Duration > time - duration);
            
            var filteredListLeft = filteredList.FindAll(x => x.GetMidiNote() <= 30);
            var filteredListRight = filteredList.FindAll(x => x.GetMidiNote() > 30);
            
            averageForEachHand[0] = this.GetAverage(filteredListLeft, mode);
            averageForEachHand[1] = this.GetAverage(filteredListRight, mode);

            return averageForEachHand;
        }

        private float GetAverage(List<MelodyNote> Notes, string mode)
        {
            if (Notes.Count != 0)
            {
                var sum = 0f;
                if (mode.Equals("midi"))
                {
                    sum = Notes.Aggregate<MelodyNote, float>(0, (current, note) => current + note.GetMidiNote());
                } else if (mode.Equals("velocity"))
                {
                    sum = Notes.Aggregate<MelodyNote, float>(0, (current, note) => current + note.Velocity);
                    return sum;
                }
                return sum / Notes.Count;
            } else
            {
                return -1f;
            }
        }
    }
}