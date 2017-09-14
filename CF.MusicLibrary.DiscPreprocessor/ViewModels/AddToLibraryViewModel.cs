﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Media;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.DiscPreprocessor.ViewModels
{
	public class AddToLibraryViewModel : ViewModelBase, IAddToLibraryViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly ISongMediaInfoProvider songMediaInfoProvider;
		private readonly IFileSystemFacade fileSystemFacade;

		private readonly bool deleteSourceContent;

		private List<AddedSong> addedSongs;
		private List<AddedDiscCoverImage> discsCoverImages;

		public string Name => "Add To Library";

		private bool dataIsReady;
		public bool DataIsReady 
		{
			get { return dataIsReady; }
			set { Set(ref dataIsReady, value); }
		}

		public ICommand AddToLibraryCommand { get; }

		private int currProgress;
		public int CurrProgress
		{
			get { return currProgress; }
			set { Set(ref currProgress, value); }
		}

		private int progressSize;
		public int ProgressSize
		{
			get { return progressSize; }
			set { Set(ref progressSize, value); }
		}

		private string progressMessages;
		public string ProgressMessages
		{
			get { return progressMessages; }
			set { Set(ref progressMessages, value); }
		}

		public void SetSongs(IEnumerable<AddedSong> songs)
		{
			addedSongs = songs.ToList();
		}

		public void SetDiscsCoverImages(IEnumerable<AddedDiscCoverImage> coverImages)
		{
			discsCoverImages = coverImages.ToList();
		}

		public AddToLibraryViewModel(IMusicLibrary musicLibrary, ISongMediaInfoProvider songMediaInfoProvider, IFileSystemFacade fileSystemFacade, bool deleteSourceContent)
		{
			if (musicLibrary == null)
			{
				throw new ArgumentNullException(nameof(musicLibrary));
			}
			if (songMediaInfoProvider == null)
			{
				throw new ArgumentNullException(nameof(songMediaInfoProvider));
			}
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.musicLibrary = musicLibrary;
			this.songMediaInfoProvider = songMediaInfoProvider;
			this.fileSystemFacade = fileSystemFacade;
			this.deleteSourceContent = deleteSourceContent;

			AddToLibraryCommand = new AsyncRelayCommand(AddContentToLibrary);
		}

		public async Task AddContentToLibrary()
		{
			if (addedSongs == null)
			{
				throw new InvalidOperationException("No songs for adding to the library");
			}

			DataIsReady = false;

			CurrProgress = 0;
			ProgressSize = 0;
			ProgressSize += await FillSongsMediaData(true);
			ProgressSize += await AddSongsToLibrary(true);
			await FillSongsMediaData(false);
			await AddSongsToLibrary(false);

			if (deleteSourceContent)
			{
				DeleteSourceDirTree();
			}

			DataIsReady = true;
		}

		private async Task<int> FillSongsMediaData(bool onlyCountProgressSize)
		{
			const int progressIncrement = 1;
			int taskProgressSize = addedSongs.Count * progressIncrement;

			if (onlyCountProgressSize)
			{
				return taskProgressSize;
			}

			foreach (var addedSong in addedSongs)
			{
				ProgressMessages += Current($"Getting media info for '{addedSong.SourceFileName}'...\n");
				var mediaInfo = await songMediaInfoProvider.GetSongMediaInfo(addedSong.SourceFileName);
				addedSong.Song.FileSize = mediaInfo.Size;
				addedSong.Song.Bitrate = mediaInfo.Bitrate;
				addedSong.Song.Duration = mediaInfo.Duration;

				CurrProgress += progressIncrement;
			}

			return taskProgressSize;
		}

		private async Task<int> AddSongsToLibrary(bool onlyCountProgressSize)
		{
			const int progressIncrement = 5;
			int taskProgressSize = 0;

			foreach (AddedSong addedSong in addedSongs)
			{
				if (!onlyCountProgressSize)
				{
					ProgressMessages += Current($"Adding to library '{addedSong.SourceFileName}'...\n");
					await musicLibrary.AddSong(addedSong.Song, addedSong.SourceFileName);
					CurrProgress += progressIncrement;
				}

				taskProgressSize += progressIncrement;
			}

			if (discsCoverImages != null)
			{
				foreach (AddedDiscCoverImage coverImage in discsCoverImages)
				{
					if (!onlyCountProgressSize)
					{
						ProgressMessages += Current($"Adding disc cover image '{coverImage.CoverImageFileName}'...\n");
						await musicLibrary.SetDiscCoverImage(coverImage.Disc, coverImage.CoverImageFileName);
						CurrProgress += progressIncrement;
					}

					taskProgressSize += progressIncrement;
				}
			}

			return taskProgressSize;
		}

		private void DeleteSourceDirTree()
		{
			foreach (var subDirectory in fileSystemFacade.EnumerateDirectories(AppSettings.GetRequiredValue<string>("WorkshopDirectory")))
			{
				List<string> files = new List<string>();
				FindDirectoryFiles(subDirectory, files);

				if (files.Any())
				{
					return;
				}

				ProgressMessages += Current($"Deleting source directory '{subDirectory}'...\n");
				fileSystemFacade.DeleteDirectory(subDirectory, true);
			}
		}

		private void FindDirectoryFiles(string directoryPath, List<string> files)
		{
			foreach (string subDirectory in fileSystemFacade.EnumerateDirectories(directoryPath))
			{
				FindDirectoryFiles(subDirectory, files);
			}

			foreach (string file in fileSystemFacade.EnumerateFiles(directoryPath))
			{
				files.Add(file);
			}
		}
	}
}