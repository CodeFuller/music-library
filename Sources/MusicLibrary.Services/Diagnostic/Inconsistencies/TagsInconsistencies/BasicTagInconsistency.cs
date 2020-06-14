﻿using System;
using MusicLibrary.Core.Models;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.TagsInconsistencies
{
	internal abstract class BasicTagInconsistency : LibraryInconsistency
	{
		protected SongModel Song { get; }

		protected string SongDisplayTitle => Current($"{Song.Disc.Folder.Name}/{Song.Disc.TreeTitle}/{Song.TreeTitle}");

		public override InconsistencySeverity Severity => InconsistencySeverity.Medium;

		protected BasicTagInconsistency(SongModel song)
		{
			Song = song ?? throw new ArgumentNullException(nameof(song));
		}
	}
}
