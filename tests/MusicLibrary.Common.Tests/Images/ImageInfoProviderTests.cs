﻿using CF.Library.Core.Facades;
using MusicLibrary.Common.Images;
using MusicLibrary.Core.Objects.Images;
using NSubstitute;
using NUnit.Framework;

namespace MusicLibrary.Common.Tests.Images
{
	[TestFixture]
	public class ImageInfoProviderTests
	{
		[Test]
		public void GetImageInfo_ReturnsCorrectImageInfo()
		{
			// Arrange

			IImageFacade imageStub = Substitute.For<IImageFacade>();
			imageStub.FromFile("SomeImageFile").Returns(imageStub);
			imageStub.Width.Returns(800);
			imageStub.Height.Returns(600);
			imageStub.Format.Returns(ImageFormatType.Png);

			IFileSystemFacade fileSystemStub = Substitute.For<IFileSystemFacade>();
			fileSystemStub.GetFileSize("SomeImageFile").Returns(12345);

			var target = new ImageInfoProvider(imageStub, fileSystemStub);

			// Act

			var imageInfo = target.GetImageInfo("SomeImageFile");

			// Assert

			Assert.AreEqual("SomeImageFile", imageInfo.FileName);
			Assert.AreEqual(800, imageInfo.Width);
			Assert.AreEqual(600, imageInfo.Height);
			Assert.AreEqual(12345, imageInfo.FileSize);
			Assert.AreEqual(ImageFormatType.Png, imageInfo.Format);
		}
	}
}