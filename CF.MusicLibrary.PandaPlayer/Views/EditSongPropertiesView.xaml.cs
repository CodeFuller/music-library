﻿using System;
using System.Windows;
using System.Windows.Input;
using CF.Library.Wpf.Extensions;
using CF.MusicLibrary.PandaPlayer.ViewModels;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for EditSongPropertiesView.xaml
	/// </summary>
	public partial class EditSongPropertiesView : Window
	{
		public EditSongPropertiesView()
		{
			InitializeComponent();
		}

		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.IsValid();
		}

		private async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var vm = DataContext as EditSongPropertiesViewModel;
			if (vm == null)
			{
				throw new InvalidOperationException("ViewModel is not set");
			}

			await vm.Save();
			DialogResult = true;
			e.Handled = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}
