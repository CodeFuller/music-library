﻿using System;
using System.Collections.Generic;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Services.Tagging;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace MusicLibrary.Services.Diagnostic.Inconsistencies.TagsInconsistencies
{
	internal class UnexpectedTagTypesInconsistency : BasicTagInconsistency
	{
		private readonly IReadOnlyCollection<SongTagType> tagTypes;

		public override string Description => Current($"Unexpected tag types for '{SongDisplayTitle}': [{String.Join(", ", tagTypes)}]");

		public UnexpectedTagTypesInconsistency(SongModel song, IEnumerable<SongTagType> tagTypes)
			: base(song)
		{
			this.tagTypes = tagTypes?.ToList() ?? throw new ArgumentNullException(nameof(tagTypes));
		}
	}
}
