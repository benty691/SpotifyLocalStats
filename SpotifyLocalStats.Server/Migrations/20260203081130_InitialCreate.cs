using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CopyrightContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopyrightContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyTrackUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    IsSingle = table.Column<bool>(type: "bit", nullable: false),
                    IsExplicit = table.Column<bool>(type: "bit", nullable: false),
                    SpotifyTrackId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackNumber = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseDatePrecision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscNumber = table.Column<int>(type: "int", nullable: false),
                    PreviewUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvaliableMarkets = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimesPlayed = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTimeUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Auth = table.Column<bool>(type: "bit", nullable: true),
                    SpotifyPermissions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyHref = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPremium = table.Column<bool>(type: "bit", nullable: true),
                    HasImportedHistorical = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalTracks = table.Column<int>(type: "int", nullable: false),
                    AvaliableMarkets = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RealeaseDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseDatePrecision = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CopyrightId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimesPlayed = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_CopyrightContent_CopyrightId",
                        column: x => x.CopyrightId,
                        principalTable: "CopyrightContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimesPlayed = table.Column<int>(type: "int", nullable: false),
                    IsBand = table.Column<bool>(type: "bit", nullable: false),
                    DOB = table.Column<DateOnly>(type: "date", nullable: false),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artists_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AggregatedTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    MsListened = table.Column<int>(type: "int", nullable: false),
                    MinsListened = table.Column<double>(type: "float", nullable: false),
                    TopListeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeFirstListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeLastListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakDays = table.Column<int>(type: "int", nullable: false),
                    LongestStreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStreakDays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongestDrySpellStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpellEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpell = table.Column<int>(type: "int", nullable: false),
                    MostTimesIn24Hours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedTracks_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatedTracks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AggregatedTracks_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportedTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MsPlayed = table.Column<int>(type: "int", nullable: false),
                    ConnCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MasterMetadataTrackName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MasterMetadataArtistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MasterMetadataAlbumName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyTrackUri = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EpisodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EpisodeShowName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyEpisodeUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AudiobookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AudiobookUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AudiobookChapterUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AudiobookChapterTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonStart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonEnd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsShuffle = table.Column<bool>(type: "bit", nullable: false),
                    IsSkipped = table.Column<bool>(type: "bit", nullable: false),
                    IsOffline = table.Column<bool>(type: "bit", nullable: false),
                    OfflineTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IncognitoMode = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportedTracks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AggregatedAlbums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimesCompleted = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    MsListened = table.Column<int>(type: "int", nullable: false),
                    MinsListened = table.Column<double>(type: "float", nullable: false),
                    TopListeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeFirstListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeLastListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakDays = table.Column<int>(type: "int", nullable: false),
                    LongestStreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStreakDays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongestDrySpellStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpellEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpell = table.Column<int>(type: "int", nullable: false),
                    MostTimesIn24Hours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedAlbums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedAlbums_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatedAlbums_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AggregatedAlbums_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumTrack",
                columns: table => new
                {
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TracksId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumTrack", x => new { x.AlbumId, x.TracksId });
                    table.ForeignKey(
                        name: "FK_AlbumTrack_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumTrack_Tracks_TracksId",
                        column: x => x.TracksId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AggregatedArtists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueTracksPlayed = table.Column<int>(type: "int", nullable: false),
                    AlbumsListened = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    MsListened = table.Column<int>(type: "int", nullable: false),
                    MinsListened = table.Column<double>(type: "float", nullable: false),
                    TopListeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeFirstListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeLastListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakDays = table.Column<int>(type: "int", nullable: false),
                    LongestStreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStreakDays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongestDrySpellStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpellEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpell = table.Column<int>(type: "int", nullable: false),
                    MostTimesIn24Hours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedArtists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatedArtists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AggregatedArtists_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumArtist",
                columns: table => new
                {
                    AlbumsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArtistsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumArtist", x => new { x.AlbumsId, x.ArtistsId });
                    table.ForeignKey(
                        name: "FK_AlbumArtist_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumArtist_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalId",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Isrc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ean = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Upc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalId_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExternalId_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExternalId_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Image_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Image_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrackTimeOfDaysStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeOfDay = table.Column<int>(type: "int", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackTimeOfDaysStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackTimeOfDaysStats_AggregatedTracks_AggregateId",
                        column: x => x.AggregateId,
                        principalTable: "AggregatedTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlbumTimeOfDaysStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeOfDay = table.Column<int>(type: "int", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumTimeOfDaysStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumTimeOfDaysStats_AggregatedAlbums_AggregateId",
                        column: x => x.AggregateId,
                        principalTable: "AggregatedAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistTimeOfDaysStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeOfDay = table.Column<int>(type: "int", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistTimeOfDaysStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistTimeOfDaysStats_AggregatedArtists_AggregateId",
                        column: x => x.AggregateId,
                        principalTable: "AggregatedArtists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedAlbums_AlbumId",
                table: "AggregatedAlbums",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedAlbums_UserId",
                table: "AggregatedAlbums",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedAlbums_UserId1",
                table: "AggregatedAlbums",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedArtists_ArtistId",
                table: "AggregatedArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedArtists_UserId",
                table: "AggregatedArtists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedArtists_UserId1",
                table: "AggregatedArtists",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedTracks_TrackId",
                table: "AggregatedTracks",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedTracks_UserId",
                table: "AggregatedTracks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedTracks_UserId1",
                table: "AggregatedTracks",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtist_ArtistsId",
                table: "AlbumArtist",
                column: "ArtistsId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_CopyrightId",
                table: "Albums",
                column: "CopyrightId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumTimeOfDaysStats_AggregateId",
                table: "AlbumTimeOfDaysStats",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumTrack_TracksId",
                table: "AlbumTrack",
                column: "TracksId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_TrackId",
                table: "Artists",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistTimeOfDaysStats_AggregateId",
                table: "ArtistTimeOfDaysStats",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalId_AlbumId",
                table: "ExternalId",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalId_ArtistId",
                table: "ExternalId",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalId_TrackId",
                table: "ExternalId",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_AlbumId",
                table: "Image",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_ArtistId",
                table: "Image",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_UserId",
                table: "Image",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportedTracks_UserId",
                table: "ImportedTracks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TsAndSpotifyUriAndUserId",
                table: "ImportedTracks",
                columns: new[] { "TimeStamp", "SpotifyTrackUri", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackTimeOfDaysStats_AggregateId",
                table: "TrackTimeOfDaysStats",
                column: "AggregateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumArtist");

            migrationBuilder.DropTable(
                name: "AlbumTimeOfDaysStats");

            migrationBuilder.DropTable(
                name: "AlbumTrack");

            migrationBuilder.DropTable(
                name: "ArtistTimeOfDaysStats");

            migrationBuilder.DropTable(
                name: "ExternalId");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "ImportedTracks");

            migrationBuilder.DropTable(
                name: "TrackTimeOfDaysStats");

            migrationBuilder.DropTable(
                name: "AggregatedAlbums");

            migrationBuilder.DropTable(
                name: "AggregatedArtists");

            migrationBuilder.DropTable(
                name: "AggregatedTracks");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CopyrightContent");

            migrationBuilder.DropTable(
                name: "Tracks");
        }
    }
}
