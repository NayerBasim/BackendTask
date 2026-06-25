using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlaylistApi.EF.Migrations
{
    /// <inheritdoc />
    public partial class SeededSongs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "Album", "Artist", "CreatedAt", "Duration", "Genre", "PlaylistId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "3 Daqat", "Abu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 210, "Pop", null, "3 Daqat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Tamally Maak", "Amr Diab", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 240, "Pop", null, "Tamally Maak", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Khaysara", "Saad Lamjarred", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 270, "Pop", null, "Khaysara", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Law Bas Tearaf", "Najwa Karam", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 250, "Pop", null, "Law Bas Tearaf", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));
        }
    }
}
