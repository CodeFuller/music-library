﻿using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace CF.MusicLibrary.AlbumPreprocessor.ParsingSong
{
	public class EthalonSongParser : IEthalonSongParser
	{
		internal static ReadOnlyCollection<SongTitlePattern> TitlePatterns { get; } = new ReadOnlyCollection<SongTitlePattern>(
			new SongTitlePattern[]
			{
				new SongTitlePattern()
				{
					Description = "Wikipedia: track, title, payload, length",
					Source = "https://en.wikipedia.org/wiki/The_Human_Contradiction",
					Pattern = @"^\d+\.\s+""(.+?)""\s+\((.+?)\)\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("2.	\"Your Body Is A Battleground\" (featuring Marco Hietala)	3:50", "Your Body Is A Battleground (feat. Marco Hietala)"),
					}
				},

				new SongTitlePattern()
				{
					Description = "Wikipedia: track, title, payload (optional), authors, length (optional)",
					Source = "https://en.wikipedia.org/wiki/We_Are_the_Others",
					Pattern = @"^\d+\.\s+""(.+?)""\s+(?:\((.+?)\)\s+)?.+?(\s+\d+:\d+)?$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1.	\"Mother Machine\"	Martijn Westerholt, Charlotte Wessels, Guus Eikens, Tripod	4:34", "Mother Machine"),
						new SongParsingTest("15.	\"Shattered\" (live)	Westerholt	4:20", "Shattered (Live)"),
						new SongParsingTest("17.	\"Come Closer\" (live)	Westerholt, Wessels", "Come Closer (Live)"),
					}
				},

				new SongTitlePattern()
				{
					Description = "Wikipedia: track, title, length",
					Source = "https://en.wikipedia.org/wiki/The_Human_Contradiction",
					Pattern = @"^\d+\.\s+""(.+?)""\s+\d+:\d+$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("1.	\"Here Come The Vultures\"	6:05", "Here Come The Vultures"),
					}
				},

				new SongTitlePattern()
				{
					Description = "Title followed by tab and payload data",
					Pattern = @"^(.+?)\t+(.*)$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("Along Comes Mary	live", "Along Comes Mary (Live)"),
					}
				},

				new SongTitlePattern()
				{
					Description = "Trimmed raw title",
					Pattern = @"^\s*(.+?)\s*$",
					Tests = new Collection<SongParsingTest>
					{
						new SongParsingTest("Mother Machine", "Mother Machine"),
					}
				},
			}
		);

		public string ParseSongTitle(string rawSongTitle)
		{
			foreach (var pattern in TitlePatterns)
			{
				SongTitleMatch match = pattern.Match(rawSongTitle, ParseSongPayload);
				if (match.Success)
				{
					return match.SongTitle;
				}
			}

			//	If all paterns failed, we just return the raw title itself.
			return rawSongTitle;
		}

		private static string ParseSongPayload(string rawSongPayload)
		{
			if (rawSongPayload == "live")
			{
				return "(Live)";
			}

			var match = new Regex("^live, featuring (.+?)$").Match(rawSongPayload);
			if (match.Success)
			{
				return FormattableString.Invariant($"(feat. {match.Groups[1].Value}) (Live)");
			}

			match = new Regex("^featuring (.+?)$").Match(rawSongPayload);
			if (match.Success)
			{
				return FormattableString.Invariant($"(feat. {match.Groups[1].Value})");
			}

			return FormattableString.Invariant($"({rawSongPayload})");
		}
	}
}