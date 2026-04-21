using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class LoyaltyProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loyalty_program",
                columns: table => new
                {
                    loyalty_program_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    airline_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    miles_per_dollar = table.Column<decimal>(type: "decimal(6,2)", nullable: false, defaultValue: 1m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loyalty_program", x => x.loyalty_program_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uq_loyalty_program_airline",
                table: "loyalty_program",
                column: "airline_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_loyalty_program_name",
                table: "loyalty_program",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loyalty_program");
        }
    }
}
