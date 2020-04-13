﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.LibraryChecker.Registrators;

namespace MusicLibrary.LibraryChecker.Checkers
{
	public class DiscConsistencyChecker : IDiscConsistencyChecker
	{
		private readonly ILibraryInconsistencyRegistrator inconsistencyRegistrator;
		private readonly IDiscTitleToAlbumMapper discTitleToAlbumMapper;
		private readonly ICheckScope checkScope;
		private readonly ILogger<DiscConsistencyChecker> logger;

		public DiscConsistencyChecker(ILibraryInconsistencyRegistrator inconsistencyRegistrator, IDiscTitleToAlbumMapper discTitleToAlbumMapper,
			ICheckScope checkScope, ILogger<DiscConsistencyChecker> logger)
		{
			this.inconsistencyRegistrator = inconsistencyRegistrator ?? throw new ArgumentNullException(nameof(inconsistencyRegistrator));
			this.discTitleToAlbumMapper = discTitleToAlbumMapper ?? throw new ArgumentNullException(nameof(discTitleToAlbumMapper));
			this.checkScope = checkScope ?? throw new ArgumentNullException(nameof(checkScope));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task CheckDiscsConsistency(IEnumerable<Disc> discs, CancellationToken cancellationToken)
		{
			logger.LogInformation("Checking discs consistency ...");

			foreach (var disc in discs.Where(d => checkScope.Contains(d)))
			{
				// Checking album title
				if (discTitleToAlbumMapper.AlbumTitleIsSuspicious(disc.AlbumTitle))
				{
					inconsistencyRegistrator.RegisterSuspiciousAlbumTitle(disc);
				}

				// Check that disc has some songs
				if (!disc.Songs.Any())
				{
					inconsistencyRegistrator.RegisterDiscWithoutSongs(disc);
					continue;
				}

				// Checking songs order & track numbers
				var trackNumbers = disc.Songs.Select(s => s.TrackNumber).ToList();
				if (trackNumbers.Any(n => n != null))
				{
					if (trackNumbers.Any(n => n == null) || trackNumbers.First() != 1 || trackNumbers.Last() != trackNumbers.Count)
					{
						inconsistencyRegistrator.RegisterBadTrackNumbersForDisc(disc, trackNumbers);
					}
				}

				// Checking that all disc songs has equal genre
				var genres = disc.Songs.Select(s => s.Genre).Distinct().ToList();
				if (genres.Count > 1)
				{
					inconsistencyRegistrator.RegisterDifferentGenresForDisc(disc, genres);
				}
			}

			await Task.FromResult(0);
		}
	}
}