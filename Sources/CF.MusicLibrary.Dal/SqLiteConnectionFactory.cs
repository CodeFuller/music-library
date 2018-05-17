﻿using System;
using System.Data.Common;
using System.Data.SQLite;
using Microsoft.Extensions.Options;

namespace CF.MusicLibrary.Dal
{
	public class SqLiteConnectionFactory : IConfiguredDbConnectionFactory
	{
		private readonly SqLiteConnectionSettings settings;

		public SqLiteConnectionFactory(IOptions<SqLiteConnectionSettings> options)
		{
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public DbConnection CreateConnection()
		{
			SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
			{
				DataSource = settings.DataSource,
				ForeignKeys = true
			};

			return new SQLiteConnection(builder.ConnectionString);
		}
	}
}