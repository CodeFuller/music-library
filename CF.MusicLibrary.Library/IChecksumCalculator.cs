﻿namespace CF.MusicLibrary.Library
{
	public interface IChecksumCalculator
	{
		int CalculateChecksumForFile(string fileName);
	}
}
