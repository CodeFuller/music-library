﻿using System;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.PandaPlayer.Events.SongEvents
{
	public abstract class BaseSongEventArgs : EventArgs
	{
		public Song Song { get; }

		protected BaseSongEventArgs(Song song)
		{
			Song = song;
		}
	}
}