﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.LibraryToolkit.Interfaces;
using MusicLibraryApi.Client.Interfaces;

namespace MusicLibrary.LibraryToolkit.Seeders
{
	public class GenresSeeder : IGenresSeeder
	{
		private readonly IGenresMutation genresMutation;

		private readonly ILogger<ArtistsSeeder> logger;

		public GenresSeeder(IGenresMutation genresMutation, ILogger<ArtistsSeeder> logger)
		{
			this.genresMutation = genresMutation ?? throw new ArgumentNullException(nameof(genresMutation));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task<IReadOnlyDictionary<int, int>> SeedGenres(CancellationToken cancellationToken)
		{
			// TODO: Restore this functionality or delete it completely.
			throw new NotImplementedException();
/*
			var genres = new Dictionary<int, int>();
			foreach (var genre in discLibrary.Genres.OrderBy(g => g.Name))
			{
				var genreData = new InputGenreData { Name = genre.Name };
				var newGenreId = await genresMutation.CreateGenre(genreData, cancellationToken);

				genres.Add(genre.Id, newGenreId);
			}

			logger.LogInformation("Seeded {GenresNumber} genres", genres.Count);

			return genres;
*/
		}
	}
}
