using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    
    public partial class relaciones : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_ticket_status_history_ticket_status_id",
                table: "ticket_status_history",
                column: "ticket_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_baggage_baggage_type_id",
                table: "ticket_baggage",
                column: "baggage_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_ticket_status_id",
                table: "ticket",
                column: "ticket_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_seat_map_cabin_class_id",
                table: "seat_map",
                column: "cabin_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_scheduled_flight_aircraft_id",
                table: "scheduled_flight",
                column: "aircraft_id");

            migrationBuilder.CreateIndex(
                name: "ix_scheduled_flight_flight_status_id",
                table: "scheduled_flight",
                column: "flight_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_scheduled_flight_gate_id",
                table: "scheduled_flight",
                column: "gate_id");

            migrationBuilder.CreateIndex(
                name: "ix_route_destination_airport_id",
                table: "route",
                column: "destination_airport_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_status_history_reservation_status_id",
                table: "reservation_status_history",
                column: "reservation_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_detail_fare_type_id",
                table: "reservation_detail",
                column: "fare_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_detail_flight_seat_id",
                table: "reservation_detail",
                column: "flight_seat_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_detail_passenger_id",
                table: "reservation_detail",
                column: "passenger_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_customer_id",
                table: "reservation",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_reservation_status_id",
                table: "reservation",
                column: "reservation_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservation_scheduled_flight_id",
                table: "reservation",
                column: "scheduled_flight_id");

            migrationBuilder.CreateIndex(
                name: "ix_refund_payment_id",
                table: "refund",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "ix_refund_refund_status_id",
                table: "refund",
                column: "refund_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_promotion_airline_id",
                table: "promotion",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_gender_id",
                table: "person",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_currency_id",
                table: "payment",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_payment_method_id",
                table: "payment",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_payment_status_id",
                table: "payment",
                column: "payment_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_reservation_id",
                table: "payment",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_payment_ticket_id",
                table: "payment",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "ix_passenger_discount_discount_type_id",
                table: "passenger_discount",
                column: "discount_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_passenger_contact_contact_type_id",
                table: "passenger_contact",
                column: "contact_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_passenger_nationality_id",
                table: "passenger",
                column: "nationality_id");

            migrationBuilder.CreateIndex(
                name: "ix_loyalty_transaction_loyalty_account_id",
                table: "loyalty_transaction",
                column: "loyalty_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_loyalty_transaction_ticket_id",
                table: "loyalty_transaction",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "ix_loyalty_account_loyalty_tier_id",
                table: "loyalty_account",
                column: "loyalty_tier_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_status_history_flight_status_id",
                table: "flight_status_history",
                column: "flight_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_seat_seat_map_id",
                table: "flight_seat",
                column: "seat_map_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_seat_seat_status_id",
                table: "flight_seat",
                column: "seat_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_promotion_promotion_id",
                table: "flight_promotion",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_delay_delay_reason_id",
                table: "flight_delay",
                column: "delay_reason_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_delay_scheduled_flight_id",
                table: "flight_delay",
                column: "scheduled_flight_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_crew_crew_role_id",
                table: "flight_crew",
                column: "crew_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_crew_employee_id",
                table: "flight_crew",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_cancellation_cancellation_reason_id",
                table: "flight_cancellation",
                column: "cancellation_reason_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_cabin_price_cabin_class_id",
                table: "flight_cabin_price",
                column: "cabin_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_flight_cabin_price_fare_type_id",
                table: "flight_cabin_price",
                column: "fare_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_airline_id",
                table: "employee",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_job_position_id",
                table: "employee",
                column: "job_position_id");

            migrationBuilder.CreateIndex(
                name: "ix_city_country_id",
                table: "city",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_check_in_check_in_status_id",
                table: "check_in",
                column: "check_in_status_id");

            migrationBuilder.CreateIndex(
                name: "ix_boarding_pass_flight_seat_id",
                table: "boarding_pass",
                column: "flight_seat_id");

            migrationBuilder.CreateIndex(
                name: "ix_boarding_pass_gate_id",
                table: "boarding_pass",
                column: "gate_id");

            migrationBuilder.CreateIndex(
                name: "ix_base_flight_airline_id",
                table: "base_flight",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "ix_base_flight_route_id",
                table: "base_flight",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "ix_baggage_allowance_fare_type_id",
                table: "baggage_allowance",
                column: "fare_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_airport_city_id",
                table: "airport",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "ix_aircraft_manufacturer_country_id",
                table: "aircraft_manufacturer",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_aircraft_aircraft_type_id",
                table: "aircraft",
                column: "aircraft_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_aircraft_airline_id",
                table: "aircraft",
                column: "airline_id");

            migrationBuilder.AddForeignKey(
                name: "fk_aircraft_airline",
                table: "aircraft",
                column: "airline_id",
                principalTable: "airline",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_aircraft_type",
                table: "aircraft",
                column: "aircraft_type_id",
                principalTable: "aircraft_type",
                principalColumn: "aircraft_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_manufacturer_country",
                table: "aircraft_manufacturer",
                column: "country_id",
                principalTable: "country",
                principalColumn: "country_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_atype_manufacturer",
                table: "aircraft_type",
                column: "manufacturer_id",
                principalTable: "aircraft_manufacturer",
                principalColumn: "manufacturer_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_airport_city",
                table: "airport",
                column: "city_id",
                principalTable: "city",
                principalColumn: "city_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ba_cabin",
                table: "baggage_allowance",
                column: "cabin_class_id",
                principalTable: "cabin_class",
                principalColumn: "cabin_class_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ba_fare_type",
                table: "baggage_allowance",
                column: "fare_type_id",
                principalTable: "fare_type",
                principalColumn: "fare_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bf_airline",
                table: "base_flight",
                column: "airline_id",
                principalTable: "airline",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bf_route",
                table: "base_flight",
                column: "route_id",
                principalTable: "route",
                principalColumn: "route_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bp_check_in",
                table: "boarding_pass",
                column: "check_in_id",
                principalTable: "check_in",
                principalColumn: "check_in_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bp_flight_seat",
                table: "boarding_pass",
                column: "flight_seat_id",
                principalTable: "flight_seat",
                principalColumn: "flight_seat_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_bp_gate",
                table: "boarding_pass",
                column: "gate_id",
                principalTable: "gate",
                principalColumn: "gate_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ci_status",
                table: "check_in",
                column: "check_in_status_id",
                principalTable: "check_in_status",
                principalColumn: "check_in_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ci_ticket",
                table: "check_in",
                column: "ticket_id",
                principalTable: "ticket",
                principalColumn: "ticket_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_city_country",
                table: "city",
                column: "country_id",
                principalTable: "country",
                principalColumn: "country_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_customer_person",
                table: "customer",
                column: "person_id",
                principalTable: "person",
                principalColumn: "person_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_airline",
                table: "employee",
                column: "airline_id",
                principalTable: "airline",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_person",
                table: "employee",
                column: "person_id",
                principalTable: "person",
                principalColumn: "person_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employee_position",
                table: "employee",
                column: "job_position_id",
                principalTable: "job_position",
                principalColumn: "job_position_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fcp_cabin",
                table: "flight_cabin_price",
                column: "cabin_class_id",
                principalTable: "cabin_class",
                principalColumn: "cabin_class_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fcp_fare_type",
                table: "flight_cabin_price",
                column: "fare_type_id",
                principalTable: "fare_type",
                principalColumn: "fare_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fcp_flight",
                table: "flight_cabin_price",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fcanc_flight",
                table: "flight_cancellation",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fcanc_reason",
                table: "flight_cancellation",
                column: "cancellation_reason_id",
                principalTable: "cancellation_reason",
                principalColumn: "cancellation_reason_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fc_employee",
                table: "flight_crew",
                column: "employee_id",
                principalTable: "employee",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fc_flight",
                table: "flight_crew",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fc_role",
                table: "flight_crew",
                column: "crew_role_id",
                principalTable: "crew_role",
                principalColumn: "crew_role_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fd_flight",
                table: "flight_delay",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fd_reason",
                table: "flight_delay",
                column: "delay_reason_id",
                principalTable: "delay_reason",
                principalColumn: "delay_reason_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fp_flight",
                table: "flight_promotion",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fp_promotion",
                table: "flight_promotion",
                column: "promotion_id",
                principalTable: "promotion",
                principalColumn: "promotion_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fs_flight",
                table: "flight_seat",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fs_seat_map",
                table: "flight_seat",
                column: "seat_map_id",
                principalTable: "seat_map",
                principalColumn: "seat_map_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fs_status",
                table: "flight_seat",
                column: "seat_status_id",
                principalTable: "seat_status",
                principalColumn: "seat_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fsh_flight",
                table: "flight_status_history",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_fsh_status",
                table: "flight_status_history",
                column: "flight_status_id",
                principalTable: "flight_status",
                principalColumn: "flight_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_gate_terminal",
                table: "gate",
                column: "terminal_id",
                principalTable: "terminal",
                principalColumn: "terminal_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_la_passenger",
                table: "loyalty_account",
                column: "passenger_id",
                principalTable: "passenger",
                principalColumn: "passenger_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_la_program",
                table: "loyalty_account",
                column: "loyalty_program_id",
                principalTable: "loyalty_program",
                principalColumn: "loyalty_program_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_la_tier",
                table: "loyalty_account",
                column: "loyalty_tier_id",
                principalTable: "loyalty_tier",
                principalColumn: "loyalty_tier_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_lp_airline",
                table: "loyalty_program",
                column: "airline_id",
                principalTable: "airline",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_lt_program",
                table: "loyalty_tier",
                column: "loyalty_program_id",
                principalTable: "loyalty_program",
                principalColumn: "loyalty_program_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ltx_account",
                table: "loyalty_transaction",
                column: "loyalty_account_id",
                principalTable: "loyalty_account",
                principalColumn: "loyalty_account_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ltx_ticket",
                table: "loyalty_transaction",
                column: "ticket_id",
                principalTable: "ticket",
                principalColumn: "ticket_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_nationality_country",
                table: "nationality",
                column: "country_id",
                principalTable: "country",
                principalColumn: "country_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_passenger_nationality",
                table: "passenger",
                column: "nationality_id",
                principalTable: "nationality",
                principalColumn: "nationality_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_passenger_person",
                table: "passenger",
                column: "person_id",
                principalTable: "person",
                principalColumn: "person_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pc_contact_type",
                table: "passenger_contact",
                column: "contact_type_id",
                principalTable: "contact_type",
                principalColumn: "contact_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pc_passenger",
                table: "passenger_contact",
                column: "passenger_id",
                principalTable: "passenger",
                principalColumn: "passenger_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_pd_detail",
                table: "passenger_discount",
                column: "reservation_detail_id",
                principalTable: "reservation_detail",
                principalColumn: "reservation_detail_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pd_discount",
                table: "passenger_discount",
                column: "discount_type_id",
                principalTable: "discount_type",
                principalColumn: "discount_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pay_currency",
                table: "payment",
                column: "currency_id",
                principalTable: "currency",
                principalColumn: "currency_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pay_method",
                table: "payment",
                column: "payment_method_id",
                principalTable: "payment_method",
                principalColumn: "payment_method_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pay_reservation",
                table: "payment",
                column: "reservation_id",
                principalTable: "reservation",
                principalColumn: "reservation_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pay_status",
                table: "payment",
                column: "payment_status_id",
                principalTable: "payment_status",
                principalColumn: "payment_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pay_ticket",
                table: "payment",
                column: "ticket_id",
                principalTable: "ticket",
                principalColumn: "ticket_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_person_doctype",
                table: "person",
                column: "document_type_id",
                principalTable: "document_type",
                principalColumn: "document_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_person_gender",
                table: "person",
                column: "gender_id",
                principalTable: "gender",
                principalColumn: "gender_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_promo_airline",
                table: "promotion",
                column: "airline_id",
                principalTable: "airline",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_refund_payment",
                table: "refund",
                column: "payment_id",
                principalTable: "payment",
                principalColumn: "payment_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_refund_status",
                table: "refund",
                column: "refund_status_id",
                principalTable: "refund_status",
                principalColumn: "refund_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_res_customer",
                table: "reservation",
                column: "customer_id",
                principalTable: "customer",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_res_flight",
                table: "reservation",
                column: "scheduled_flight_id",
                principalTable: "scheduled_flight",
                principalColumn: "scheduled_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_res_status",
                table: "reservation",
                column: "reservation_status_id",
                principalTable: "reservation_status",
                principalColumn: "reservation_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rd_fare_type",
                table: "reservation_detail",
                column: "fare_type_id",
                principalTable: "fare_type",
                principalColumn: "fare_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rd_passenger",
                table: "reservation_detail",
                column: "passenger_id",
                principalTable: "passenger",
                principalColumn: "passenger_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rd_reservation",
                table: "reservation_detail",
                column: "reservation_id",
                principalTable: "reservation",
                principalColumn: "reservation_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rd_seat",
                table: "reservation_detail",
                column: "flight_seat_id",
                principalTable: "flight_seat",
                principalColumn: "flight_seat_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rsh_reservation",
                table: "reservation_status_history",
                column: "reservation_id",
                principalTable: "reservation",
                principalColumn: "reservation_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rsh_status",
                table: "reservation_status_history",
                column: "reservation_status_id",
                principalTable: "reservation_status",
                principalColumn: "reservation_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_route_destination",
                table: "route",
                column: "destination_airport_id",
                principalTable: "airport",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_route_origin",
                table: "route",
                column: "origin_airport_id",
                principalTable: "airport",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rs_base_flight",
                table: "route_schedule",
                column: "base_flight_id",
                principalTable: "base_flight",
                principalColumn: "base_flight_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_sf_aircraft",
                table: "scheduled_flight",
                column: "aircraft_id",
                principalTable: "aircraft",
                principalColumn: "aircraft_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sf_base_flight",
                table: "scheduled_flight",
                column: "base_flight_id",
                principalTable: "base_flight",
                principalColumn: "base_flight_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sf_gate",
                table: "scheduled_flight",
                column: "gate_id",
                principalTable: "gate",
                principalColumn: "gate_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sf_status",
                table: "scheduled_flight",
                column: "flight_status_id",
                principalTable: "flight_status",
                principalColumn: "flight_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sm_aircraft_type",
                table: "seat_map",
                column: "aircraft_type_id",
                principalTable: "aircraft_type",
                principalColumn: "aircraft_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sm_cabin_class",
                table: "seat_map",
                column: "cabin_class_id",
                principalTable: "cabin_class",
                principalColumn: "cabin_class_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_terminal_airport",
                table: "terminal",
                column: "airport_id",
                principalTable: "airport",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_detail",
                table: "ticket",
                column: "reservation_detail_id",
                principalTable: "reservation_detail",
                principalColumn: "reservation_detail_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_status",
                table: "ticket",
                column: "ticket_status_id",
                principalTable: "ticket_status",
                principalColumn: "ticket_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_baggage_type",
                table: "ticket_baggage",
                column: "baggage_type_id",
                principalTable: "baggage_type",
                principalColumn: "baggage_type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tb_ticket",
                table: "ticket_baggage",
                column: "ticket_id",
                principalTable: "ticket",
                principalColumn: "ticket_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tsh_status",
                table: "ticket_status_history",
                column: "ticket_status_id",
                principalTable: "ticket_status",
                principalColumn: "ticket_status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tsh_ticket",
                table: "ticket_status_history",
                column: "ticket_id",
                principalTable: "ticket",
                principalColumn: "ticket_id",
                onDelete: ReferentialAction.Restrict);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_aircraft_airline",
                table: "aircraft");

            migrationBuilder.DropForeignKey(
                name: "fk_aircraft_type",
                table: "aircraft");

            migrationBuilder.DropForeignKey(
                name: "fk_manufacturer_country",
                table: "aircraft_manufacturer");

            migrationBuilder.DropForeignKey(
                name: "fk_atype_manufacturer",
                table: "aircraft_type");

            migrationBuilder.DropForeignKey(
                name: "fk_airport_city",
                table: "airport");

            migrationBuilder.DropForeignKey(
                name: "fk_ba_cabin",
                table: "baggage_allowance");

            migrationBuilder.DropForeignKey(
                name: "fk_ba_fare_type",
                table: "baggage_allowance");

            migrationBuilder.DropForeignKey(
                name: "fk_bf_airline",
                table: "base_flight");

            migrationBuilder.DropForeignKey(
                name: "fk_bf_route",
                table: "base_flight");

            migrationBuilder.DropForeignKey(
                name: "fk_bp_check_in",
                table: "boarding_pass");

            migrationBuilder.DropForeignKey(
                name: "fk_bp_flight_seat",
                table: "boarding_pass");

            migrationBuilder.DropForeignKey(
                name: "fk_bp_gate",
                table: "boarding_pass");

            migrationBuilder.DropForeignKey(
                name: "fk_ci_status",
                table: "check_in");

            migrationBuilder.DropForeignKey(
                name: "fk_ci_ticket",
                table: "check_in");

            migrationBuilder.DropForeignKey(
                name: "fk_city_country",
                table: "city");

            migrationBuilder.DropForeignKey(
                name: "fk_customer_person",
                table: "customer");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_airline",
                table: "employee");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_person",
                table: "employee");

            migrationBuilder.DropForeignKey(
                name: "fk_employee_position",
                table: "employee");

            migrationBuilder.DropForeignKey(
                name: "fk_fcp_cabin",
                table: "flight_cabin_price");

            migrationBuilder.DropForeignKey(
                name: "fk_fcp_fare_type",
                table: "flight_cabin_price");

            migrationBuilder.DropForeignKey(
                name: "fk_fcp_flight",
                table: "flight_cabin_price");

            migrationBuilder.DropForeignKey(
                name: "fk_fcanc_flight",
                table: "flight_cancellation");

            migrationBuilder.DropForeignKey(
                name: "fk_fcanc_reason",
                table: "flight_cancellation");

            migrationBuilder.DropForeignKey(
                name: "fk_fc_employee",
                table: "flight_crew");

            migrationBuilder.DropForeignKey(
                name: "fk_fc_flight",
                table: "flight_crew");

            migrationBuilder.DropForeignKey(
                name: "fk_fc_role",
                table: "flight_crew");

            migrationBuilder.DropForeignKey(
                name: "fk_fd_flight",
                table: "flight_delay");

            migrationBuilder.DropForeignKey(
                name: "fk_fd_reason",
                table: "flight_delay");

            migrationBuilder.DropForeignKey(
                name: "fk_fp_flight",
                table: "flight_promotion");

            migrationBuilder.DropForeignKey(
                name: "fk_fp_promotion",
                table: "flight_promotion");

            migrationBuilder.DropForeignKey(
                name: "fk_fs_flight",
                table: "flight_seat");

            migrationBuilder.DropForeignKey(
                name: "fk_fs_seat_map",
                table: "flight_seat");

            migrationBuilder.DropForeignKey(
                name: "fk_fs_status",
                table: "flight_seat");

            migrationBuilder.DropForeignKey(
                name: "fk_fsh_flight",
                table: "flight_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_fsh_status",
                table: "flight_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_gate_terminal",
                table: "gate");

            migrationBuilder.DropForeignKey(
                name: "fk_la_passenger",
                table: "loyalty_account");

            migrationBuilder.DropForeignKey(
                name: "fk_la_program",
                table: "loyalty_account");

            migrationBuilder.DropForeignKey(
                name: "fk_la_tier",
                table: "loyalty_account");

            migrationBuilder.DropForeignKey(
                name: "fk_lp_airline",
                table: "loyalty_program");

            migrationBuilder.DropForeignKey(
                name: "fk_lt_program",
                table: "loyalty_tier");

            migrationBuilder.DropForeignKey(
                name: "fk_ltx_account",
                table: "loyalty_transaction");

            migrationBuilder.DropForeignKey(
                name: "fk_ltx_ticket",
                table: "loyalty_transaction");

            migrationBuilder.DropForeignKey(
                name: "fk_nationality_country",
                table: "nationality");

            migrationBuilder.DropForeignKey(
                name: "fk_passenger_nationality",
                table: "passenger");

            migrationBuilder.DropForeignKey(
                name: "fk_passenger_person",
                table: "passenger");

            migrationBuilder.DropForeignKey(
                name: "fk_pc_contact_type",
                table: "passenger_contact");

            migrationBuilder.DropForeignKey(
                name: "fk_pc_passenger",
                table: "passenger_contact");

            migrationBuilder.DropForeignKey(
                name: "fk_pd_detail",
                table: "passenger_discount");

            migrationBuilder.DropForeignKey(
                name: "fk_pd_discount",
                table: "passenger_discount");

            migrationBuilder.DropForeignKey(
                name: "fk_pay_currency",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "fk_pay_method",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "fk_pay_reservation",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "fk_pay_status",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "fk_pay_ticket",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "fk_person_doctype",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "fk_person_gender",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "fk_promo_airline",
                table: "promotion");

            migrationBuilder.DropForeignKey(
                name: "fk_refund_payment",
                table: "refund");

            migrationBuilder.DropForeignKey(
                name: "fk_refund_status",
                table: "refund");

            migrationBuilder.DropForeignKey(
                name: "fk_res_customer",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_res_flight",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_res_status",
                table: "reservation");

            migrationBuilder.DropForeignKey(
                name: "fk_rd_fare_type",
                table: "reservation_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_rd_passenger",
                table: "reservation_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_rd_reservation",
                table: "reservation_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_rd_seat",
                table: "reservation_detail");

            migrationBuilder.DropForeignKey(
                name: "fk_rsh_reservation",
                table: "reservation_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_rsh_status",
                table: "reservation_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_route_destination",
                table: "route");

            migrationBuilder.DropForeignKey(
                name: "fk_route_origin",
                table: "route");

            migrationBuilder.DropForeignKey(
                name: "fk_rs_base_flight",
                table: "route_schedule");

            migrationBuilder.DropForeignKey(
                name: "fk_sf_aircraft",
                table: "scheduled_flight");

            migrationBuilder.DropForeignKey(
                name: "fk_sf_base_flight",
                table: "scheduled_flight");

            migrationBuilder.DropForeignKey(
                name: "fk_sf_gate",
                table: "scheduled_flight");

            migrationBuilder.DropForeignKey(
                name: "fk_sf_status",
                table: "scheduled_flight");

            migrationBuilder.DropForeignKey(
                name: "fk_sm_aircraft_type",
                table: "seat_map");

            migrationBuilder.DropForeignKey(
                name: "fk_sm_cabin_class",
                table: "seat_map");

            migrationBuilder.DropForeignKey(
                name: "fk_terminal_airport",
                table: "terminal");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_detail",
                table: "ticket");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_status",
                table: "ticket");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_baggage_type",
                table: "ticket_baggage");

            migrationBuilder.DropForeignKey(
                name: "fk_tb_ticket",
                table: "ticket_baggage");

            migrationBuilder.DropForeignKey(
                name: "fk_tsh_status",
                table: "ticket_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_tsh_ticket",
                table: "ticket_status_history");

            migrationBuilder.DropIndex(
                name: "ix_ticket_status_history_ticket_status_id",
                table: "ticket_status_history");

            migrationBuilder.DropIndex(
                name: "ix_ticket_baggage_baggage_type_id",
                table: "ticket_baggage");

            migrationBuilder.DropIndex(
                name: "ix_ticket_ticket_status_id",
                table: "ticket");

            migrationBuilder.DropIndex(
                name: "ix_seat_map_cabin_class_id",
                table: "seat_map");

            migrationBuilder.DropIndex(
                name: "ix_scheduled_flight_aircraft_id",
                table: "scheduled_flight");

            migrationBuilder.DropIndex(
                name: "ix_scheduled_flight_flight_status_id",
                table: "scheduled_flight");

            migrationBuilder.DropIndex(
                name: "ix_scheduled_flight_gate_id",
                table: "scheduled_flight");

            migrationBuilder.DropIndex(
                name: "ix_route_destination_airport_id",
                table: "route");

            migrationBuilder.DropIndex(
                name: "ix_reservation_status_history_reservation_status_id",
                table: "reservation_status_history");

            migrationBuilder.DropIndex(
                name: "ix_reservation_detail_fare_type_id",
                table: "reservation_detail");

            migrationBuilder.DropIndex(
                name: "ix_reservation_detail_flight_seat_id",
                table: "reservation_detail");

            migrationBuilder.DropIndex(
                name: "ix_reservation_detail_passenger_id",
                table: "reservation_detail");

            migrationBuilder.DropIndex(
                name: "ix_reservation_customer_id",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ix_reservation_reservation_status_id",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ix_reservation_scheduled_flight_id",
                table: "reservation");

            migrationBuilder.DropIndex(
                name: "ix_refund_payment_id",
                table: "refund");

            migrationBuilder.DropIndex(
                name: "ix_refund_refund_status_id",
                table: "refund");

            migrationBuilder.DropIndex(
                name: "ix_promotion_airline_id",
                table: "promotion");

            migrationBuilder.DropIndex(
                name: "ix_person_gender_id",
                table: "person");

            migrationBuilder.DropIndex(
                name: "ix_payment_currency_id",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "ix_payment_payment_method_id",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "ix_payment_payment_status_id",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "ix_payment_reservation_id",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "ix_payment_ticket_id",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "ix_passenger_discount_discount_type_id",
                table: "passenger_discount");

            migrationBuilder.DropIndex(
                name: "ix_passenger_contact_contact_type_id",
                table: "passenger_contact");

            migrationBuilder.DropIndex(
                name: "ix_passenger_nationality_id",
                table: "passenger");

            migrationBuilder.DropIndex(
                name: "ix_loyalty_transaction_loyalty_account_id",
                table: "loyalty_transaction");

            migrationBuilder.DropIndex(
                name: "ix_loyalty_transaction_ticket_id",
                table: "loyalty_transaction");

            migrationBuilder.DropIndex(
                name: "ix_loyalty_account_loyalty_tier_id",
                table: "loyalty_account");

            migrationBuilder.DropIndex(
                name: "ix_flight_status_history_flight_status_id",
                table: "flight_status_history");

            migrationBuilder.DropIndex(
                name: "ix_flight_seat_seat_map_id",
                table: "flight_seat");

            migrationBuilder.DropIndex(
                name: "ix_flight_seat_seat_status_id",
                table: "flight_seat");

            migrationBuilder.DropIndex(
                name: "ix_flight_promotion_promotion_id",
                table: "flight_promotion");

            migrationBuilder.DropIndex(
                name: "ix_flight_delay_delay_reason_id",
                table: "flight_delay");

            migrationBuilder.DropIndex(
                name: "ix_flight_delay_scheduled_flight_id",
                table: "flight_delay");

            migrationBuilder.DropIndex(
                name: "ix_flight_crew_crew_role_id",
                table: "flight_crew");

            migrationBuilder.DropIndex(
                name: "ix_flight_crew_employee_id",
                table: "flight_crew");

            migrationBuilder.DropIndex(
                name: "ix_flight_cancellation_cancellation_reason_id",
                table: "flight_cancellation");

            migrationBuilder.DropIndex(
                name: "ix_flight_cabin_price_cabin_class_id",
                table: "flight_cabin_price");

            migrationBuilder.DropIndex(
                name: "ix_flight_cabin_price_fare_type_id",
                table: "flight_cabin_price");

            migrationBuilder.DropIndex(
                name: "ix_employee_airline_id",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "ix_employee_job_position_id",
                table: "employee");

            migrationBuilder.DropIndex(
                name: "ix_city_country_id",
                table: "city");

            migrationBuilder.DropIndex(
                name: "ix_check_in_check_in_status_id",
                table: "check_in");

            migrationBuilder.DropIndex(
                name: "ix_boarding_pass_flight_seat_id",
                table: "boarding_pass");

            migrationBuilder.DropIndex(
                name: "ix_boarding_pass_gate_id",
                table: "boarding_pass");

            migrationBuilder.DropIndex(
                name: "ix_base_flight_airline_id",
                table: "base_flight");

            migrationBuilder.DropIndex(
                name: "ix_base_flight_route_id",
                table: "base_flight");

            migrationBuilder.DropIndex(
                name: "ix_baggage_allowance_fare_type_id",
                table: "baggage_allowance");

            migrationBuilder.DropIndex(
                name: "ix_airport_city_id",
                table: "airport");

            migrationBuilder.DropIndex(
                name: "ix_aircraft_manufacturer_country_id",
                table: "aircraft_manufacturer");

            migrationBuilder.DropIndex(
                name: "ix_aircraft_aircraft_type_id",
                table: "aircraft");

            migrationBuilder.DropIndex(
                name: "ix_aircraft_airline_id",
                table: "aircraft");
        }
    }
}
