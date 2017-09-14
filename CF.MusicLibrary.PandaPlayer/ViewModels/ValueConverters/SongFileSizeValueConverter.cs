﻿using System;
using System.Globalization;
using System.Windows.Data;
using static CF.Library.Core.Extensions.FormattableStringExtensions;

namespace CF.MusicLibrary.PandaPlayer.ViewModels.ValueConverters
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Class is used from XAML")]
	internal class SongFileSizeValueConverter : IValueConverter
	{
		private const int KiloMultiplier = 1024;
		private const int KbValue = 1 * KiloMultiplier;
		private const int MbValue = 1 * KiloMultiplier * KbValue;
		private const int GbValue = 1 * KiloMultiplier * MbValue;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Int32) || targetType != typeof(String))
			{
				return null;
			}

			float size = (int)value;

			if (size < KbValue)
			{
				return Current($"{size} B");
			}

			if (size < MbValue)
			{
				return Current($"{size/KbValue:F1} KB");
			}

			if (size < GbValue)
			{
				return Current($"{size/MbValue:F1} MB");
			}

			return Current($"{size/GbValue:F1} GB");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}