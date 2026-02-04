using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class OnModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggregatedAlbums_Users_UserId1",
                table: "AggregatedAlbums");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregatedArtists_Users_UserId1",
                table: "AggregatedArtists");

            migrationBuilder.DropForeignKey(
                name: "FK_AggregatedTracks_Users_UserId1",
                table: "AggregatedTracks");

            migrationBuilder.DropIndex(
                name: "IX_AggregatedTracks_UserId1",
                table: "AggregatedTracks");

            migrationBuilder.DropIndex(
                name: "IX_AggregatedArtists_UserId1",
                table: "AggregatedArtists");

            migrationBuilder.DropIndex(
                name: "IX_AggregatedAlbums_UserId1",
                table: "AggregatedAlbums");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AggregatedTracks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AggregatedArtists");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AggregatedAlbums");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "AggregatedTracks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "AggregatedArtists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "AggregatedAlbums",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedTracks_UserId1",
                table: "AggregatedTracks",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedArtists_UserId1",
                table: "AggregatedArtists",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedAlbums_UserId1",
                table: "AggregatedAlbums",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AggregatedAlbums_Users_UserId1",
                table: "AggregatedAlbums",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregatedArtists_Users_UserId1",
                table: "AggregatedArtists",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AggregatedTracks_Users_UserId1",
                table: "AggregatedTracks",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
