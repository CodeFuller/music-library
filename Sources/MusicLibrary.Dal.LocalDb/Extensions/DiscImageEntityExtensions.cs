using System;
using System.Linq;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using DiscImageType = MusicLibrary.Core.Models.DiscImageType;

namespace MusicLibrary.Dal.LocalDb.Extensions
{
	internal static class DiscImageEntityExtensions
	{
		public static DiscImageModel ToModel(this DiscImageEntity discImage, IUriTranslator uriTranslator)
		{
			return new DiscImageModel
			{
				Id = discImage.Id.ToItemId(),
				TreeTitle = new ItemUriParts(discImage.Uri).Last(),
				ImageType = ConvertImageType(discImage.ImageType),
				Uri = uriTranslator.GetExternalUri(discImage.Uri),
				Size = discImage.FileSize,
			};
		}

		private static DiscImageType ConvertImageType(Entities.DiscImageType imageType)
		{
			switch (imageType)
			{
				case Entities.DiscImageType.Cover:
					return DiscImageType.Cover;

				default:
					throw new InvalidOperationException($"Unexpected disc image type: {imageType}");
			}
		}
	}
}
