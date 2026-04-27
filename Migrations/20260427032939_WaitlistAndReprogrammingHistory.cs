using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    /// <inheritdoc />
    public partial class WaitlistAndReprogrammingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                SET NAMES utf8mb4;

                CREATE TABLE IF NOT EXISTS historial_reprogramaciones (
                  historial_reprogramacion_id INT NOT NULL AUTO_INCREMENT,
                  reservation_id              INT NOT NULL,
                  vuelo_anterior_id           INT NOT NULL,
                  nuevo_vuelo_id              INT NOT NULL,
                  fecha_cambio                DATETIME(6) NOT NULL,
                  motivo                      VARCHAR(500) NULL,
                  created_at                  DATETIME(6) NOT NULL,
                  PRIMARY KEY (historial_reprogramacion_id),
                  KEY ix_hist_repr_res (reservation_id),
                  KEY ix_hist_repr_vuelos (vuelo_anterior_id, nuevo_vuelo_id),
                  KEY ix_hist_repr_fecha (fecha_cambio),
                  CONSTRAINT fk_hist_repr_res
                    FOREIGN KEY (reservation_id) REFERENCES reservation (reservation_id)
                    ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

                CREATE TABLE IF NOT EXISTS lista_espera (
                  lista_espera_id         INT NOT NULL AUTO_INCREMENT,
                  reservation_id          INT NOT NULL,
                  scheduled_flight_id     INT NOT NULL,
                  passenger_id            INT NOT NULL,
                  fare_type_id            INT NOT NULL,
                  fecha_solicitud         DATETIME(6) NOT NULL,
                  prioridad               INT NOT NULL,
                  estado                  VARCHAR(20) NOT NULL,
                  created_at              DATETIME(6) NOT NULL,
                  updated_at              DATETIME(6) NULL,
                  fecha_promocion         DATETIME(6) NULL,
                  PRIMARY KEY (lista_espera_id),
                  KEY ix_lista_vuelo_estado_fecha (scheduled_flight_id, estado, prioridad, fecha_solicitud, lista_espera_id),
                  KEY ix_lista_reserva (reservation_id),
                  KEY ix_lista_pax (passenger_id),
                  CONSTRAINT fk_lista_res
                    FOREIGN KEY (reservation_id) REFERENCES reservation (reservation_id)
                    ON DELETE CASCADE ON UPDATE CASCADE,
                  CONSTRAINT fk_lista_sf
                    FOREIGN KEY (scheduled_flight_id) REFERENCES scheduled_flight (scheduled_flight_id)
                    ON DELETE CASCADE ON UPDATE CASCADE,
                  CONSTRAINT fk_lista_pax
                    FOREIGN KEY (passenger_id) REFERENCES passenger (passenger_id)
                    ON DELETE RESTRICT ON UPDATE CASCADE,
                  CONSTRAINT fk_lista_fare
                    FOREIGN KEY (fare_type_id) REFERENCES fare_type (fare_type_id)
                    ON DELETE RESTRICT ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TABLE IF EXISTS lista_espera;
                DROP TABLE IF EXISTS historial_reprogramaciones;
                """);
        }
    }
}
