﻿using MusicLibrary.Core.Objects;
using MusicLibrary.PandaPlayer.ViewModels.Interfaces;

namespace MusicLibrary.PandaPlayer.Events.SongListEvents
{
	public class PlaylistChangedEventArgs : BaseSongListEventArgs
	{
		public Song CurrentSong { get; }

		public PlaylistChangedEventArgs(ISongPlaylistViewModel playlist)
			: base(playlist.Songs)
		{
			CurrentSong = playlist.CurrentSong;
		}
	}
}