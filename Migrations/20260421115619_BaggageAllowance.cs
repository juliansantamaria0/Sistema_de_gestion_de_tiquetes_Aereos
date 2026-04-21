using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class BaggageAllowance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "baggage_allowance",
                columns: table => new
                {
                    baggage_allowance_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cabin_class_id = table.Column<int>(type: "int", nullable: false),
                    fare_type_id = table.Column<int>(type: "int", nullable: false),
                    carry_on_pieces = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    carry_on_kg = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 10m),
                    checked_pieces = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    checked_kg = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_baggage_allowance", x => x.baggage_allowance_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uq_ba",
                table: "baggage_allowance",
                columns: new[] { "cabin_class_id", "fare_type_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "baggage_allowance");
        }
    }
}
