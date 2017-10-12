﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.Events.DiscEvents;
using CF.MusicLibrary.PandaPlayer.Events.SongEvents;
using CF.MusicLibrary.PandaPlayer.Events.SongListEvents;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		private const int ExplorerSongListIndex = 0;
		private const int PlaylistSongListIndex = 1;

		private const string DefaultTitle = "Panda Player";

		private readonly DiscLibrary discLibrary;

		private readonly IViewNavigator viewNavigator;

		private string title = DefaultTitle;
		public string Title
		{
			get { return title; }
			set { Set(ref title, value); }
		}

		public IApplicationViewModelHolder ViewModelHolder { get; }

		public ILibraryExplorerViewModel LibraryExplorerViewModel => ViewModelHolder.LibraryExplorerViewModel;

		public ISongPlaylistViewModel Playlist => MusicPlayerViewModel.Playlist;

		public IMusicPlayerViewModel MusicPlayerViewModel { get; }

		private int selectedSongListIndex;
		public int SelectedSongListIndex
		{
			get { return selectedSongListIndex; }
			set
			{
				Set(ref selectedSongListIndex, value);
				ActiveDisc = (SelectedSongListIndex == ExplorerSongListIndex) ? LibraryExplorerViewModel.SelectedDisc : PlaylistActiveDisc;
			}
		}

		private Disc activeDisc;
		private Disc ActiveDisc
		{
			set
			{
				if (activeDisc != value)
				{
					activeDisc = value;
					Messenger.Default.Send(new ActiveDiscChangedEventArgs(activeDisc));
				}
			}
		}

		private Disc PlaylistActiveDisc => Playlist.CurrentSong?.Disc ?? Playlist.PlayedDisc;

		public ICommand LoadCommand { get; }

		public ICommand ReversePlayingCommand { get; }

		public ICommand ShowLibraryStatisticsCommand { get; }

		public ApplicationViewModel(DiscLibrary discLibrary, IApplicationViewModelHolder viewModelHolder, IMusicPlayerViewModel musicPlayerViewModel, IViewNavigator viewNavigator)
		{
			if (discLibrary == null)
			{
				throw new ArgumentNullException(nameof(discLibrary));
			}
			if (viewModelHolder == null)
			{
				throw new ArgumentNullException(nameof(viewModelHolder));
			}
			if (musicPlayerViewModel == null)
			{
				throw new ArgumentNullException(nameof(musicPlayerViewModel));
			}
			if (viewNavigator == null)
			{
				throw new ArgumentNullException(nameof(viewNavigator));
			}

			this.discLibrary = discLibrary;
			ViewModelHolder = viewModelHolder;
			MusicPlayerViewModel = musicPlayerViewModel;
			this.viewNavigator = viewNavigator;

			LoadCommand = new AsyncRelayCommand(Load);
			ReversePlayingCommand = new RelayCommand(ReversePlaying);
			ShowLibraryStatisticsCommand = new RelayCommand(ShowLibraryStatistics);

			Messenger.Default.Register<PlaySongsListEventArgs>(this, OnPlaySongList);
			Messenger.Default.Register<PlayDiscFromSongEventArgs>(this, OnPlayDiscFromSongLaunched);
			Messenger.Default.Register<PlayPlaylistStartingFromSongEventArgs>(this, OnPlayPlaylistStartingFromSong);
			Messenger.Default.Register<PlaylistFinishedEventArgs>(this, OnPlaylistFinished);
			Messenger.Default.Register<LibraryExplorerDiscChangedEventArgs>(this, e => SwitchToExplorerSongList());
			Messenger.Default.Register<PlaylistChangedEventArgs>(this, e => OnPlaylistSongChanged());
			Messenger.Default.Register<PlaylistLoadedEventArgs>(this, e => OnPlaylistSongChanged());
			Messenger.Default.Register<NavigateLibraryExplorerToDiscEventArgs>(this, e => NavigateLibraryExplorerToDisc(e.Disc));
		}

		public async Task Load()
		{
			await discLibrary.Load();
			Messenger.Default.Send(new LibraryLoadedEventArgs(discLibrary));

			//	Setting playlist active if some previous playlist was loaded.
			if (Playlist.Songs.Any())
			{
				SwitchToSongPlaylist();
			}
		}

		public void ShowLibraryStatistics()
		{
			viewNavigator.ShowLibraryStatisticsView();
		}

		private void OnPlaySongList(PlaySongsListEventArgs e)
		{
			Playlist.SetSongs(e.Songs);
			Playlist.SwitchToNextSong();
			MusicPlayerViewModel.Play();
			SwitchToSongPlaylist();
		}

		private void OnPlayDiscFromSongLaunched(PlayDiscFromSongEventArgs message)
		{
			var disc = message.Song.Disc;
			Playlist.SetSongs(disc.Songs);
			Playlist.SwitchToSong(message.Song);
			MusicPlayerViewModel.Play();
			SwitchToSongPlaylist();
		}

		private void OnPlayPlaylistStartingFromSong(PlayPlaylistStartingFromSongEventArgs message)
		{
			MusicPlayerViewModel.Stop();
			MusicPlayerViewModel.Play();
			SwitchToSongPlaylist();
		}

		internal void ReversePlaying()
		{
			if (MusicPlayerViewModel.IsPlaying)
			{
				MusicPlayerViewModel.Pause();
			}
			else
			{
				MusicPlayerViewModel.Play();
			}
		}

		private void OnPlaylistSongChanged()
		{
			Title = Playlist.CurrentSong != null ? BuildCurrentTitle(Playlist.CurrentSong) : DefaultTitle;

			if (SelectedSongListIndex == PlaylistSongListIndex)
			{
				ActiveDisc = PlaylistActiveDisc;
			}
		}

		private string BuildCurrentTitle(Song song)
		{
			var songTitle = song.Artist != null ? Current($"{song.Artist.Name} - {song.Title}") : song.Title;
			return Current($"{Playlist.CurrentSongIndex + 1}/{Playlist.SongsNumber} - {songTitle}");
		}

		private void OnPlaylistFinished(PlaylistFinishedEventArgs e)
		{
			var playedDisc = Playlist.PlayedDisc;
			if (playedDisc == null || playedDisc.Songs.All(s => s.Rating != null))
			{
				return;
			}

			viewNavigator.ShowRateDiscView(playedDisc);
		}

		private void SwitchToExplorerSongList()
		{
			SelectedSongListIndex = ExplorerSongListIndex;
		}

		private void SwitchToSongPlaylist()
		{
			SelectedSongListIndex = PlaylistSongListIndex;
		}

		private void NavigateLibraryExplorerToDisc(Disc disc)
		{
			LibraryExplorerViewModel.SwitchToDisc(disc);
			SwitchToExplorerSongList();
		}
	}
}
