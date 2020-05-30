﻿using System;
using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.SongEvents
{
	internal class SongChangedEventArgs : BaseSongEventArgs
	{
		public string PropertyName { get; set; }

		public SongChangedEventArgs(SongModel song, string propertyName)
			: base(song)
		{
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
		}
	}
}
