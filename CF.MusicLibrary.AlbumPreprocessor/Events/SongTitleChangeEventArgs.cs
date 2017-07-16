﻿using System;

namespace CF.MusicLibrary.AlbumPreprocessor.Events
{
	public abstract class SongTitleChangeEventArgs : EventArgs
	{
		public string OldTitle { get; set; }

		public string NewTitle { get; set; }

		protected SongTitleChangeEventArgs(string oldTitle, string newTitle)
		{
			OldTitle = oldTitle;
			NewTitle = newTitle;
		}
	}
}