﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.LibraryChecker.Checkers
{
	public interface ILastFMConsistencyChecker
	{
		Task CheckArtists(DiscLibrary library);

		Task CheckAlbums(IEnumerable<Disc> discs);

		Task CheckSongs(IEnumerable<Song> songs);
	}
}
