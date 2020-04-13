﻿using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.Events.DiscEvents;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.ViewModels.DiscImages
{
	public class DiscImageViewModel : ViewModelBase, IDiscImageViewModel
	{
		private readonly IMusicLibrary musicLibrary;
		private readonly IViewNavigator viewNavigator;

		private Disc currentDisc;

		private Disc CurrentDisc
		{
			get => currentDisc;
			set
			{
				Set(ref currentDisc, value);
				CurrImageFileName = GetCurrImageFileName();
			}
		}

		private string currImageFileName;

		public string CurrImageFileName
		{
			get => currImageFileName;
			private set
			{
				// Why don't we use ViewModelBase.Set(ref currImageFileName, value) ?
				// When disc image is updated with new file, CurrImageFileName is not actually changed, however
				// we need PropertyChanged event to be fired so that Image control updated image in the view.
				// Seems like ViewModelBase.Set() has some internal check whether new value equals to the old one
				// and don't fire the event in this case. That's why we should raise event manually.
				currImageFileName = value;
				RaisePropertyChanged();
			}
		}

		public DiscImageViewModel(IMusicLibrary musicLibrary, IViewNavigator viewNavigator)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.viewNavigator = viewNavigator ?? throw new ArgumentNullException(nameof(viewNavigator));

			Messenger.Default.Register<ActiveDiscChangedEventArgs>(this, e => CurrentDisc = e.Disc);
			Messenger.Default.Register<DiscImageChangedEventArgs>(this, e => OnDiscImageChanged(e.Disc));
		}

		private string GetCurrImageFileName()
		{
			var activeDisc = CurrentDisc;
			return activeDisc == null ? null : musicLibrary.GetDiscCoverImage(activeDisc).Result;
		}

		public async Task EditDiscImage()
		{
			var activeDisc = CurrentDisc;
			if (activeDisc == null)
			{
				return;
			}

			await viewNavigator.ShowEditDiscImageView(activeDisc);
		}

		private void OnDiscImageChanged(Disc disc)
		{
			if (disc == CurrentDisc)
			{
				CurrImageFileName = GetCurrImageFileName();
			}
		}
	}
}