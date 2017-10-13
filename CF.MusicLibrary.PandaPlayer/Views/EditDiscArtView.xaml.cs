﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CF.MusicLibrary.PandaPlayer.ViewModels.Interfaces;
using CF.MusicLibrary.PandaPlayer.Views.ClipboardAccess;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for EditDiscArtView.xaml
	/// </summary>
	public partial class EditDiscArtView : Window
	{
		private IEditDiscArtViewModel ViewModel => DataContext.GetViewModel<IEditDiscArtViewModel>();

		private readonly IClipboardChangeTracker clipboardChangeTracker = new ClipboardChangeTracker();

		private readonly IClipboardDataProvider clipboardDataProvider = new ClipboardDataProvider();

		public EditDiscArtView()
		{
			InitializeComponent();
		}

		private void ClipboardChangeTrackerOnClipboardContentChanged(object sender, ClipboardContentChangedEventArgs clipboardContentChangedEventArgs)
		{
			string textData = clipboardDataProvider.GetTextData();
			if (textData != null)
			{
				Uri imageUri;
				if (Uri.TryCreate(textData, UriKind.Absolute, out imageUri))
				{
					ViewModel.SetImage(imageUri);
				}
				return;
			}

			BitmapFrame imageData = clipboardDataProvider.GetImageData();
			if (imageData != null)
			{
				var encoder = new JpegBitmapEncoder();
				encoder.Frames.Add(imageData);

				using (var memoryStream = new MemoryStream())
				{
					encoder.Save(memoryStream);
					ViewModel.SetImage(memoryStream.ToArray());
				}
			}
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			await ViewModel.Save();
			DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Window_OnLoaded(object sender, RoutedEventArgs e)
		{
			clipboardChangeTracker.ClipboardContentChanged += ClipboardChangeTrackerOnClipboardContentChanged;
			clipboardChangeTracker.StartTracking();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			clipboardChangeTracker.ClipboardContentChanged -= ClipboardChangeTrackerOnClipboardContentChanged;
			clipboardChangeTracker.StopTracking();
			ViewModel.Unload();

			base.OnClosing(e);
		}
	}
}