using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateUploadHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UploadHistoryId",
                table: "ImportedTracks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UploadHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportedTracks_UploadHistoryId",
                table: "ImportedTracks",
                column: "UploadHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadHistories_UserId",
                table: "UploadHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportedTracks_UploadHistories_UploadHistoryId",
                table: "ImportedTracks",
                column: "UploadHistoryId",
                principalTable: "UploadHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportedTracks_UploadHistories_UploadHistoryId",
                table: "ImportedTracks");

            migrationBuilder.DropTable(
                name: "UploadHistories");

            migrationBuilder.DropIndex(
                name: "IX_ImportedTracks_UploadHistoryId",
                table: "ImportedTracks");

            migrationBuilder.DropColumn(
                name: "UploadHistoryId",
                table: "ImportedTracks");
        }
    }
}
