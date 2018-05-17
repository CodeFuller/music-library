﻿using System;
using System.Linq;
using CF.MusicLibrary.Core.Objects;
using CF.MusicLibrary.DiscPreprocessor.AddingToLibrary;
using CF.MusicLibrary.DiscPreprocessor.MusicStorage;
using NUnit.Framework;

namespace CF.MusicLibrary.DiscPreprocessor.Tests.AddingToLibrary
{
	[TestFixture]
	public class CompilationDiscWithArtistInfoViewItemTests
	{
		[Test]
		public void ArtistSetter_ThrowsInvalidOperationException()
		{
			// Arrange

			var artist = new Artist { Name = "Some Artist" };

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithArtistInfoViewItem(discInfo, new[] { artist }, Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.Throws<InvalidOperationException>(() => target.Artist = artist);
		}

		[Test]
		public void ArtistIsEditableGetter_ReturnsFalse()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithArtistInfoViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsFalse(target.ArtistIsEditable);
		}

		[Test]
		public void ArtistIsNotFilledGetter_ReturnsFalse()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithArtistInfoViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.IsFalse(target.ArtistIsNotFilled);
		}

		[Test]
		public void DiscTypeTitleGetter_ReturnsCorrectDiscTypeTitle()
		{
			// Arrange

			var discInfo = new AddedDiscInfo(Enumerable.Empty<AddedSongInfo>())
			{
				DiscType = DsicType.CompilationDiscWithArtistInfo,
				DiscTitle = "Some Title",
			};

			var target = new CompilationDiscWithArtistInfoViewItem(discInfo, new[] { new Artist { Name = "Some Artist" } }, Enumerable.Empty<Genre>());

			// Act & Assert

			Assert.AreEqual("Compilation with Artists", target.DiscTypeTitle);
		}
	}
}