using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImportedTracks_UserId",
                table: "ImportedTracks");

            migrationBuilder.AlterColumn<string>(
                name: "MasterMetadataTrackName",
                table: "ImportedTracks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MasterMetadataArtistName",
                table: "ImportedTracks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MasterMetadataAlbumName",
                table: "ImportedTracks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MsListened",
                table: "AggregateBase",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportedTracks_UserId_AlbumName",
                table: "ImportedTracks",
                columns: new[] { "UserId", "MasterMetadataAlbumName" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportedTracks_UserId_ArtistName",
                table: "ImportedTracks",
                columns: new[] { "UserId", "MasterMetadataArtistName" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportedTracks_UserId_TrackName",
                table: "ImportedTracks",
                columns: new[] { "UserId", "MasterMetadataTrackName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImportedTracks_UserId_AlbumName",
                table: "ImportedTracks");

            migrationBuilder.DropIndex(
                name: "IX_ImportedTracks_UserId_ArtistName",
                table: "ImportedTracks");

            migrationBuilder.DropIndex(
                name: "IX_ImportedTracks_UserId_TrackName",
                table: "ImportedTracks");

            migrationBuilder.AlterColumn<string>(
                name: "MasterMetadataTrackName",
                table: "ImportedTracks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MasterMetadataArtistName",
                table: "ImportedTracks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MasterMetadataAlbumName",
                table: "ImportedTracks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MsListened",
                table: "AggregateBase",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportedTracks_UserId",
                table: "ImportedTracks",
                column: "UserId");
        }
    }
}
