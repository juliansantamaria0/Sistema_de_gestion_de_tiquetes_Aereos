using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    
    public partial class City : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    city_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_city", x => x.city_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uq_city",
                table: "city",
                columns: new[] { "name", "country_id" },
                unique: true);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "city");
        }
    }
}
