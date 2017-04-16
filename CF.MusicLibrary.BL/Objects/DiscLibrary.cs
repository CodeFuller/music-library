﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.BL.Objects
{
	/// <summary>
	/// Collection of Music Discs without groupping by Artist.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Suffix 'Library' is suitable in this context")]
	public class DiscLibrary : IEnumerable<Disc>
	{
		private readonly List<Disc> discs;

		public IReadOnlyCollection<Disc> Discs => discs;

		public DiscLibrary(IEnumerable<Disc> libraryDiscs)
		{
			discs = libraryDiscs.ToList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Disc> GetEnumerator()
		{
			return discs.GetEnumerator();
		}
	}
}
