﻿using System;
using System.IO;
using MusicLibrary.DiscAdder.Interfaces;
using MusicLibrary.Shared.Images;

namespace MusicLibrary.DiscAdder.Internal
{
	internal class SourceFileTypeResolver : ISourceFileTypeResolver
	{
		private readonly IDiscImageValidator discImageValidator;

		public SourceFileTypeResolver(IDiscImageValidator discImageValidator)
		{
			this.discImageValidator = discImageValidator ?? throw new ArgumentNullException(nameof(discImageValidator));
		}

		public SourceFileType GetSourceFileType(string filePath)
		{
			if (Path.GetExtension(filePath) == ".mp3")
			{
				return SourceFileType.Song;
			}

			if (discImageValidator.IsSupportedFileFormat(filePath))
			{
				return SourceFileType.Image;
			}

			throw new InvalidOperationException($"Failed to recognize type of source file '{filePath}'");
		}
	}
}
