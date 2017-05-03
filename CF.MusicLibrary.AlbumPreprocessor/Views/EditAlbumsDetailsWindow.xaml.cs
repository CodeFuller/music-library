﻿using System.Windows;
using CF.MusicLibrary.AlbumPreprocessor.ViewModels;

namespace CF.MusicLibrary.AlbumPreprocessor.Views
{
	/// <summary>
	/// Interaction logic for EditAlbumsDetailsWindow.xaml
	/// </summary>
	public partial class EditAlbumsDetailsWindow : Window
	{
		public EditAlbumsDetailsWindow(EditAlbumsDetailsViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
