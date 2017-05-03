﻿using System.Windows;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;
using CF.MusicLibrary.AlbumPreprocessor.Views;

namespace CF.MusicLibrary.AlbumPreprocessor
{
	internal class WpfWindowService : IWindowService
	{
		public bool ShowEditAlbumsDetailsWindow(EditAlbumsDetailsViewModel viewModel)
		{
			var window = new EditAlbumsDetailsWindow(viewModel);
			return window.ShowDialog() ?? false;
		}

		public bool ShowEditSongsDetailsWindow(EditSongsDetailsViewModel viewModel)
		{
			var window = new EditSongsDetailsWindow(viewModel);
			return window.ShowDialog() ?? false;
		}

		public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
		{
			return MessageBox.Show(messageBoxText, caption, button, icon);
		}
	}
}