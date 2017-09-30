﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.BL.Interfaces
{
	public interface IMusicLibraryReader
	{
		/// <summary>
		/// Loads all library discs including deleted.
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<Disc>> LoadDiscs();

		Task<DiscLibrary> LoadLibrary();

		Task<SongTagData> GetSongTagData(Song song);

		Task<IEnumerable<SongTagType>> GetSongTagTypes(Song song);

		Task<string> GetSongFile(Song song);

		Task<string> GetDiscCoverImage(Disc disc);

		Task CheckStorage(DiscLibrary library, ILibraryStorageInconsistencyRegistrator registrator);
	}
}
