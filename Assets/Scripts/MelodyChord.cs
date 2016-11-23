using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
	[Serializable]
	public class MelodyChord : MelodyNote
	{
		public List<MelodyNote> Notes = new List<MelodyNote>();

		public void AddNote(MelodyNote note)
		{
			Notes.Add(note);
			// align start with start of chord
			note.Start -= Start;
		}
	}
}