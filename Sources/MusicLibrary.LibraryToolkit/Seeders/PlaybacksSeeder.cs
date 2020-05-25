﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.LibraryToolkit.Interfaces;
using MusicLibraryApi.Client.Interfaces;

namespace MusicLibrary.LibraryToolkit.Seeders
{
	public class PlaybacksSeeder : IPlaybacksSeeder
	{
		private readonly IPlaybacksMutation playbacksMutation;

		private readonly ILogger<PlaybacksSeeder> logger;

		public PlaybacksSeeder(IPlaybacksMutation playbacksMutation, ILogger<PlaybacksSeeder> logger)
		{
			this.playbacksMutation = playbacksMutation ?? throw new ArgumentNullException(nameof(playbacksMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task<IReadOnlyDictionary<int, int>> SeedPlaybacks(IReadOnlyDictionary<int, int> songs, CancellationToken cancellationToken)
		{
			// TODO: Restore this functionality or delete it completely.
			throw new NotImplementedException();
/*
			// Verifying songs playbacks
			foreach (var song in discLibrary.AllSongs.OrderBy(s => s.Id))
			{
				var lastPlaybackTime1 = song.LastPlaybackTime;
				var lastPlaybackTime2 = song.Playbacks.OrderByDescending(p => p.PlaybackTime).FirstOrDefault()?.PlaybackTime;
				if (lastPlaybackTime1 != lastPlaybackTime2)
				{
					// MediaMonkey player registers playbacks with small delta.
					if (lastPlaybackTime1 == null || lastPlaybackTime2 == null || Math.Abs((lastPlaybackTime1.Value - lastPlaybackTime2.Value).TotalSeconds) > 60)
					{
						throw new InvalidOperationException($"Playbacks info for song {song.Id} is inconsistent: {lastPlaybackTime1:yyyy.MM.dd HH:mm:ss} != {lastPlaybackTime2:yyyy.MM.dd HH:mm:ss}");
					}
				}

				if (song.PlaybacksCount != song.Playbacks.Count)
				{
					throw new InvalidOperationException($"Playbacks counters for song {song.Id} are inconsistent: {song.PlaybacksCount} != {song.Playbacks.Count}");
				}
			}

			var playbacks = new Dictionary<int, int>();
			foreach (var playback in discLibrary.AllSongs.SelectMany(s => s.Playbacks).OrderBy(p => p.PlaybackTime))
			{
				if (!songs.TryGetValue(playback.Song.Id, out var songId))
				{
					throw new InvalidOperationException($"The new id for song {playback.Song.Title} is unknown");
				}

				var playbackData = new InputPlaybackData
				{
					SongId = songId,
					PlaybackTime = playback.PlaybackTime,
				};

				var playbackId = await playbacksMutation.AddSongPlayback(playbackData, cancellationToken);
				playbacks.Add(playback.Id, playbackId);
			}

			logger.LogInformation("Seeded {PlaybacksNumber} playbacks", playbacks.Count);

			return playbacks;
*/
		}
	}
}
