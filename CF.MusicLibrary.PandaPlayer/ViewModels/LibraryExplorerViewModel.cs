﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CF.Library.Wpf;
using CF.MusicLibrary.BL.Objects;
using CF.MusicLibrary.PandaPlayer.ContentUpdate;
using CF.MusicLibrary.PandaPlayer.Events;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.ViewModels.LibraryBrowser;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace CF.MusicLibrary.PandaPlayer.ViewModels
{
	public class LibraryExplorerViewModel : ViewModelBase, ILibraryExplorerViewModel
	{
		private readonly ILibraryBrowser libraryBrowser;

		private readonly ILibraryContentUpdater libraryContentUpdater;

		private readonly IViewNavigator viewNavigator;

		private FolderExplorerItem ParentFolder { get; set; }

		private ObservableCollection<FolderExplorerItem> items;
		public ObservableCollection<FolderExplorerItem> Items
		{
			get { return items; }
			private set { Set(ref items, value); }
		}

		private FolderExplorerItem CurrentFolder { get; set; }

		private FolderExplorerItem selectedItem;
		public FolderExplorerItem SelectedItem
		{
			get { return selectedItem; }
			set { Set(ref selectedItem, value); }
		}

		public ICommand ChangeFolderCommand { get; }
		public ICommand PlayDiscCommand { get; }
		public ICommand DeleteDiscCommand { get; }
		public ICommand JumpToFirstItemCommand { get; }
		public ICommand JumpToLastItemCommand { get; }
		public ICommand EditDiscPropertiesCommand { get; }

		public LibraryExplorerViewModel(ILibraryBrowser libraryBrowser, ILibraryContentUpdater libraryContentUpdater, IViewNavigator viewNavigator)
		{
			if (libraryBrowser == null)
			{
				throw new ArgumentNullException(nameof(libraryBrowser));
			}
			if (libraryContentUpdater == null)
			{
				throw new ArgumentNullException(nameof(libraryContentUpdater));
			}
			if (viewNavigator == null)
			{
				throw new ArgumentNullException(nameof(viewNavigator));
			}

			this.libraryBrowser = libraryBrowser;
			this.libraryContentUpdater = libraryContentUpdater;
			this.viewNavigator = viewNavigator;

			ChangeFolderCommand = new RelayCommand(ChangeFolder);
			PlayDiscCommand = new RelayCommand(PlayDisc);
			DeleteDiscCommand = new AsyncRelayCommand(DeleteDisc);
			JumpToFirstItemCommand = new RelayCommand(() => SelectedItem = Items.FirstOrDefault());
			JumpToLastItemCommand = new RelayCommand(() => SelectedItem = Items.LastOrDefault());
			EditDiscPropertiesCommand = new RelayCommand(EditDiscProperties);
		}

		public void Load()
		{
			ChangeFolder(FolderExplorerItem.Root);
		}

		public void SwitchToDisc(Disc disc)
		{
			var discItem = libraryBrowser.GetDiscItem(disc);
			if (discItem == null)
			{
				return;
			}

			var discParentFolder = libraryBrowser.GetParentFolder(discItem);
			if (discParentFolder == null)
			{
				return;
			}

			ChangeFolder(discParentFolder);
			SelectedItem = Items.OfType<DiscExplorerItem>().SingleOrDefault(it => it.Disc.Id == disc.Id);
		}

		private void ChangeFolder()
		{
			ChangeFolder(SelectedItem);
		}

		private void PlayDisc()
		{
			var discItem = SelectedItem as DiscExplorerItem;
			if (discItem == null)
			{
				return;
			}

			Messenger.Default.Send(new PlayDiscEventArgs(discItem.Disc));
		}

		private async Task DeleteDisc()
		{
			var discItem = SelectedItem as DiscExplorerItem;
			if (discItem == null)
			{
				return;
			}

			await libraryContentUpdater.DeleteDisc(discItem.Disc);

			libraryBrowser.RemoveDiscItem(discItem);
			Items.Remove(discItem);

			//	If only '..' item remains
			if (Items.Count == 1)
			{
				FolderExplorerItem currFolder = discItem;
				do
				{
					currFolder = libraryBrowser.GetParentFolder(currFolder);
				}
				while (currFolder != null && !libraryBrowser.GetChildFolderItems(currFolder).Any()) ;

				if (currFolder != null)
				{
					ChangeFolder(currFolder);
				}
			}
		}

		private void EditDiscProperties()
		{
			var discItem = SelectedItem as DiscExplorerItem;
			if (discItem != null)
			{
				viewNavigator.ShowDiscPropertiesView(discItem.Disc);
			}
		}

		private void ChangeFolder(FolderExplorerItem newFolder)
		{
			if (newFolder == null)
			{
				return;
			}

			var childFolderItems = libraryBrowser.GetChildFolderItems(newFolder).ToList();
			if (!childFolderItems.Any())
			{
				return;
			}

			//	Remember current directory if we're moving up
			FolderExplorerItem prevFolder = null;
			if (newFolder.IsParentItem)
			{
				prevFolder = CurrentFolder;
			}
			CurrentFolder = newFolder;

			//	Getting a parent of new folder
			ParentFolder = libraryBrowser.GetParentFolder(newFolder);
			if (ParentFolder != null)
			{
				ParentFolder.IsParentItem = true;
			}

			//	Building new items list
			Items = new ObservableCollection<FolderExplorerItem>(childFolderItems);
			if (ParentFolder != null)
			{
				Items.Insert(0, ParentFolder);
			}

			//	Setting selected item
			FolderExplorerItem newSelectedItem = null;
			if (prevFolder != null)
			{
				newSelectedItem = Items.FirstOrDefault(f => new FolderItemComparer().Equals(f, prevFolder));
			}
			SelectedItem = newSelectedItem ?? Items.FirstOrDefault();
		}
	}
}
