﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CF.MusicLibrary.PandaPlayer.ViewModels;

namespace CF.MusicLibrary.PandaPlayer.Views
{
	/// <summary>
	/// Interaction logic for LoggerView.xaml
	/// </summary>
	public partial class LoggerView : UserControl
	{
		public LoggerView()
		{
			InitializeComponent();
		}

		private void View_Loaded(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as LoggerViewModel;
			if (viewModel != null)
			{
				viewModel.Messages.CollectionChanged += (s, ev) => ScrollToDataGridBottom(MessagesDataGrid);
			}
		}

		private static void ScrollToDataGridBottom(DataGrid dataGrid)
		{
			var border = VisualTreeHelper.GetChild(dataGrid, 0) as Decorator;
			var scroll = border?.Child as ScrollViewer;
			scroll?.ScrollToEnd();
		}
	}
}