using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverstromingsApp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedUtc", "Email", "PasswordHash", "PasswordSalt" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 6, 18, 1, 51, 19, 431, DateTimeKind.Utc).AddTicks(6080), "admin@example.com", new byte[] { 81, 175, 43, 154, 99, 248, 161, 137, 141, 131, 166, 10, 253, 125, 7, 165, 221, 161, 107, 79, 10, 142, 175, 128, 245, 242, 128, 88, 135, 250, 121, 85 }, new byte[] { 132, 78, 229, 227, 21, 61, 90, 82, 37, 108, 210, 148, 61, 19, 225, 146 } });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
