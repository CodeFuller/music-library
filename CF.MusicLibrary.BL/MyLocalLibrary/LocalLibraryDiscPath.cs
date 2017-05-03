﻿using System;
using CF.MusicLibrary.BL.Objects;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.BL.MyLocalLibrary
{
	internal class LocalLibraryDiscPath
	{
		private readonly DiscPathParts pathParts;

		public LocalLibraryDiscPath(Disc disc) :
			this(disc.Uri)
		{
		}

		public LocalLibraryDiscPath(Uri discUri)
		{
			pathParts = new DiscPathParts(discUri);

			if (pathParts.Count < 2)
			{
				throw new InvalidOperationException(Current($"Could not extract category and nested directory from disc path {discUri.LocalPath}"));
			}
		}

		public string Category => pathParts[0];

		public string NestedDirectory => pathParts[1];
	}
}