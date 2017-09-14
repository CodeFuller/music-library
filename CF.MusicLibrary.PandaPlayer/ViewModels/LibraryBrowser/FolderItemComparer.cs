﻿using System;
using System.Collections.Generic;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser
{
	public class FolderItemComparer : IEqualityComparer<FolderExplorerItem>
	{
		public bool Equals(FolderExplorerItem x, FolderExplorerItem y)
		{
			if (x == null || y == null)
			{
				return x == null && y == null;
			}

			return x.Uri == y.Uri;
		}

		public int GetHashCode(FolderExplorerItem obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			return obj.GetHashCode();
		}
	}
}