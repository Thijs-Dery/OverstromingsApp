using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverstromingsApp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedUtc", "Role" },
                values: new object[] { new DateTime(2025, 6, 18, 2, 13, 34, 937, DateTimeKind.Utc).AddTicks(3234), "Standaard" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedUtc",
                value: new DateTime(2025, 6, 18, 1, 51, 19, 431, DateTimeKind.Utc).AddTicks(6080));
        }
    }
}
