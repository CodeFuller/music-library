﻿using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Application;

namespace CF.MusicLibrary.BL
{
	public partial class RepositoryAndStorageMusicLibrary : IMusicLibraryWriter, IMusicLibraryReader
	{
		public async Task AddSong(Song song, string songSourceFileName)
		{
			await libraryStorage.AddSong(song, songSourceFileName);
			await libraryRepository.AddSong(song);
		}

		public async Task UpdateSong(Song song, UpdatedSongProperties updatedProperties)
		{
			if ((updatedProperties & SongTagData.TaggedProperties) != 0)
			{
				await libraryStorage.UpdateSongTagData(song, updatedProperties);
			}
			await libraryRepository.UpdateSong(song);
		}

		public async Task DeleteSong(Song song, DateTime deleteTime)
		{
			await libraryStorage.DeleteSong(song);

			song.DeleteDate = deleteTime;
			await libraryRepository.UpdateSong(song);
		}

		public async Task ChangeDiscUri(Disc disc, Uri newDiscUri)
		{
			await libraryStorage.ChangeDiscUri(disc, newDiscUri);
			disc.Uri = newDiscUri;
			await libraryRepository.UpdateDisc(disc);

			foreach (var song in disc.Songs)
			{
				song.Uri = libraryStructurer.ReplaceDiscPartInSongUri(newDiscUri, song.Uri);
				await libraryRepository.UpdateSong(song);
			}
		}

		public async Task ChangeSongUri(Song song, Uri newSongUri)
		{
			await libraryStorage.ChangeSongUri(song, newSongUri);
			song.Uri = newSongUri;
			await libraryRepository.UpdateSong(song);
		}

		public async Task UpdateDisc(Disc disc, UpdatedSongProperties updatedProperties)
		{
			if ((updatedProperties & SongTagData.TaggedProperties) != 0)
			{
				foreach (var song in disc.Songs)
				{
					await libraryStorage.UpdateSongTagData(song, updatedProperties);
				}
			}
			await libraryRepository.UpdateDisc(disc);
		}

		public async Task DeleteDisc(Disc disc)
		{
			Logger.WriteInfo($"Deleting disc '{disc.Title}'");
			var deleteTime = DateTime.Now;
			foreach (var song in disc.Songs)
			{
				await DeleteSong(song, deleteTime);
			}
			Logger.WriteInfo($"Disc '{disc.Title}' was deleted successfully");
		}

		public async Task SetDiscCoverImage(Disc disc, string coverImageFileName)
		{
			await libraryStorage.SetDiscCoverImage(disc, coverImageFileName);
		}

		public async Task AddSongPlayback(Song song, DateTime playbackTime)
		{
			await libraryRepository.AddSongPlayback(song, playbackTime);
		}

		public async Task FixSongTagData(Song song)
		{
			await libraryStorage.FixSongTagData(song);
		}
	}
}
