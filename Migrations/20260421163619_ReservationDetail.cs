using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class ReservationDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservation_detail",
                columns: table => new
                {
                    reservation_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    reservation_id = table.Column<int>(type: "int", nullable: false),
                    passenger_id = table.Column<int>(type: "int", nullable: false),
                    flight_seat_id = table.Column<int>(type: "int", nullable: false),
                    fare_type_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_detail", x => x.reservation_detail_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uq_rd_passenger",
                table: "reservation_detail",
                columns: new[] { "reservation_id", "passenger_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_rd_seat",
                table: "reservation_detail",
                columns: new[] { "reservation_id", "flight_seat_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservation_detail");
        }
    }
}
