﻿using System;
using System.Threading.Tasks;
using CF.MusicLibrary.BL;
using CF.MusicLibrary.BL.Interfaces;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class EditDiscPropertiesViewModel : ViewModelBase, IEditDiscPropertiesViewModel
	{
		private readonly ILibraryStructurer libraryStructurer;
		private readonly ILibraryContentUpdater libraryContentUpdater;

		private Disc Disc { get; set; }

		private string folderName;
		public string FolderName
		{
			get { return folderName; }
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of Disc folder name could not be empty");
				}
				Set(ref folderName, value);
			}
		}

		private string discTitle;
		public string DiscTitle
		{
			get { return discTitle; }
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					throw new InvalidOperationException("Value of Disc title could not be empty");
				}
				Set(ref discTitle, value);
			}
		}

		private string albumTitle;
		public string AlbumTitle
		{
			get { return albumTitle; }
			set
			{
				if (value?.Length == 0)
				{
					value = null;
				}
				Set(ref albumTitle, value);
			}
		}

		public EditDiscPropertiesViewModel(ILibraryStructurer libraryStructurer, ILibraryContentUpdater libraryContentUpdater)
		{
			if (libraryStructurer == null)
			{
				throw new ArgumentNullException(nameof(libraryStructurer));
			}
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}

			this.libraryStructurer = libraryStructurer;
			this.libraryContentUpdater = libraryContentUpdater;
		}

		public void Load(Disc disc)
		{
			Disc = disc;
			FolderName = libraryStructurer.GetDiscFolderName(disc.Uri);
			DiscTitle = disc.Title;
			AlbumTitle = disc.AlbumTitle;
		}

		public async Task Save()
		{
			var updatedProperties = UpdatedSongProperties.None;
			if (!String.Equals(Disc.AlbumTitle, AlbumTitle, StringComparison.OrdinalIgnoreCase))
			{
				updatedProperties |= UpdatedSongProperties.Album;
			}

			Disc.Title = DiscTitle;
			Disc.AlbumTitle = AlbumTitle;

			var originalDiscFolderName = libraryStructurer.GetDiscFolderName(Disc.Uri);
			if (!String.Equals(originalDiscFolderName, FolderName, StringComparison.OrdinalIgnoreCase))
			{
				await libraryContentUpdater.ChangeDiscUri(Disc, libraryStructurer.ReplaceDiscPartInUri(Disc.Uri, FolderName));
			}

			await libraryContentUpdater.UpdateDisc(Disc, updatedProperties);
		}
	}
}