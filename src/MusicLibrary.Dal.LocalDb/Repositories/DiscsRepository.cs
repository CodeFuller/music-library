﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Core.Models;
using MusicLibrary.Dal.LocalDb.Entities;
using MusicLibrary.Dal.LocalDb.Extensions;
using MusicLibrary.Dal.LocalDb.Interfaces;
using MusicLibrary.Dal.LocalDb.Internal;
using MusicLibrary.Services.Interfaces.Dal;

namespace MusicLibrary.Dal.LocalDb.Repositories
{
	internal class DiscsRepository : IDiscsRepository
	{
		private readonly IMusicLibraryDbContextFactory contextFactory;

		private readonly IContentUriProvider contentUriProvider;

		public DiscsRepository(IMusicLibraryDbContextFactory contextFactory, IContentUriProvider contentUriProvider)
		{
			this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
			this.contentUriProvider = contentUriProvider ?? throw new ArgumentNullException(nameof(contentUriProvider));
		}

		public async Task CreateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			var discEntity = disc.ToEntity();

			await using var context = contextFactory.Create();
			await context.Discs.AddAsync(discEntity, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);

			disc.Id = discEntity.Id.ToItemId();
		}

		public async Task<IReadOnlyCollection<DiscModel>> GetAllDiscs(CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var discEntities = await GetDiscsQueryable(context)
				.ToListAsync(cancellationToken);

			var folderModels = discEntities
				.Select(disc => disc.Folder)
				.Distinct()
				.Select(folder => folder.ToShallowModel())
				.ToDictionary(folder => folder.Id, folder => folder);

			return discEntities
					.Select(disc => disc.ToModel(folderModels[disc.Folder.Id.ToItemId()], contentUriProvider))
					.ToList();
		}

		public async Task<DiscModel> GetDisc(ItemId discId, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var disc = await FindDisc(context, discId, cancellationToken);
			return disc.ToModel(contentUriProvider);
		}

		private static IQueryable<DiscEntity> GetDiscsQueryable(MusicLibraryDbContext context)
		{
			return context.Discs
				.Include(disc => disc.Folder)
				.Include(disc => disc.Songs).ThenInclude(song => song.Artist)
				.Include(disc => disc.Songs).ThenInclude(song => song.Genre)
				.Include(disc => disc.Images);
		}

		public async Task UpdateDisc(DiscModel disc, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();
			var discEntity = await FindDisc(context, disc.Id, cancellationToken);

			var updatedEntity = disc.ToEntity();
			context.Entry(discEntity).CurrentValues.SetValues(updatedEntity);
			await context.SaveChangesAsync(cancellationToken);
		}

		public async Task AddDiscImage(DiscImageModel image, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();
			var discEntity = await FindDisc(context, image.Disc.Id, cancellationToken);

			var imageEntity = image.ToEntity();
			discEntity.Images.Add(imageEntity);

			await context.SaveChangesAsync(cancellationToken);

			image.Id = imageEntity.Id.ToItemId();
		}

		public async Task DeleteDiscImage(DiscImageModel image, CancellationToken cancellationToken)
		{
			await using var context = contextFactory.Create();

			var imageEntity = await context.DiscImages.FindAsync(new object[] { image.Id.ToInt32() }, cancellationToken);
			context.DiscImages.Remove(imageEntity);

			await context.SaveChangesAsync(cancellationToken);
		}

		private static async Task<DiscEntity> FindDisc(MusicLibraryDbContext context, ItemId id, CancellationToken cancellationToken)
		{
			var entityId = id.ToInt32();
			return await GetDiscsQueryable(context)
				.SingleAsync(s => s.Id == entityId, cancellationToken);
		}
	}
}