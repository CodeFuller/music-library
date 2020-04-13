﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CF.Library.Core.Facades;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Options;
using MusicLibrary.DiscPreprocessor.Events;
using MusicLibrary.DiscPreprocessor.Interfaces;
using MusicLibrary.DiscPreprocessor.MusicStorage;
using MusicLibrary.DiscPreprocessor.ParsingContent;
using MusicLibrary.DiscPreprocessor.ViewModels.Interfaces;
using MusicLibrary.DiscPreprocessor.ViewModels.SourceContent;
using static System.FormattableString;

namespace MusicLibrary.DiscPreprocessor.ViewModels
{
	public class EditSourceContentViewModel : ViewModelBase, IEditSourceContentViewModel
	{
		public string Name => "Edit Source Content";

		private readonly IContentCrawler contentCrawler;
		private readonly IDiscContentParser discContentParser;
		private readonly IDiscContentComparer discContentComparer;
		private readonly IWorkshopMusicStorage workshopMusicStorage;

		private readonly DiscPreprocessorSettings settings;

		public EthalonContentViewModel RawEthalonDiscs { get; }

		public DiscTreeViewModel EthalonDiscs { get; }

		public DiscTreeViewModel CurrentDiscs { get; }

		public IEnumerable<AddedDiscInfo> AddedDiscs => CurrentDiscs.Select(d => workshopMusicStorage.GetAddedDiscInfo(d.DiscDirectory, d.SongFileNames));

		public ICommand ReloadRawContentCommand { get; }

		private bool dataIsReady;

		public bool DataIsReady
		{
			get => dataIsReady;
			set => Set(ref dataIsReady, value);
		}

		public EditSourceContentViewModel(IContentCrawler contentCrawler, IDiscContentParser discContentParser, IDiscContentComparer discContentComparer,
			IWorkshopMusicStorage workshopMusicStorage, IFileSystemFacade fileSystemFacade, IOptions<DiscPreprocessorSettings> options)
		{
			if (fileSystemFacade == null)
			{
				throw new ArgumentNullException(nameof(fileSystemFacade));
			}

			this.contentCrawler = contentCrawler ?? throw new ArgumentNullException(nameof(contentCrawler));
			this.discContentParser = discContentParser ?? throw new ArgumentNullException(nameof(discContentParser));
			this.discContentComparer = discContentComparer ?? throw new ArgumentNullException(nameof(discContentComparer));
			this.workshopMusicStorage = workshopMusicStorage ?? throw new ArgumentNullException(nameof(workshopMusicStorage));
			this.settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

			EthalonDiscs = new DiscTreeViewModel();
			CurrentDiscs = new DiscTreeViewModel();

			RawEthalonDiscs = new EthalonContentViewModel(fileSystemFacade, settings.DataStoragePath);
			RawEthalonDiscs.PropertyChanged += OnRawEthalonDiscsPropertyChanged;

			ReloadRawContentCommand = new RelayCommand(ReloadRawContent);

			Messenger.Default.Register<DiscContentChangedEventArgs>(this, OnDiscContentChanged);
		}

		public void LoadDefaultContent()
		{
			RawEthalonDiscs.LoadRawEthalonDiscsContent();

			LoadCurrentDiscs();
		}

		private void OnDiscContentChanged(DiscContentChangedEventArgs message)
		{
			UpdateContentCorrectness();
		}

		public void ReloadRawContent()
		{
			var contentBuilder = new StringBuilder();
			foreach (var disc in CurrentDiscs.Discs)
			{
				contentBuilder.Append(Invariant($"# {disc.DiscDirectory}\n\n"));
			}

			RawEthalonDiscs.Content = contentBuilder.ToString();
		}

		private void OnRawEthalonDiscsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateDiscs(EthalonDiscs, discContentParser.Parse(RawEthalonDiscs.Content));
		}

		public void LoadCurrentDiscs()
		{
			var discs = contentCrawler.LoadDiscs(settings.WorkshopStoragePath).ToList();

			UpdateDiscs(CurrentDiscs, discs);
		}

		private void UpdateDiscs(DiscTreeViewModel discs, IEnumerable<DiscContent> newDiscs)
		{
			discs.SetDiscs(newDiscs);
			UpdateContentCorrectness();
		}

		private void SetContentCorrectness()
		{
			discContentComparer.SetDiscsCorrectness(EthalonDiscs, CurrentDiscs);
		}

		private void UpdateContentCorrectness()
		{
			SetContentCorrectness();
			DataIsReady = !EthalonDiscs.ContentIsIncorrect && !CurrentDiscs.ContentIsIncorrect;
		}
	}
}