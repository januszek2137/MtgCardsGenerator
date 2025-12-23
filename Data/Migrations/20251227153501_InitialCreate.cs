using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiMagicCardsGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScryfallId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ManaCost = table.Column<string>(type: "TEXT", nullable: true),
                    Cmc = table.Column<decimal>(type: "TEXT", nullable: false),
                    TypeLine = table.Column<string>(type: "TEXT", nullable: false),
                    OracleText = table.Column<string>(type: "TEXT", nullable: true),
                    Power = table.Column<string>(type: "TEXT", nullable: true),
                    Toughness = table.Column<string>(type: "TEXT", nullable: true),
                    Colors = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", nullable: false),
                    FlavorText = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Cmc",
                table: "Cards",
                column: "Cmc");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Colors",
                table: "Cards",
                column: "Colors");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Name",
                table: "Cards",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ScryfallId",
                table: "Cards",
                column: "ScryfallId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_TypeLine",
                table: "Cards",
                column: "TypeLine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");
        }
    }
}
