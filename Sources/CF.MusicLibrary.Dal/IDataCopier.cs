﻿using System.Threading;
using System.Threading.Tasks;

namespace CF.MusicLibrary.Dal
{
	public interface IDataCopier
	{
		Task CopyData(string sourceDatabaseFileName, string targetDatabaseFileName, CancellationToken cancellationToken);
	}
}
