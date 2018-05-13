﻿using System;
using System.Linq;
using System.Text;
using CF.Library.Core.Extensions;
using CF.Library.Core.Facades;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.PandaPlayer;
using CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.PandaPlayer.ViewModels.PersistentPlaylist
{
	[TestFixture]
	public class JsonFileGenericRepositoryPlaylistDataTests
	{
		[Test]
		public void Load_LoadsSavedPlaylistDataCorrectly()
		{
			// Arrange

			var song1 = new Song { Id = 1, Uri = new Uri("/SongUri1", UriKind.Relative) };
			var song2 = new Song { Id = 2, Uri = new Uri("/SongUri2", UriKind.Relative) };

			PlaylistData playlistData = new PlaylistData
			{
				Songs = new[] { new PlaylistSongData(song1), new PlaylistSongData(song2) }.ToCollection(),
				CurrentSongIndex = 1,
			};

			string writtenData = null;
			IFileSystemFacade fileSystemFacadeStub = Substitute.For<IFileSystemFacade>();
			fileSystemFacadeStub.FileExists("SomeFile.json").Returns(true);
			fileSystemFacadeStub.WriteAllText("SomeFile.json", Arg.Do<string>(arg => writtenData = arg), Encoding.UTF8);
			fileSystemFacadeStub.ReadAllText("SomeFile.json", Encoding.UTF8).Returns(x => writtenData);

			var target = new JsonFileGenericRepository<PlaylistData>(fileSystemFacadeStub, Substitute.For<ILogger<JsonFileGenericRepository<PlaylistData>>>(), "SomeFile.json");

			// Act

			target.Save(playlistData);
			var loadedPlaylistData = target.Load();

			// Assert

			Assert.IsNotNull(loadedPlaylistData);
			var loadedSongs = loadedPlaylistData.Songs.ToList();
			Assert.AreEqual(2, loadedSongs.Count);
			Assert.AreEqual(1, loadedSongs[0].Id);
			Assert.AreEqual(new Uri("/SongUri1", UriKind.Relative), loadedSongs[0].Uri);
			Assert.AreEqual(2, loadedSongs[1].Id);
			Assert.AreEqual(new Uri("/SongUri2", UriKind.Relative), loadedSongs[1].Uri);
			Assert.AreEqual(1, loadedPlaylistData.CurrentSongIndex);
		}
	}
}
