using System;
using UnityEngine;

namespace Assets.Scripts
{
	[Serializable]
	public class MelodyNote
	{

		public Note Note;

		public float Start;

		public float Duration;

		public ParticleSystem ParticleSystem;

		public MelodyNote(char letter, int accidental = 0)
		{
			Note = new Note(letter, accidental);
		}

		public MelodyNote() { }
		public int Octave { get; set; }
	}
}
