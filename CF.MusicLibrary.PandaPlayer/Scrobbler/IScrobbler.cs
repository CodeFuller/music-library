﻿using System.Threading.Tasks;
using CF.MusicLibrary.LastFM.Objects;

namespace CF.MusicLibrary.PandaPlayer.Scrobbler
{
	public interface IScrobbler
	{
		Task UpdateNowPlaying(Track track);

		Task Scrobble(TrackScrobble trackScrobble);
	}
}
