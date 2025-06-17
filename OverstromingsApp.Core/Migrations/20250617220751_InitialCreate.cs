using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverstromingsApp.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Neerslag",
                columns: table => new
                {
                    Jaar = table.Column<int>(type: "int", nullable: false),
                    Maand = table.Column<int>(type: "int", nullable: false),
                    NeerslagMM = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Neerslag", x => new { x.Jaar, x.Maand });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Neerslag");
        }
    }
}
