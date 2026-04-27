using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardingPassCodeAndIssuedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "boarding_pass_code",
                table: "boarding_pass",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "issued_at",
                table: "boarding_pass",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.CreateIndex(
                name: "uq_boarding_pass_code",
                table: "boarding_pass",
                column: "boarding_pass_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_boarding_pass_code",
                table: "boarding_pass");

            migrationBuilder.DropColumn(
                name: "boarding_pass_code",
                table: "boarding_pass");

            migrationBuilder.DropColumn(
                name: "issued_at",
                table: "boarding_pass");
        }
    }
}
