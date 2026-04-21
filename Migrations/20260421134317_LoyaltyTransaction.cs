using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class LoyaltyTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loyalty_transaction",
                columns: table => new
                {
                    loyalty_transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    loyalty_account_id = table.Column<int>(type: "int", nullable: false),
                    ticket_id = table.Column<int>(type: "int", nullable: true),
                    transaction_type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    miles = table.Column<int>(type: "int", nullable: false),
                    transaction_date = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loyalty_transaction", x => x.loyalty_transaction_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loyalty_transaction");
        }
    }
}
