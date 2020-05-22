﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MusicLibrary.Core.Interfaces;
using MusicLibrary.Core.Objects;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Logic.Interfaces.Dal;
using MusicLibrary.Logic.Models;

namespace MusicLibrary.Dal.LocalDb
{
	internal class DiscsRepository : IDiscsRepository
	{
		private readonly IMusicLibrary musicLibrary;

		private readonly IDataStorage dataStorage;

		private readonly DiscLibrary discLibrary;

		public DiscsRepository(IMusicLibrary musicLibrary, IDataStorage dataStorage, DiscLibrary discLibrary)
		{
			this.musicLibrary = musicLibrary ?? throw new ArgumentNullException(nameof(musicLibrary));
			this.dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
			this.discLibrary = discLibrary ?? throw new ArgumentNullException(nameof(discLibrary));
		}

		public Task<DiscModel> GetDisc(ItemId discId, CancellationToken cancellationToken)
		{
			var disc = FindDisc(discId);
			return Task.FromResult(disc.ToModel(dataStorage));
		}

		public Task UpdateDisc(DiscModel discModel, CancellationToken cancellationToken)
		{
			// TODO: Implement
			throw new NotImplementedException();
		}

		public Task DeleteDisc(ItemId discId, CancellationToken cancellationToken)
		{
			var disc = FindDisc(discId);
			return musicLibrary.DeleteDisc(disc);
		}

		private Disc FindDisc(ItemId discId)
		{
			return discLibrary.Discs.Single(d => d.Id.ToItemId() == discId);
		}
	}
}
