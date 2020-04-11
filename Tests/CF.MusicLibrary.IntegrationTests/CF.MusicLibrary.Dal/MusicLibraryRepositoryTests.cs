﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.Dal.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CF.MusicLibrary.IntegrationTests.CF.MusicLibrary.Dal
{
	[TestFixture]
	public class MusicLibraryRepositoryTests
	{
		[Test]
		public void GetDiscs_LoadsDiscsDataCorrectly()
		{
			// Arrange

			var services = new ServiceCollection();

			var binPath = AppDomain.CurrentDomain.BaseDirectory;
			services.AddDal(settings => settings.DataSource = Path.Combine(binPath, "MusicLibrary.db"));

			var serviceProvider = services.BuildServiceProvider();
			var target = serviceProvider.GetRequiredService<IMusicLibraryRepository>();

			// Act

			var discs = target.GetDiscs().Result.ToList();

			// Assert

			Assert.IsNotEmpty(discs);
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Artist).Where(a => a != null));
			Assert.IsNotEmpty(discs.SelectMany(d => d.Songs).Select(s => s.Genre).Where(g => g != null));
			Assert.IsNotEmpty(discs.Select(d => d.CoverImage).Where(c => c != null));
		}

		[Test]
		public void CopyData_CopiesDataFromAllDatabaseTables()
		{
			// Arrange

			var knownTables = new HashSet<string>
			{
				"Artists",
				"DiscImages",
				"Discs",
				"Genres",
				"Playbacks",
				"Songs",
			};

			var binPath = AppDomain.CurrentDomain.BaseDirectory;
			var connectionString = Path.Combine(binPath, "MusicLibrary.db").ToConnectionString();

			// Act

			List<string> actualTables;
			using (var connection = new SqliteConnection(connectionString))
			{
				connection.Open();
				var schema = connection.GetSchema("Tables");
				actualTables = schema.Rows.Cast<DataRow>().Select(r => (string)r["TABLE_NAME"]).ToList();
			}

			// Assert

			var unknownTables = actualTables.Where(t => !knownTables.Contains(t)).ToList();
			CollectionAssert.IsEmpty(unknownTables, $"Following table(s) are not covered by {nameof(MusicLibraryRepository.CopyData)} method: {String.Join(", ", unknownTables)}");
		}
	}
}