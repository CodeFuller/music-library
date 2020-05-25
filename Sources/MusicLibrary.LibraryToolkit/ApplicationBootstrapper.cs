using System;
using CF.Library.Bootstrap;
using CF.Library.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.LibraryToolkit.Interfaces;
using MusicLibrary.LibraryToolkit.Seeders;
using MusicLibrary.LibraryToolkit.Settings;
using MusicLibrary.Services.Extensions;
using MusicLibraryApi.Client;

namespace MusicLibrary.LibraryToolkit
{
	public class ApplicationBootstrapper : DiApplicationBootstrapper<IApplicationLogic>
	{
		protected override void RegisterServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddLocalDbDal(settings => configuration.Bind("localDb:dataStorage", settings));
			services.AddMusicLibraryDbContext(configuration.GetConnectionString("musicLibraryDb"));

			services.AddMusicLibraryServices();

			services.Configure<DiscsSeederSettings>(options => configuration.Bind("seeders:discsSeeder", options));
			services.Configure<SongsSeederSettings>(options => configuration.Bind("seeders:songsSeeder", options));

			services.AddTransient<IApplicationLogic, ApplicationLogic>();
			services.AddTransient<IMigrateDatabaseCommand, MigrateDatabaseCommand>();
			services.AddTransient<ISeedApiDatabaseCommand, SeedApiDatabaseCommand>();

			services.AddTransient<IGenresSeeder, GenresSeeder>();
			services.AddTransient<IArtistsSeeder, ArtistsSeeder>();
			services.AddTransient<IFoldersSeeder, FoldersSeeder>();
			services.AddTransient<IDiscsSeeder, DiscsSeeder>();
			services.AddTransient<ISongsSeeder, SongsSeeder>();
			services.AddTransient<IPlaybacksSeeder, PlaybacksSeeder>();

			services.AddMusicLibraryApiClient(settings => configuration.Bind("musicLibraryApiConnection", settings));
		}

		protected override void BootstrapConfiguration(IConfigurationBuilder configurationBuilder, string[] commandLineArgs)
		{
			// We do not load configuration from command line, so that it does not conflict with utility commands.
			base.BootstrapConfiguration(configurationBuilder, Array.Empty<string>());
		}

		protected override void BootstrapLogging(ILoggerFactory loggerFactory, IConfiguration configuration)
		{
			loggerFactory.LoadLoggingConfiguration(configuration);
		}
	}
}
