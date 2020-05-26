CREATE TABLE [Folders] (
  [Id] INTEGER NOT NULL,
  [ParentFolder_Id] INTEGER NULL,
  [Name] ntext NOT NULL,
  [DeleteDate] datetime NULL,
  CONSTRAINT [sqlite_master_PK_Folders] PRIMARY KEY ([Id]),
  FOREIGN KEY ([ParentFolder_Id]) REFERENCES [Folders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

/* TODO: Remove Uri column */
CREATE TABLE [Discs] (
  [Id] INTEGER NOT NULL,
  [Folder_Id] INTEGER NOT NULL,
  [Title] ntext NOT NULL,
  [TreeTitle] ntext NOT NULL,
  [AlbumTitle] ntext NULL,
  [Uri] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Discs] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Discs] UNIQUE (Uri),
  FOREIGN KEY ([Folder_Id]) REFERENCES [Folders] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Artists] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Artists] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Artists] UNIQUE (Name)
);

CREATE TABLE [Genres] (
  [Id] INTEGER NOT NULL,
  [Name] ntext NOT NULL,
  CONSTRAINT [sqlite_master_PK_Genres] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Genres] UNIQUE (Name)
);

/* TODO: Remove Uri column */
CREATE TABLE [Songs] (
  [Id] INTEGER NOT NULL,
  [Artist_Id] int NULL,
  [Disc_Id] int NOT NULL,
  [Genre_Id] int NULL,
  [TrackNumber] smallint NULL,
  [Year] smallint NULL,
  [Title] ntext NOT NULL,
  [TreeTitle] ntext NOT NULL,
  [Duration] float NOT NULL,
  [Rating] int NULL,
  [Uri] ntext NOT NULL,
  /* TODO: It can be NULL for deleted songs */
  [FileSize] int NOT NULL,
  /* TODO: It can be NULL for deleted songs */
  /* TODO: Make it uint */
  [Checksum] int NOT NULL,
  [Bitrate] int NULL,
  [LastPlaybackTime] datetime NULL,
  [PlaybacksCount] int NOT NULL,
  [DeleteDate] datetime NULL,
  CONSTRAINT [sqlite_master_PK_Songs] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Songs] UNIQUE (Uri),
  FOREIGN KEY ([Genre_Id]) REFERENCES [Genres] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
  FOREIGN KEY ([Disc_Id]) REFERENCES [Discs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION,
  FOREIGN KEY ([Artist_Id]) REFERENCES [Artists] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

CREATE TABLE [Playbacks] (
  [Id] INTEGER NOT NULL,
  [Song_Id] int NOT NULL,
  [PlaybackTime] datetime NOT NULL,
  CONSTRAINT [sqlite_master_PK_Playbacks] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Playbacks] UNIQUE (PlaybackTime, Song_Id),
  FOREIGN KEY ([Song_Id]) REFERENCES [Songs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
);

/* TODO: Remove Uri column */
CREATE TABLE [DiscImages] (
  [Id] INTEGER NOT NULL,
  [Disc_Id] int NOT NULL,
  [TreeTitle] ntext NOT NULL,
  [ImageType] int NOT NULL,
  [Uri] ntext NOT NULL,
  [FileSize] int NOT NULL,
  [Checksum] int NOT NULL,
  CONSTRAINT [sqlite_master_PK_Discs] PRIMARY KEY ([Id]),
  CONSTRAINT [sqlite_master_UC_Discs] UNIQUE (Uri),
  FOREIGN KEY ([Disc_Id]) REFERENCES [Discs] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE INDEX [IX_Genre_Id] ON [Songs] ([Genre_Id] ASC);
CREATE INDEX [IX_Disc_Id] ON [Songs] ([Disc_Id] ASC);
CREATE INDEX [IX_Artist_Id] ON [Songs] ([Artist_Id] ASC);
CREATE INDEX [IX_Song_Id] ON [Playbacks] ([Song_Id] ASC);
CREATE INDEX [IX_Folder_ParentFolder_Id] ON [Folders] ([ParentFolder_Id] ASC);
CREATE INDEX [IX_Disc_Folder_Id] ON [Discs] ([Folder_Id] ASC);
