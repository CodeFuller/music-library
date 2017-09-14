﻿using System.Data.Common;
using System.Data.Entity;
using CF.MusicLibrary.BL.Objects;

namespace CF.MusicLibrary.Dal
{
	public class MusicLibraryEntities : DbContext
	{
		public MusicLibraryEntities() :
			base("name=MusicLibraryEntities")
		{
		}

		public MusicLibraryEntities(DbConnection dbConnection) :
			base(dbConnection, false)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Artist>().Property(a => a.Name).IsRequired();

			modelBuilder.Entity<Disc>().Ignore(d => d.Artist);
			modelBuilder.Entity<Disc>().Ignore(d => d.Genre);
			modelBuilder.Entity<Disc>().Property(d => d.Title).IsRequired();
			modelBuilder.Entity<Disc>().Ignore(d => d.Uri);
			modelBuilder.Entity<Disc>().Property(d => d.DiscUri).IsRequired().HasColumnName("Uri");
			modelBuilder.Entity<Disc>().Ignore(d => d.LastPlaybackTime);
			modelBuilder.Entity<Disc>().Ignore(d => d.PlaybacksPassed);
			modelBuilder.Entity<Disc>().Ignore(d => d.Songs);
			modelBuilder.Entity<Disc>().Ignore(d => d.IsDeleted);

			modelBuilder.Entity<Genre>().Property(g => g.Name).IsRequired();

			modelBuilder.Entity<Song>().Property(s => s.Title).IsRequired();
			modelBuilder.Entity<Song>().Ignore(s => s.Duration);
			modelBuilder.Entity<Song>().Property(s => s.DurationInMilliseconds).IsRequired().HasColumnName("Duration");
			modelBuilder.Entity<Song>().Ignore(s => s.Uri);
			modelBuilder.Entity<Song>().Property(s => s.SongUri).IsRequired().HasColumnName("Uri");
			modelBuilder.Entity<Song>().Ignore(s => s.IsDeleted);
		}

		public DbSet<Artist> Artists { get; set; }

		public DbSet<Disc> Discs { get; set; }

		public DbSet<Song> Songs { get; set; }

		public DbSet<Genre> Genres { get; set; }
	}
}