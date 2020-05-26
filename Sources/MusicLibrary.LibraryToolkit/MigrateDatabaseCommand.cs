using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.LibraryToolkit.Interfaces;
using MusicLibrary.Services.Interfaces;
using static System.FormattableString;

namespace MusicLibrary.LibraryToolkit
{
	public class MigrateDatabaseCommand : IMigrateDatabaseCommand
	{
		internal class FolderData
		{
			private readonly List<FolderData> subfolders = new List<FolderData>();

			private readonly List<DiscModel> discs = new List<DiscModel>();

			public int Id { get; set; }

			public string Name { get; }

			public int? ParentFolderId { get; set; }

			public DateTimeOffset? DeleteTime { get; set; }

			public IReadOnlyCollection<FolderData> Subfolders => subfolders;

			public IReadOnlyCollection<DiscModel> Discs => discs;

			public FolderData(string name)
			{
				Name = name;
			}

			public void AddSubfolder(FolderData subfolder)
			{
				subfolders.Add(subfolder);
			}

			public void AddDisc(DiscModel disc)
			{
				discs.Add(disc);
			}
		}

		private const char SegmentsSeparator = '/';

		private static Uri RootUri => new Uri("/", UriKind.Relative);

		private readonly IDiscsService discsService;

		private readonly ILogger<MigrateDatabaseCommand> logger;

		public MigrateDatabaseCommand(IDiscsService discsService, ILogger<MigrateDatabaseCommand> logger)
		{
			this.discsService = discsService ?? throw new ArgumentNullException(nameof(discsService));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Execute(string targetDatabaseFileName, CancellationToken cancellationToken)
		{
			var rootFolder = await CreateFoldersTree(cancellationToken);

			AssignFolderIds(rootFolder);

			FillDeleteTime(rootFolder);

			var scriptBuilder = new StringBuilder();

			scriptBuilder.AppendLine("ALTER TABLE [Discs] ADD COLUMN [TreeTitle] ntext NOT NULL DEFAULT '<DEFAULT>';");
			scriptBuilder.AppendLine("ALTER TABLE [Discs] ADD COLUMN [Folder_Id] INTEGER NOT NULL DEFAULT 0;");
			scriptBuilder.AppendLine("ALTER TABLE [Songs] ADD COLUMN [TreeTitle] ntext NOT NULL DEFAULT '<DEFAULT>';");
			scriptBuilder.AppendLine("ALTER TABLE [DiscImages] ADD COLUMN [TreeTitle] ntext NOT NULL DEFAULT '<DEFAULT>';");
			scriptBuilder.AppendLine();

			scriptBuilder.AppendLine("CREATE TABLE [Folders] (");
			scriptBuilder.AppendLine("  [Id] INTEGER NOT NULL,");
			scriptBuilder.AppendLine("  [ParentFolder_Id] INTEGER NULL,");
			scriptBuilder.AppendLine("  [Name] ntext NOT NULL,");
			scriptBuilder.AppendLine("  [DeleteDate] datetime NULL,");
			scriptBuilder.AppendLine("  CONSTRAINT [sqlite_master_PK_Folders] PRIMARY KEY ([Id]),");
			scriptBuilder.AppendLine("  FOREIGN KEY ([ParentFolder_Id]) REFERENCES [Folders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION");
			scriptBuilder.AppendLine(");");
			scriptBuilder.AppendLine();

			var folders = TraverseBreadthFirst(rootFolder)
				.OrderBy(f => f.Id)
				.ToList();

			foreach (var folder in folders)
			{
				var parentFolderId = folder.ParentFolderId == null ? "NULL" : Invariant($"{folder.ParentFolderId}");
				var deleteDate = folder.DeleteTime == null ? "NULL" : Invariant($"'{folder.DeleteTime.Value:O}'");
				scriptBuilder.AppendLine(Invariant($"INSERT INTO [Folders] ([Id], [ParentFolder_Id], [Name], [DeleteDate]) VALUES({folder.Id}, {parentFolderId}, '{EscapeSqlStringLiteral(folder.Name)}', {deleteDate});"));
			}

			scriptBuilder.AppendLine();

			var discsAndFolders = folders
				.SelectMany(f => f.Discs.Select(d => (Folder: f, Disc: d)))
				.OrderBy(p => p.Disc.Id.ToInt32())
				.ToList();

			foreach (var disc in discsAndFolders.Select(p => p.Disc))
			{
				scriptBuilder.AppendLine(Invariant($"UPDATE [Discs] SET [TreeTitle] = '{EscapeSqlStringLiteral(disc.TreeTitle)}' WHERE [Id] = {disc.Id.Value};"));
			}

			scriptBuilder.AppendLine();

			foreach (var pair in discsAndFolders)
			{
				scriptBuilder.AppendLine(Invariant($"UPDATE [Discs] SET [Folder_Id] = {pair.Folder.Id} WHERE [Id] = {pair.Disc.Id.Value};"));
			}

			scriptBuilder.AppendLine();

			foreach (var song in discsAndFolders.Select(p => p.Disc).SelectMany(d => d.Songs).OrderBy(s => s.Id.ToInt32()))
			{
				scriptBuilder.AppendLine(Invariant($"UPDATE [Songs] SET [TreeTitle] = '{EscapeSqlStringLiteral(song.TreeTitle)}' WHERE [Id] = {song.Id.Value};"));
			}

			scriptBuilder.AppendLine();

			foreach (var image in discsAndFolders.Select(p => p.Disc).SelectMany(d => d.Images).OrderBy(s => s.Id.ToInt32()))
			{
				scriptBuilder.AppendLine(Invariant($"UPDATE [DiscImages] SET [TreeTitle] = '{EscapeSqlStringLiteral(image.TreeTitle)}' WHERE [Id] = {image.Id.Value};"));
			}

			await System.IO.File.WriteAllTextAsync(targetDatabaseFileName, scriptBuilder.ToString(), cancellationToken);
		}

		private async Task<FolderData> CreateFoldersTree(CancellationToken cancellationToken)
		{
			var discs = await discsService.GetAllDiscs(cancellationToken);
			discs = discs.OrderBy(d => d.Folder.Id.Value)
				.ThenBy(d => d.TreeTitle)
				.ToList();

			var rootFolder = new FolderData("<ROOT>");

			var folders = new Dictionary<Uri, FolderData>
			{
				[RootUri] = rootFolder,
			};

			foreach (var disc in discs.OrderBy(d => d.Folder.Id.Value))
			{
				var folderUri = disc.Folder.Id.ToUri();
				var parentFolders = SplitInternalUriToSegments(folderUri);

				var parentFolder = rootFolder;
				for (var i = 0; i < parentFolders.Count; ++i)
				{
					var currentFolderUri = BuildInternalUriFromSegments(parentFolders.Take(i + 1));

					if (!folders.TryGetValue(currentFolderUri, out var currentFolder))
					{
						currentFolder = new FolderData(parentFolders[i]);
						parentFolder.AddSubfolder(currentFolder);
						folders.Add(currentFolderUri, currentFolder);
					}

					parentFolder = currentFolder;
				}

				parentFolder.AddDisc(disc);
			}

			return rootFolder;
		}

		private static string EscapeSqlStringLiteral(string literal)
		{
			return literal.Replace("'", "''", StringComparison.Ordinal);
		}

		private static void AssignFolderIds(FolderData folder)
		{
			var nextFolderId = 1;
			foreach (var currentFolder in TraverseBreadthFirst(folder))
			{
				currentFolder.Id = nextFolderId++;
				foreach (var subfolder in currentFolder.Subfolders)
				{
					subfolder.ParentFolderId = currentFolder.Id;
				}
			}
		}

		private static IEnumerable<FolderData> TraverseBreadthFirst(FolderData folder)
		{
			var queue = new Queue<FolderData>();
			queue.Enqueue(folder);

			while (queue.TryDequeue(out var currentFolder))
			{
				yield return currentFolder;

				foreach (var subfolder in currentFolder.Subfolders)
				{
					queue.Enqueue(subfolder);
				}
			}
		}

		private static void FillDeleteTime(FolderData folder)
		{
			foreach (var subfolder in folder.Subfolders)
			{
				FillDeleteTime(subfolder);
			}

			var subfoldersDeleteTime = SelectDeleteTime(folder.Subfolders.Select(f => f.DeleteTime));
			var discsDeleteTime = SelectDeleteTime(folder.Discs.SelectMany(d => d.Songs).Select(s => s.DeleteDate));

			var deleteTime = GetLatestDeleteTime(subfoldersDeleteTime, discsDeleteTime);

			if (deleteTime == DateTimeOffset.MinValue)
			{
				throw new InvalidOperationException($"Folder '{folder.Name}' does not contain neither subfolders nor discs");
			}

			folder.DeleteTime = deleteTime;
		}

		private static DateTimeOffset? SelectDeleteTime(IEnumerable<DateTimeOffset?> dates)
		{
			DateTimeOffset? maxDate = DateTimeOffset.MinValue;

			foreach (var date in dates)
			{
				if (date == null)
				{
					return null;
				}

				maxDate = GetLatestDeleteTime(maxDate, date);
			}

			return maxDate;
		}

		private static DateTimeOffset? GetLatestDeleteTime(DateTimeOffset? dt1, DateTimeOffset? dt2)
		{
			if (dt1 == null || dt2 == null)
			{
				return null;
			}

			return dt1 > dt2 ? dt1 : dt2;
		}

		private static IList<string> SplitInternalUriToSegments(Uri internalUri)
		{
			return internalUri.OriginalString.Split(SegmentsSeparator).Skip(1).ToArray();
		}

		private static Uri BuildInternalUriFromSegments(IEnumerable<string> segments)
		{
			return new Uri($"{RootUri.OriginalString}{String.Join(SegmentsSeparator, segments)}", UriKind.Relative);
		}
	}
}
