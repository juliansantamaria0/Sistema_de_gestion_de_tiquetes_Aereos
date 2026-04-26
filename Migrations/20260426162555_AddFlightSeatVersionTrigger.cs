using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightSeatVersionTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Trigger que genera el valor de version en INSERT
            migrationBuilder.Sql(@"
                CREATE TRIGGER `trg_flight_seat_version_insert`
                BEFORE INSERT ON `flight_seat`
                FOR EACH ROW
                SET NEW.version = UNHEX(REPLACE(UUID(), '-', ''));
            ");

            // Trigger que regenera el valor de version en UPDATE (para concurrencia optimista)
            migrationBuilder.Sql(@"
                CREATE TRIGGER `trg_flight_seat_version_update`
                BEFORE UPDATE ON `flight_seat`
                FOR EACH ROW
                SET NEW.version = UNHEX(REPLACE(UUID(), '-', ''));
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS `trg_flight_seat_version_insert`;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS `trg_flight_seat_version_update`;");
        }
    }
}
