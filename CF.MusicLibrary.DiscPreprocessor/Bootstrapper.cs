﻿using CF.Library.Core.Configuration;
using CF.Library.Core.Facades;
using CF.Library.Core.Interfaces;
using CF.Library.Unity;
using CF.MusicLibrary.Common.DiscArt;
using CF.MusicLibrary.Core.Interfaces;
using CF.MusicLibrary.Core.Media;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.Dal;
using CF.MusicLibrary.DiscPreprocessor.Interfaces;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using CF.MusicLibrary.DiscPreprocessor.ParsingContent;
using CF.MusicLibrary.DiscPreprocessor.ParsingSong;
using CF.MusicLibrary.DiscPreprocessor.ViewModels;
using CF.MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using CF.MusicLibrary.Library;
using CF.MusicLibrary.Local;
using CF.MusicLibrary.Tagger;
using Microsoft.Practices.Unity;

namespace CF.MusicLibrary.DiscPreprocessor
{
	internal class Bootstrapper : UnityBootstrapper<ApplicationViewModel>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It's ok for Composition Root")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "LifetimeManager is disposed by DI Container.")]
		protected override void RegisterDependencies()
		{
			string workshopDirectory = AppSettings.GetRequiredValue<string>("WorkshopDirectory");
			string localStorageRoot = AppSettings.GetRequiredValue<string>("LocalStorageRoot");
			bool deleteSourceContentAfterAdding = AppSettings.GetRequiredValue<bool>("DeleteSourceContentAfterAdding");

			DIContainer.RegisterType<IMusicLibraryRepository, MusicLibraryRepositoryEF>(new InjectionConstructor());
			DIContainer.RegisterType<IMusicLibraryStorage, FileSystemMusicStorage>(
				new InjectionConstructor(typeof(IFileSystemFacade), typeof(ISongTagger), typeof(IDiscArtFileStorage), localStorageRoot));
			DIContainer.RegisterType<IMusicLibrary, RepositoryAndStorageMusicLibrary>();
			DIContainer.RegisterType<IFileSystemFacade, FileSystemFacade>();
			DIContainer.RegisterType<IWorkshopMusicStorage, WorkshopMusicStorage>(new InjectionConstructor(typeof(IFileSystemFacade), workshopDirectory));

			DIContainer.RegisterInstance(new DiscLibrary(async () =>
			{
				var library = DIContainer.Resolve<IMusicLibrary>();
				return await library.LoadDiscs();
			}));

			DIContainer.RegisterType<IEthalonSongParser, EthalonSongParser>();
			DIContainer.RegisterType<IEthalonDiscParser, EthalonDiscParser>();
			DIContainer.RegisterType<IDiscContentParser, DiscContentParser>();
			DIContainer.RegisterType<IInputContentSplitter, InputContentSplitter>();
			DIContainer.RegisterType<IDiscContentComparer, DiscContentComparer>();
			DIContainer.RegisterType<ISongTagger, SongTagger>();
			DIContainer.RegisterType<ISongMediaInfoProvider, SongMediaInfoProvider>();
			DIContainer.RegisterType<ILibraryStructurer, MyLibraryStructurer>();
			DIContainer.RegisterType<IDiscArtValidator, DiscArtValidator>();
			DIContainer.RegisterType<IDiscArtImageFile, DiscArtImageFile>();
			DIContainer.RegisterType<IObjectFactory<IDiscArtImageFile>, UnityBasedObjectFactory<IDiscArtImageFile>>(new InjectionConstructor(DIContainer));
			DIContainer.RegisterType<IDiscArtFileStorage, DiscArtFileStorage>();
			DIContainer.RegisterType<IDiscCrawler, DiscCrawler>();
			DIContainer.RegisterType<ISongFileFilter, SongFileFilter>();
			DIContainer.RegisterType<IImageFacade, ImageFacade>();

			DIContainer.RegisterType<IEditSourceContentViewModel, EditSourceContentViewModel>();
			DIContainer.RegisterType<IEditDiscsDetailsViewModel, EditDiscsDetailsViewModel>();
			DIContainer.RegisterType<IEditSongsDetailsViewModel, EditSongsDetailsViewModel>();
			DIContainer.RegisterType<IAddToLibraryViewModel, AddToLibraryViewModel>(new InjectionConstructor(
				typeof(IMusicLibrary), typeof(ISongMediaInfoProvider), typeof(IWorkshopMusicStorage), deleteSourceContentAfterAdding));
			DIContainer.RegisterType<ApplicationViewModel>();
		}
	}
}
