using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class LoyaltyAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loyalty_account",
                columns: table => new
                {
                    loyalty_account_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    passenger_id = table.Column<int>(type: "int", nullable: false),
                    loyalty_program_id = table.Column<int>(type: "int", nullable: false),
                    loyalty_tier_id = table.Column<int>(type: "int", nullable: false),
                    total_miles = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    available_miles = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    joined_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loyalty_account", x => x.loyalty_account_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uq_la",
                table: "loyalty_account",
                columns: new[] { "passenger_id", "loyalty_program_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loyalty_account");
        }
    }
}
