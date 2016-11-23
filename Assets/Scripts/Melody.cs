using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
	[Serializable]
	public class Melody
	{
		public List<MelodyNote> Notes = new List<MelodyNote>();

		public bool IsEmpty
		{
			get { return Notes.Count == 0; }
		}

		public float Start { get; set; }
	}
}