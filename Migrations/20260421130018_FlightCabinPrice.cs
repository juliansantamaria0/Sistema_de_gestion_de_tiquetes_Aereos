using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class FlightCabinPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "flight_cabin_price",
                columns: table => new
                {
                    flight_cabin_price_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    scheduled_flight_id = table.Column<int>(type: "int", nullable: false),
                    cabin_class_id = table.Column<int>(type: "int", nullable: false),
                    fare_type_id = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flight_cabin_price", x => x.flight_cabin_price_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uq_fcp",
                table: "flight_cabin_price",
                columns: new[] { "scheduled_flight_id", "cabin_class_id", "fare_type_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "flight_cabin_price");
        }
    }
}
