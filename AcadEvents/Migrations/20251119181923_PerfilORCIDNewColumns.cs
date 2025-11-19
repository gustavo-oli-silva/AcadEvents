using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcadEvents.Migrations
{
    /// <inheritdoc />
    public partial class PerfilORCIDNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "PerfisORCID",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncAt",
                table: "PerfisORCID",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "PerfisORCID",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiresAt",
                table: "PerfisORCID",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "PerfisORCID");

            migrationBuilder.DropColumn(
                name: "LastSyncAt",
                table: "PerfisORCID");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "PerfisORCID");

            migrationBuilder.DropColumn(
                name: "TokenExpiresAt",
                table: "PerfisORCID");
        }
    }
}
