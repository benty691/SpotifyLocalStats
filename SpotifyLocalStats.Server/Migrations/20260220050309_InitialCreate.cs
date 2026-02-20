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
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Genres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimesPlayed = table.Column<int>(type: "int", nullable: true),
                    IsBand = table.Column<bool>(type: "bit", nullable: true),
                    DOB = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CopyrightContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopyrightContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportJobStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProgressPercent = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportJobStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTimeUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalTracks = table.Column<int>(type: "int", nullable: true),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvaliableMarkets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RealeaseDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseDatePrecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CopyrightId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimesPlayed = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Albums_CopyrightContent_CopyrightId",
                        column: x => x.CopyrightId,
                        principalTable: "CopyrightContent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AggregateBase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    MsListened = table.Column<int>(type: "int", nullable: true),
                    TopListeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeFirstListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeLastListened = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakDays = table.Column<int>(type: "int", nullable: false),
                    LongestStreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStreakDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LongestDrySpellStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpellEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestDrySpell = table.Column<int>(type: "int", nullable: false),
                    MostTimesIn24Hours = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregateBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregateBase_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImportedTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MsPlayed = table.Column<int>(type: "int", nullable: true),
                    ConnCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MasterMetadataTrackName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MasterMetadataArtistName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MasterMetadataAlbumName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyTrackUri = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EpisodeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EpisodeShowName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotifyEpisodeUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AudiobookTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AudiobookUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AudiobookChapterUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AudiobookChapterTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsShuffle = table.Column<bool>(type: "bit", nullable: true),
                    IsSkipped = table.Column<bool>(type: "bit", nullable: true),
                    IsOffline = table.Column<bool>(type: "bit", nullable: true),
                    OfflineTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncognitoMode = table.Column<bool>(type: "bit", nullable: true),
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
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Width = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpotifyTrackUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MsPlayed = table.Column<int>(type: "int", nullable: true),
                    IsSingle = table.Column<bool>(type: "bit", nullable: true),
                    IsExplicit = table.Column<bool>(type: "bit", nullable: true),
                    SpotifyTrackId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackNumber = table.Column<int>(type: "int", nullable: true),
                    ReleaseDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseDatePrecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscNumber = table.Column<int>(type: "int", nullable: true),
                    PreviewUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvaliableMarkets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimesPlayed = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tracks_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AggregatedAlbums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimesCompleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedAlbums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedAlbums_AggregateBase_Id",
                        column: x => x.Id,
                        principalTable: "AggregateBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatedAlbums_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
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
                    AlbumsListened = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedArtists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedArtists_AggregateBase_Id",
                        column: x => x.Id,
                        principalTable: "AggregateBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatedArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AggregatedTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedTracks_AggregateBase_Id",
                        column: x => x.Id,
                        principalTable: "AggregateBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatedTracks_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalId",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Isrc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Upc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_AggregateBase_UserId",
                table: "AggregateBase",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedAlbums_AlbumId",
                table: "AggregatedAlbums",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedArtists_ArtistId",
                table: "AggregatedArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedTracks_TrackId",
                table: "AggregatedTracks",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Albums_CopyrightId",
                table: "Albums",
                column: "CopyrightId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumTimeOfDaysStats_AggregateId",
                table: "AlbumTimeOfDaysStats",
                column: "AggregateId");

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
                unique: true,
                filter: "[SpotifyTrackUri] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_AlbumId",
                table: "Tracks",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ArtistId",
                table: "Tracks",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTimeOfDaysStats_AggregateId",
                table: "TrackTimeOfDaysStats",
                column: "AggregateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumTimeOfDaysStats");

            migrationBuilder.DropTable(
                name: "ArtistTimeOfDaysStats");

            migrationBuilder.DropTable(
                name: "ExternalId");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropTable(
                name: "ImportedTracks");

            migrationBuilder.DropTable(
                name: "ImportJobStatuses");

            migrationBuilder.DropTable(
                name: "TrackTimeOfDaysStats");

            migrationBuilder.DropTable(
                name: "AggregatedAlbums");

            migrationBuilder.DropTable(
                name: "AggregatedArtists");

            migrationBuilder.DropTable(
                name: "AggregatedTracks");

            migrationBuilder.DropTable(
                name: "AggregateBase");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "CopyrightContent");
        }
    }
}
