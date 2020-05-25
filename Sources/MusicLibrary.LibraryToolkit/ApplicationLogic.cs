using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CF.Library.Bootstrap;
using CF.Library.Core.Exceptions;
using CF.Library.Core.Facades;
using MusicLibrary.LibraryToolkit.Interfaces;
using NDesk.Options;
using static System.FormattableString;

namespace MusicLibrary.LibraryToolkit
{
	public class ApplicationLogic : IApplicationLogic
	{
		private readonly IMigrateDatabaseCommand migrateDatabaseCommand;
		private readonly ISeedApiDatabaseCommand seedApiDatabaseCommand;
		private readonly IFileSystemFacade fileSystemFacade;

		public ApplicationLogic(IMigrateDatabaseCommand migrateDatabaseCommand, ISeedApiDatabaseCommand seedApiDatabaseCommand, IFileSystemFacade fileSystemFacade)
		{
			this.migrateDatabaseCommand = migrateDatabaseCommand ?? throw new ArgumentNullException(nameof(migrateDatabaseCommand));
			this.seedApiDatabaseCommand = seedApiDatabaseCommand ?? throw new ArgumentNullException(nameof(seedApiDatabaseCommand));
			this.fileSystemFacade = fileSystemFacade ?? throw new ArgumentNullException(nameof(fileSystemFacade));
		}

		public async Task<int> Run(string[] args, CancellationToken cancellationToken)
		{
			var command = LaunchCommand.ShowHelp;

			var optionSet = new OptionSet
			{
				{ "migrate-database", s => command = LaunchCommand.MigrateDatabase },
				{ "seed-api-database", s => command = LaunchCommand.SeedApiDatabase },
			};
			var restArgs = optionSet.Parse(args);

			switch (command)
			{
				case LaunchCommand.ShowHelp:
					ShowHelp();
					return 1;

				case LaunchCommand.MigrateDatabase:
					if (restArgs.Count != 1)
					{
						ShowHelp();
						return 1;
					}

					await migrateDatabaseCommand.Execute(restArgs[0], cancellationToken);
					break;

				case LaunchCommand.SeedApiDatabase:
					if (restArgs.Any())
					{
						ShowHelp();
						return 1;
					}

					await seedApiDatabaseCommand.Execute(cancellationToken);
					break;

				default:
					throw new UnexpectedEnumValueException(command);
			}

			return 0;
		}

		private void ShowHelp()
		{
			Console.Error.WriteLine();
			Console.Error.WriteLine(Invariant($"Usage: {Path.GetFileName(fileSystemFacade.GetProcessExecutableFileName())} <command> [command options]"));
			Console.Error.WriteLine("Supported commands:");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --migrate-database   <target db file>");
			Console.Error.WriteLine("      Creates database schema by included 'MusicLibrary.sql' and copies data from source database.");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  --seed-api-database");
			Console.Error.WriteLine("      Copies the data to Music Library API.");
			Console.Error.WriteLine();
		}
	}
}
