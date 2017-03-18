﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingContent
{
	public class AlbumContentParser : IAlbumContentParser
	{
		private readonly IInputContentSplitter inputContentSplitter;
		private readonly IEthalonAlbumParser ethalonAlbumParser;

		public AlbumContentParser(IInputContentSplitter inputContentSplitter, IEthalonAlbumParser ethalonAlbumParser)
		{
			if (inputContentSplitter == null)
			{
				throw new ArgumentNullException(nameof(inputContentSplitter));
			}
			if (ethalonAlbumParser == null)
			{
				throw new ArgumentNullException(nameof(ethalonAlbumParser));
			}

			this.inputContentSplitter = inputContentSplitter;
			this.ethalonAlbumParser = ethalonAlbumParser;
		}

		public IEnumerable<AlbumContent> Parse(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			return Parse(content.Split(new [] { "\r\n", "\n" }, StringSplitOptions.None));
		}

		private IEnumerable<AlbumContent> Parse(IEnumerable<string> content)
		{
			return inputContentSplitter.Split(content).
				Select(c => ethalonAlbumParser.Parse(c));
		}
	}
}