﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL
{
	/// <summary>
	/// Interface for music library repository.
	/// </summary>
	public interface IMusicLibraryRepository
	{
		/// <summary>
		/// Loads disc library from the repository.
		/// </summary>
		Task<DiscLibrary> GetDiscLibraryAsync();

		Task<IEnumerable<Artist>> GetArtistsAsync();

		Task<IEnumerable<Disc>> GetDiscsAsync();

		Task<IEnumerable<Song>> GetSongsAsync();

		Task<IEnumerable<Genre>> GetGenresAsync();

		Task AddSongPlayback(Song song, DateTime playbackTime);
	}
}
