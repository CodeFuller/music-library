﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Services.IntegrationTests.Data;
using MusicLibrary.Services.IntegrationTests.Extensions;
using MusicLibrary.Services.Interfaces;
using MusicLibrary.Services.Tagging;

namespace MusicLibrary.Services.IntegrationTests
{
	[TestClass]
	public class DiscsServiceTests : BasicServiceTests<IDiscsService>
	{
		[TestMethod]
		public async Task CreateDisc_ForDiscWithAllOptionalPropertiesFilled_CreatesDiscSuccessfully()
		{
			// Arrange

			var newDisc = new DiscModel
			{
				Folder = await GetFolder(ReferenceData.ArtistFolderId),
				Year = 2021,
				Title = "Some New Disc (CD 1)",
				TreeTitle = "2021 - Some New Disc (CD 1)",
				AlbumTitle = "Some New Disc",

				// Songs are not added by CreateDisc method.
				// However we fill songs list so that CreateDisc is tested in more real-life conditions.
				AllSongs = new[]
				{
					new SongModel
					{
						Id = ReferenceData.NextSongId,
						Title = "Some New Song",
						TreeTitle = "01 - Some New Song.mp3",
						Duration = new TimeSpan(0, 3, 12),
						BitRate = 12345,
						Size = 67890,
						Checksum = 54321,
					},
				},
			};

			newDisc.AllSongs.Single().Disc = newDisc;

			var target = CreateTestTarget();

			// Act

			await target.CreateDisc(newDisc, CancellationToken.None);

			// Assert

			newDisc.Id.Should().Be(ReferenceData.NextDiscId);

			var referenceData = GetReferenceData();
			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithNullValues,
				referenceData.DeletedDisc,
				new DiscModel
				{
					Id = ReferenceData.NextDiscId,
					Folder = referenceData.ArtistFolder,
					Year = 2021,
					Title = "Some New Disc (CD 1)",
					TreeTitle = "2021 - Some New Disc (CD 1)",
					AlbumTitle = "Some New Disc",
					AllSongs = new List<SongModel>(),
				},
			};

			var allDiscs = await target.GetAllDiscs(CancellationToken.None);
			allDiscs.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedDiscs, x => x.IgnoringCyclicReferences());

			var discDirectoryPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2021 - Some New Disc (CD 1)");
			Directory.Exists(discDirectoryPath).Should().BeTrue();
		}

		[TestMethod]
		public async Task CreateDisc_ForDiscWithAllOptionalPropertiesNotFilled_CreatesDiscSuccessfully()
		{
			// Arrange

			var newDisc = new DiscModel
			{
				Folder = await GetFolder(ReferenceData.ArtistFolderId),
				Title = "Some New Disc (CD 1)",
				TreeTitle = "2021 - Some New Disc (CD 1)",

				// Songs are not added by CreateDisc method.
				// However we fill songs list so that CreateDisc is tested in more real-life conditions.
				AllSongs = new[]
				{
					new SongModel
					{
						Id = ReferenceData.NextSongId,
						Title = "Some New Song",
						TreeTitle = "01 - Some New Song.mp3",
						Duration = new TimeSpan(0, 3, 12),
						BitRate = 12345,
						Size = 67890,
						Checksum = 54321,
					},
				},
			};

			newDisc.AllSongs.Single().Disc = newDisc;

			var target = CreateTestTarget();

			// Act

			await target.CreateDisc(newDisc, CancellationToken.None);

			// Assert

			newDisc.Id.Should().Be(ReferenceData.NextDiscId);

			var referenceData = GetReferenceData();
			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithNullValues,
				referenceData.DeletedDisc,
				new DiscModel
				{
					Id = ReferenceData.NextDiscId,
					Folder = referenceData.ArtistFolder,
					Title = "Some New Disc (CD 1)",
					TreeTitle = "2021 - Some New Disc (CD 1)",
					AllSongs = new List<SongModel>(),
				},
			};

			var allDiscs = await target.GetAllDiscs(CancellationToken.None);
			allDiscs.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedDiscs, x => x.IgnoringCyclicReferences());

			var discDirectoryPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2021 - Some New Disc (CD 1)");
			Directory.Exists(discDirectoryPath).Should().BeTrue();
		}

		[TestMethod]
		public async Task CreateDisc_ForFirstDiscInFolder_CreatesDiscSuccessfully()
		{
			// Arrange

			var newDisc = new DiscModel
			{
				Folder = await GetFolder(ReferenceData.EmptyFolderId),
				Title = "Some New Disc (CD 1)",
				TreeTitle = "2021 - Some New Disc (CD 1)",
			};

			var target = CreateTestTarget();

			// Act

			await target.CreateDisc(newDisc, CancellationToken.None);

			// Assert

			newDisc.Id.Should().Be(ReferenceData.NextDiscId);

			var discDirectoryPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "Empty Folder", "2021 - Some New Disc (CD 1)");
			Directory.Exists(discDirectoryPath).Should().BeTrue();
		}

		[TestMethod]
		public async Task GetAllDiscs_ReturnsDiscsCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			// Act

			var discs = await target.GetAllDiscs(CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscs = new[]
			{
				referenceData.NormalDisc,
				referenceData.DiscWithNullValues,
				referenceData.DeletedDisc,
			};

			discs.OrderBy(x => x.Id.ToInt32()).Should().BeEquivalentTo(expectedDiscs, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task UpdateDisc_IfTreeTitleWasChanged_UpdatesDiscCorrectly()
		{
			// Arrange

			var oldDiscDirectoryPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)");
			Directory.Exists(oldDiscDirectoryPath).Should().BeTrue();

			var target = CreateTestTarget();

			var disc = await GetDisc(ReferenceData.NormalDiscId, target);

			// Act

			disc.TreeTitle = "New Tree Title";

			await target.UpdateDisc(disc, CancellationToken.None);

			// Assert

			Directory.Exists(oldDiscDirectoryPath).Should().BeFalse();

			var newDiscDirectoryPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "New Tree Title");
			Directory.Exists(newDiscDirectoryPath).Should().BeTrue();

			var expectedSongContentUris = new[]
			{
				"Foreign/Guano Apes/New Tree Title/01 - Break The Line.mp3".ToContentUri(LibraryStorageRoot),
				"Foreign/Guano Apes/New Tree Title/Song With Null Values.mp3".ToContentUri(LibraryStorageRoot),
			};

			disc.ActiveSongs.Select(x => x.ContentUri).Should().BeEquivalentTo(expectedSongContentUris);

			var expectedImageContentUris = new[]
			{
				"Foreign/Guano Apes/New Tree Title/cover.jpg".ToContentUri(LibraryStorageRoot),
			};

			disc.Images.Select(x => x.ContentUri).Should().BeEquivalentTo(expectedImageContentUris);

			var updatedDisc = await GetDisc(ReferenceData.NormalDiscId, target);
			updatedDisc.Should().BeEquivalentTo(disc, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task UpdateDisc_IfTagsRelatedDataWasChanged_UpdatesDiscCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var disc = await GetDisc(ReferenceData.NormalDiscId, target);

			// Act

			disc.Year = 2021;
			disc.AlbumTitle = "New Album Title";

			await target.UpdateDisc(disc, CancellationToken.None);

			// Assert

			var songTagger = GetService<ISongTagger>();

			var tagData1 = songTagger.GetTagData(Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "01 - Break The Line.mp3"));
			tagData1.Year.Should().Be(2021);
			tagData1.Album.Should().Be("New Album Title");

			var tagData2 = songTagger.GetTagData(Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "Song With Null Values.mp3"));
			tagData2.Year.Should().Be(2021);
			tagData2.Album.Should().Be("New Album Title");

			var updatedDisc = await GetDisc(ReferenceData.NormalDiscId, target);
			updatedDisc.Should().BeEquivalentTo(disc, x => x.IgnoringCyclicReferences());
		}

		[TestMethod]
		public async Task SetDiscCoverImage_ForDiscWithoutCoverImage_SetsCoverImageCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var discCoverImage = new DiscImageModel
			{
				Disc = await GetDisc(ReferenceData.DiscWithNullValuesId, target),
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
			};

			// Act

			await using var imageContent = File.OpenRead("ContentForAdding/NewCover.jpg");
			await target.SetDiscCoverImage(discCoverImage, imageContent, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscCoverImage = new DiscImageModel
			{
				Id = ReferenceData.NextDiscCoverImageId,
				Disc = referenceData.DiscWithNullValues,
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
				Size = 119957,
				Checksum = 1208131419,
				ContentUri = "Foreign/Guano Apes/Disc With Null Values (CD 1)/cover.jpg".ToContentUri(LibraryStorageRoot),
			};

			referenceData.DiscWithNullValues.Images = new[] { expectedDiscCoverImage };

			discCoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var updatedDisc = await GetDisc(ReferenceData.DiscWithNullValuesId, target);
			updatedDisc.CoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var imagePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "Disc With Null Values (CD 1)", "cover.jpg");
			var fileInfo = new FileInfo(imagePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(119957);
		}

		[TestMethod]
		public async Task SetDiscCoverImage_ForDiscWithExistingCoverImageOfSameType_SetsCoverImageCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var discCoverImage = new DiscImageModel
			{
				Disc = await GetDisc(ReferenceData.NormalDiscId, target),
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
			};

			// Act

			await using var imageContent = File.OpenRead("ContentForAdding/NewCover.jpg");
			await target.SetDiscCoverImage(discCoverImage, imageContent, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscCoverImage = new DiscImageModel
			{
				Id = ReferenceData.DiscCoverImageId,
				Disc = referenceData.NormalDisc,
				TreeTitle = "cover.jpg",
				ImageType = DiscImageType.Cover,
				Size = 119957,
				Checksum = 1208131419,
				ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/cover.jpg".ToContentUri(LibraryStorageRoot),
			};

			referenceData.NormalDisc.Images = new[] { expectedDiscCoverImage };

			discCoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var updatedDisc = await GetDisc(ReferenceData.NormalDiscId, target);
			updatedDisc.CoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var imagePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "cover.jpg");
			var fileInfo = new FileInfo(imagePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(119957);
		}

		[TestMethod]
		public async Task SetDiscCoverImage_ForDiscWithExistingCoverImageOfAnotherType_SetsCoverImageCorrectly()
		{
			// Arrange

			var target = CreateTestTarget();

			var discCoverImage = new DiscImageModel
			{
				Disc = await GetDisc(ReferenceData.NormalDiscId, target),
				TreeTitle = "cover.png",
				ImageType = DiscImageType.Cover,
			};

			// Act

			await using var imageContent = File.OpenRead("ContentForAdding/NewCover.png");
			await target.SetDiscCoverImage(discCoverImage, imageContent, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDiscCoverImage = new DiscImageModel
			{
				Id = ReferenceData.DiscCoverImageId,
				Disc = referenceData.NormalDisc,
				TreeTitle = "cover.png",
				ImageType = DiscImageType.Cover,
				Size = 184257,
				Checksum = 1738836760,
				ContentUri = "Foreign/Guano Apes/2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)/cover.png".ToContentUri(LibraryStorageRoot),
			};

			referenceData.NormalDisc.Images = new[] { expectedDiscCoverImage };

			discCoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var updatedDisc = await GetDisc(ReferenceData.NormalDiscId, target);
			updatedDisc.CoverImage.Should().BeEquivalentTo(expectedDiscCoverImage, x => x.IgnoringCyclicReferences());

			var imagePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "cover.png");
			var fileInfo = new FileInfo(imagePath);
			fileInfo.Exists.Should().BeTrue();
			fileInfo.Length.Should().Be(184257);

			var oldImagePath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)", "cover.jpg");
			File.Exists(oldImagePath).Should().BeFalse();
		}

		[TestMethod]
		public async Task DeleteDisc_DeletesDiscSuccessfully()
		{
			// Arrange

			var discDirectoryPath = Path.Combine(LibraryStorageRoot, "Foreign", "Guano Apes", "2004 - Planet Of The Apes - Best Of Guano Apes (CD 1)");
			Directory.Exists(discDirectoryPath).Should().BeTrue();

			var deleteDate = new DateTimeOffset(2021, 07, 03, 13, 25, 33, TimeSpan.FromHours(3));
			var target = CreateTestTarget(StubClock(deleteDate));

			// Act

			await target.DeleteDisc(ReferenceData.NormalDiscId, CancellationToken.None);

			// Assert

			var referenceData = GetReferenceData();
			var expectedDisc = referenceData.NormalDisc;
			foreach (var song in expectedDisc.ActiveSongs)
			{
				song.DeleteDate = deleteDate;
				song.BitRate = null;
				song.Size = null;
				song.Checksum = null;
				song.ContentUri = null;
			}

			var deletedDisc = await GetDisc(ReferenceData.NormalDiscId, target);
			deletedDisc.Should().BeEquivalentTo(expectedDisc, x => x.IgnoringCyclicReferences());

			Directory.Exists(discDirectoryPath).Should().BeFalse();
		}

		private async Task<ShallowFolderModel> GetFolder(ItemId folderId)
		{
			var folderService = GetService<IFoldersService>();
			return await folderService.GetFolder(folderId, CancellationToken.None);
		}

		private static async Task<DiscModel> GetDisc(ItemId discId, IDiscsService discService)
		{
			var allDiscs = await discService.GetAllDiscs(CancellationToken.None);
			return allDiscs.Single(x => x.Id == discId);
		}
	}
}
