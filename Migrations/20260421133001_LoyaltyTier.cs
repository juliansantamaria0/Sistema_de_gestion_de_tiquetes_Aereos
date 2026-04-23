using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    
    public partial class LoyaltyTier : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loyalty_tier",
                columns: table => new
                {
                    loyalty_tier_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    loyalty_program_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    min_miles = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    benefits = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_loyalty_tier", x => x.loyalty_tier_id);
                    table.UniqueConstraint("ak_loyalty_tiers_loyalty_program_id_id", x => new { x.loyalty_program_id, x.loyalty_tier_id });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_loyalty_account_loyalty_program_id_loyalty_tier_id",
                table: "loyalty_account",
                columns: new[] { "loyalty_program_id", "loyalty_tier_id" });

            migrationBuilder.CreateIndex(
                name: "uq_lt",
                table: "loyalty_tier",
                columns: new[] { "loyalty_program_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_lt_fk",
                table: "loyalty_tier",
                columns: new[] { "loyalty_program_id", "loyalty_tier_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_account_loyalty_tiers_loyalty_program_id_loyalty_tie",
                table: "loyalty_account",
                columns: new[] { "loyalty_program_id", "loyalty_tier_id" },
                principalTable: "loyalty_tier",
                principalColumns: new[] { "loyalty_program_id", "loyalty_tier_id" },
                onDelete: ReferentialAction.Cascade);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_account_loyalty_tiers_loyalty_program_id_loyalty_tie",
                table: "loyalty_account");

            migrationBuilder.DropTable(
                name: "loyalty_tier");

            migrationBuilder.DropIndex(
                name: "ix_loyalty_account_loyalty_program_id_loyalty_tier_id",
                table: "loyalty_account");
        }
    }
}
