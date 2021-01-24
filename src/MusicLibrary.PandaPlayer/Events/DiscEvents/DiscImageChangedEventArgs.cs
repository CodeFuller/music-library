﻿using MusicLibrary.Core.Models;

namespace MusicLibrary.PandaPlayer.Events.DiscEvents
{
	public class DiscImageChangedEventArgs : BaseDiscEventArgs
	{
		public DiscImageChangedEventArgs(DiscModel disc)
			: base(disc)
		{
		}
	}
}