using System;
using UnityEngine;

namespace Assets.Scripts
{
	[Serializable]
	public class MelodyNote
	{
		public const int DefaultOctave = 4;
        public char Letter { get; set; }
        public int Accidental { get; set; }
        public int Octave { get; set; }
        public float Start { get; set; }
        public float Duration { get; set; }
        public float Velocity { get; set; }

        public MelodyNote(char letter, int accidental = 0, int octave = DefaultOctave)
		{
			this.Letter = letter;
			this.Accidental = accidental;
			this.Octave = octave;
		}

		public MelodyNote() { }

		protected bool Equals(MelodyNote other)
		{
			return Letter == other.Letter && Accidental == other.Accidental && Octave == other.Octave;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MelodyNote) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Letter.GetHashCode();
				hashCode = (hashCode*397) ^ Accidental;
				hashCode = (hashCode*397) ^ Octave;
				return hashCode;
			}
		}

		public static MelodyNote FromMidiNote(int note)
		{
			var octave = note / 12;
			var noteInOctave = note - octave * 12;
			switch (noteInOctave)
			{
				case 0:
					return new MelodyNote('C', 0, octave);
				case 1:
					return new MelodyNote('C', 1, octave);
				case 2:
					return new MelodyNote('D', 0, octave);
				case 3:
					return new MelodyNote('D', 1, octave);
				case 4:
					return new MelodyNote('E', 0, octave);
				case 5:
					return new MelodyNote('F', 0, octave);
				case 6:
					return new MelodyNote('F', 1, octave);
				case 7:
					return new MelodyNote('G', 0, octave);
				case 8:
					return new MelodyNote('G', 1, octave);
				case 9:
					return new MelodyNote('A', 0, octave);
				case 10:
					return new MelodyNote('A', 1, octave);
				case 11:
					return new MelodyNote('B', 0, octave);
			}
			throw new Exception("Invalid Note");
		}

		public static MelodyNote FromKeyCode(KeyCode key)
		{
			switch (key)
			{
				case KeyCode.A:
					return new MelodyNote('C');
				case KeyCode.W:
					return new MelodyNote('C', 1);
				case KeyCode.S:
					return new MelodyNote('D');
				case KeyCode.E:
					return new MelodyNote('D', 1);
				case KeyCode.D:
					return new MelodyNote('E');
				case KeyCode.F:
					return new MelodyNote('F');
				case KeyCode.T:
				case KeyCode.R:
					return new MelodyNote('F', 1);
				case KeyCode.G:
					return new MelodyNote('G');
				case KeyCode.Z:
					return new MelodyNote('G', 1);
				case KeyCode.H:
					return new MelodyNote('A');
				case KeyCode.U:
					return new MelodyNote('A', 1);
				case KeyCode.J:
					return new MelodyNote('B');
			}

			return null;
		}
	}
}
