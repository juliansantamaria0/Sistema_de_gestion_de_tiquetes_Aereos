
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

#nullable disable

namespace Sistema_de_gestion_de_tiquetes_Aereos.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260421121024_CabinClass")]
    partial class CabinClass
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Aircraft.Infrastructure.Entity.AircraftEntity", b =>
                {
                    b.Property<int>("AircraftId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("aircraft_id");

                    b.Property<int>("AircraftTypeId")
                        .HasColumnType("int")
                        .HasColumnName("aircraft_type_id");

                    b.Property<int>("AirlineId")
                        .HasColumnType("int")
                        .HasColumnName("airline_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true)
                        .HasColumnName("is_active");

                    b.Property<int>("ManufactureYear")
                        .HasColumnType("int")
                        .HasColumnName("manufacture_year");

                    b.Property<string>("RegistrationNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("registration_number");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("AircraftId")
                        .HasName("pk_aircraft");

                    b.HasIndex("RegistrationNumber")
                        .IsUnique()
                        .HasDatabaseName("ix_aircraft_registration_number");

                    b.ToTable("aircraft", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.Infrastructure.Entity.AircraftManufacturerEntity", b =>
                {
                    b.Property<int>("ManufacturerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("manufacturer_id");

                    b.Property<int>("CountryId")
                        .HasColumnType("int")
                        .HasColumnName("country_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.HasKey("ManufacturerId")
                        .HasName("pk_aircraft_manufacturer");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_aircraft_manufacturer_name");

                    b.ToTable("aircraft_manufacturer", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Infrastructure.Entity.AircraftTypeEntity", b =>
                {
                    b.Property<int>("AircraftTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("aircraft_type_id");

                    b.Property<decimal>("CargoCapacityKg")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("cargo_capacity_kg");

                    b.Property<int>("ManufacturerId")
                        .HasColumnType("int")
                        .HasColumnName("manufacturer_id");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("model");

                    b.Property<int>("TotalSeats")
                        .HasColumnType("int")
                        .HasColumnName("total_seats");

                    b.HasKey("AircraftTypeId")
                        .HasName("pk_aircraft_type");

                    b.HasIndex("ManufacturerId", "Model")
                        .IsUnique()
                        .HasDatabaseName("uq_aircraft_type");

                    b.ToTable("aircraft_type", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity.AirlineEntity", b =>
                {
                    b.Property<int>("AirlineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("airline_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("IataCode")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("char(2)")
                        .HasColumnName("iata_code")
                        .IsFixedLength();

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true)
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("varchar(120)")
                        .HasColumnName("name");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("AirlineId")
                        .HasName("pk_airline");

                    b.HasIndex("IataCode")
                        .IsUnique()
                        .HasDatabaseName("ix_airline_iata_code");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_airline_name");

                    b.ToTable("airline", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Infrastructure.Entity.AirportEntity", b =>
                {
                    b.Property<int>("AirportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("airport_id");

                    b.Property<int>("CityId")
                        .HasColumnType("int")
                        .HasColumnName("city_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("IataCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("char(3)")
                        .HasColumnName("iata_code")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)")
                        .HasColumnName("name");

                    b.HasKey("AirportId")
                        .HasName("pk_airport");

                    b.HasIndex("IataCode")
                        .IsUnique()
                        .HasDatabaseName("ix_airport_iata_code");

                    b.ToTable("airport", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Infrastructure.Entity.BaggageAllowanceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("baggage_allowance_id");

                    b.Property<int>("CabinClassId")
                        .HasColumnType("int")
                        .HasColumnName("cabin_class_id");

                    b.Property<decimal>("CarryOnKg")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(5,2)")
                        .HasDefaultValue(10m)
                        .HasColumnName("carry_on_kg");

                    b.Property<int>("CarryOnPieces")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1)
                        .HasColumnName("carry_on_pieces");

                    b.Property<decimal>("CheckedKg")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(5,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("checked_kg");

                    b.Property<int>("CheckedPieces")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("checked_pieces");

                    b.Property<int>("FareTypeId")
                        .HasColumnType("int")
                        .HasColumnName("fare_type_id");

                    b.HasKey("Id")
                        .HasName("pk_baggage_allowance");

                    b.HasIndex("CabinClassId", "FareTypeId")
                        .IsUnique()
                        .HasDatabaseName("uq_ba");

                    b.ToTable("baggage_allowance", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageType.Infrastructure.Entity.BaggageTypeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("baggage_type_id");

                    b.Property<decimal>("ExtraFee")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("extra_fee");

                    b.Property<decimal>("MaxWeightKg")
                        .HasColumnType("decimal(5,2)")
                        .HasColumnName("max_weight_kg");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_baggage_type");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_baggage_type_name");

                    b.ToTable("baggage_type", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.Infrastructure.Entity.BaseFlightEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("base_flight_id");

                    b.Property<int>("AirlineId")
                        .HasColumnType("int")
                        .HasColumnName("airline_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("FlightCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("flight_code");

                    b.Property<int>("RouteId")
                        .HasColumnType("int")
                        .HasColumnName("route_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_base_flight");

                    b.HasIndex("FlightCode", "AirlineId")
                        .IsUnique()
                        .HasDatabaseName("uq_base_flight");

                    b.ToTable("base_flight", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Infrastructure.Entity.BoardingPassEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("boarding_pass_id");

                    b.Property<string>("BoardingGroup")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("boarding_group");

                    b.Property<int>("CheckInId")
                        .HasColumnType("int")
                        .HasColumnName("check_in_id");

                    b.Property<int>("FlightSeatId")
                        .HasColumnType("int")
                        .HasColumnName("flight_seat_id");

                    b.Property<int?>("GateId")
                        .HasColumnType("int")
                        .HasColumnName("gate_id");

                    b.HasKey("Id")
                        .HasName("pk_boarding_pass");

                    b.HasIndex("CheckInId")
                        .IsUnique()
                        .HasDatabaseName("uq_boarding_pass_check_in");

                    b.ToTable("boarding_pass", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity.CabinClassEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("cabin_class_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_cabin_class");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_cabin_class_name");

                    b.ToTable("cabin_class", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Infrastructure.Entity.CancellationReasonEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("cancellation_reason_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_cancellation_reason");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_cancellation_reason_name");

                    b.ToTable("cancellation_reason", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Infrastructure.Entity.CheckInEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("check_in_id");

                    b.Property<int>("CheckInStatusId")
                        .HasColumnType("int")
                        .HasColumnName("check_in_status_id");

                    b.Property<DateTime>("CheckInTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("check_in_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("CounterNumber")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("counter_number");

                    b.Property<int>("TicketId")
                        .HasColumnType("int")
                        .HasColumnName("ticket_id");

                    b.HasKey("Id")
                        .HasName("pk_check_in");

                    b.HasIndex("TicketId")
                        .IsUnique()
                        .HasDatabaseName("uq_check_in_ticket");

                    b.ToTable("check_in", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.Infrastructure.Entity.CheckInStatusEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("check_in_status_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_check_in_status");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_check_in_status_name");

                    b.ToTable("check_in_status", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Infrastructure.Entity.CityEntity", b =>
                {
                    b.Property<int>("CityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("city_id");

                    b.Property<int>("CountryId")
                        .HasColumnType("int")
                        .HasColumnName("country_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.HasKey("CityId")
                        .HasName("pk_city");

                    b.HasIndex("Name", "CountryId")
                        .IsUnique()
                        .HasDatabaseName("uq_city");

                    b.ToTable("city", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Infrastructure.Entity.CountryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("country_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_country");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_country_name");

                    b.ToTable("country", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.CrewRole.Infrastructure.Entity.CrewRoleEntity", b =>
                {
                    b.Property<int>("CrewRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("crew_role_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.HasKey("CrewRoleId")
                        .HasName("pk_crew_role");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_crew_role_name");

                    b.ToTable("crew_role", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Entity.DelayReasonEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("delay_reason_id");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("category");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_delay_reason");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_delay_reason_name");

                    b.ToTable("delay_reason", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Infrastructure.Entity.EmployeeEntity", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("employee_id");

                    b.Property<int>("AirlineId")
                        .HasColumnType("int")
                        .HasColumnName("airline_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<DateOnly>("HireDate")
                        .HasColumnType("date")
                        .HasColumnName("hire_date");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true)
                        .HasColumnName("is_active");

                    b.Property<int?>("JobPositionId")
                        .HasColumnType("int")
                        .HasColumnName("job_position_id");

                    b.Property<int>("PersonId")
                        .HasColumnType("int")
                        .HasColumnName("person_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("EmployeeId")
                        .HasName("pk_employee");

                    b.HasIndex("PersonId")
                        .IsUnique()
                        .HasDatabaseName("ix_employee_person_id");

                    b.ToTable("employee", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Infrastructure.Entity.FlightCancellationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("cancellation_id");

                    b.Property<int>("CancellationReasonId")
                        .HasColumnType("int")
                        .HasColumnName("cancellation_reason_id");

                    b.Property<DateTime>("CancelledAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("cancelled_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("Notes")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)")
                        .HasColumnName("notes");

                    b.Property<int>("ScheduledFlightId")
                        .HasColumnType("int")
                        .HasColumnName("scheduled_flight_id");

                    b.HasKey("Id")
                        .HasName("pk_flight_cancellation");

                    b.HasIndex("ScheduledFlightId")
                        .IsUnique()
                        .HasDatabaseName("uq_flight_cancellation_flight");

                    b.ToTable("flight_cancellation", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Infrastructure.Entity.FlightCrewEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("flight_crew_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<int>("CrewRoleId")
                        .HasColumnType("int")
                        .HasColumnName("crew_role_id");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int")
                        .HasColumnName("employee_id");

                    b.Property<int>("ScheduledFlightId")
                        .HasColumnType("int")
                        .HasColumnName("scheduled_flight_id");

                    b.HasKey("Id")
                        .HasName("pk_flight_crew");

                    b.HasIndex("ScheduledFlightId", "EmployeeId")
                        .IsUnique()
                        .HasDatabaseName("uq_fc_employee");

                    b.ToTable("flight_crew", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Entity.FlightDelayEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("flight_delay_id");

                    b.Property<int>("DelayMinutes")
                        .HasColumnType("int")
                        .HasColumnName("delay_minutes");

                    b.Property<int>("DelayReasonId")
                        .HasColumnType("int")
                        .HasColumnName("delay_reason_id");

                    b.Property<DateTime>("ReportedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("reported_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<int>("ScheduledFlightId")
                        .HasColumnType("int")
                        .HasColumnName("scheduled_flight_id");

                    b.HasKey("Id")
                        .HasName("pk_flight_delay");

                    b.ToTable("flight_delay", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity.FlightSeatEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("flight_seat_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<int>("ScheduledFlightId")
                        .HasColumnType("int")
                        .HasColumnName("scheduled_flight_id");

                    b.Property<int>("SeatMapId")
                        .HasColumnType("int")
                        .HasColumnName("seat_map_id");

                    b.Property<int>("SeatStatusId")
                        .HasColumnType("int")
                        .HasColumnName("seat_status_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_flight_seat");

                    b.HasIndex("ScheduledFlightId", "SeatMapId")
                        .IsUnique()
                        .HasDatabaseName("uq_flight_seat");

                    b.ToTable("flight_seat", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Infrastructure.Entity.FlightStatusEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("flight_status_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_flight_status");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_flight_status_name");

                    b.ToTable("flight_status", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Infrastructure.Entity.FlightStatusHistoryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("flight_status_history_id");

                    b.Property<DateTime>("ChangedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("changed_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<int>("FlightStatusId")
                        .HasColumnType("int")
                        .HasColumnName("flight_status_id");

                    b.Property<string>("Notes")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)")
                        .HasColumnName("notes");

                    b.Property<int>("ScheduledFlightId")
                        .HasColumnType("int")
                        .HasColumnName("scheduled_flight_id");

                    b.HasKey("Id")
                        .HasName("pk_flight_status_history");

                    b.HasIndex("ScheduledFlightId", "FlightStatusId", "ChangedAt")
                        .IsUnique()
                        .HasDatabaseName("uq_fsh");

                    b.ToTable("flight_status_history", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity.GateEntity", b =>
                {
                    b.Property<int>("GateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("gate_id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("code");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true)
                        .HasColumnName("is_active");

                    b.Property<int>("TerminalId")
                        .HasColumnType("int")
                        .HasColumnName("terminal_id");

                    b.HasKey("GateId")
                        .HasName("pk_gate");

                    b.HasIndex("TerminalId", "Code")
                        .IsUnique()
                        .HasDatabaseName("uq_gate");

                    b.ToTable("gate", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity.JobPositionEntity", b =>
                {
                    b.Property<int>("JobPositionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("job_position_id");

                    b.Property<string>("Department")
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("department");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("name");

                    b.HasKey("JobPositionId")
                        .HasName("pk_job_position");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_job_position_name");

                    b.ToTable("job_position", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity.ReservationStatusHistoryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("reservation_status_history_id");

                    b.Property<DateTime>("ChangedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("changed_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("Notes")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)")
                        .HasColumnName("notes");

                    b.Property<int>("ReservationId")
                        .HasColumnType("int")
                        .HasColumnName("reservation_id");

                    b.Property<int>("ReservationStatusId")
                        .HasColumnType("int")
                        .HasColumnName("reservation_status_id");

                    b.HasKey("Id")
                        .HasName("pk_reservation_status_history");

                    b.HasIndex("ReservationId", "ReservationStatusId", "ChangedAt")
                        .IsUnique()
                        .HasDatabaseName("uq_rsh");

                    b.ToTable("reservation_status_history", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity.RouteEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("route_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<int>("DestinationAirportId")
                        .HasColumnType("int")
                        .HasColumnName("destination_airport_id");

                    b.Property<int>("OriginAirportId")
                        .HasColumnType("int")
                        .HasColumnName("origin_airport_id");

                    b.HasKey("Id")
                        .HasName("pk_route");

                    b.HasIndex("OriginAirportId", "DestinationAirportId")
                        .IsUnique()
                        .HasDatabaseName("uq_route");

                    b.ToTable("route", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Infrastructure.Entity.RouteScheduleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("route_schedule_id");

                    b.Property<int>("BaseFlightId")
                        .HasColumnType("int")
                        .HasColumnName("base_flight_id");

                    b.Property<byte>("DayOfWeek")
                        .HasColumnType("tinyint unsigned")
                        .HasColumnName("day_of_week");

                    b.Property<TimeOnly>("DepartureTime")
                        .HasColumnType("time(6)")
                        .HasColumnName("departure_time");

                    b.HasKey("Id")
                        .HasName("pk_route_schedule");

                    b.HasIndex("BaseFlightId", "DayOfWeek", "DepartureTime")
                        .IsUnique()
                        .HasDatabaseName("uq_rs");

                    b.ToTable("route_schedule", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity.ScheduledFlightEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("scheduled_flight_id");

                    b.Property<int>("AircraftId")
                        .HasColumnType("int")
                        .HasColumnName("aircraft_id");

                    b.Property<int>("BaseFlightId")
                        .HasColumnType("int")
                        .HasColumnName("base_flight_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<DateOnly>("DepartureDate")
                        .HasColumnType("date")
                        .HasColumnName("departure_date");

                    b.Property<TimeOnly>("DepartureTime")
                        .HasColumnType("time(6)")
                        .HasColumnName("departure_time");

                    b.Property<DateTime>("EstimatedArrivalDatetime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("estimated_arrival_datetime");

                    b.Property<int>("FlightStatusId")
                        .HasColumnType("int")
                        .HasColumnName("flight_status_id");

                    b.Property<int?>("GateId")
                        .HasColumnType("int")
                        .HasColumnName("gate_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_scheduled_flight");

                    b.HasIndex("BaseFlightId", "DepartureDate", "DepartureTime")
                        .IsUnique()
                        .HasDatabaseName("uq_sf");

                    b.ToTable("scheduled_flight", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity.SeatMapEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("seat_map_id");

                    b.Property<int>("AircraftTypeId")
                        .HasColumnType("int")
                        .HasColumnName("aircraft_type_id");

                    b.Property<int>("CabinClassId")
                        .HasColumnType("int")
                        .HasColumnName("cabin_class_id");

                    b.Property<string>("SeatFeatures")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("seat_features");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("seat_number");

                    b.HasKey("Id")
                        .HasName("pk_seat_map");

                    b.HasIndex("AircraftTypeId", "SeatNumber")
                        .IsUnique()
                        .HasDatabaseName("uq_seat_map");

                    b.ToTable("seat_map", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Entity.SeatStatusEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("seat_status_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_seat_status");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("uq_seat_status_name");

                    b.ToTable("seat_status", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity.TerminalEntity", b =>
                {
                    b.Property<int>("TerminalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("terminal_id");

                    b.Property<int>("AirportId")
                        .HasColumnType("int")
                        .HasColumnName("airport_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<bool>("IsInternational")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false)
                        .HasColumnName("is_international");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.HasKey("TerminalId")
                        .HasName("pk_terminal");

                    b.HasIndex("AirportId", "Name")
                        .IsUnique()
                        .HasDatabaseName("uq_terminal");

                    b.ToTable("terminal", (string)null);
                });

            modelBuilder.Entity("Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity.TicketStatusHistoryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ticket_status_history_id");

                    b.Property<DateTime>("ChangedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("changed_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                    b.Property<string>("Notes")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)")
                        .HasColumnName("notes");

                    b.Property<int>("TicketId")
                        .HasColumnType("int")
                        .HasColumnName("ticket_id");

                    b.Property<int>("TicketStatusId")
                        .HasColumnType("int")
                        .HasColumnName("ticket_status_id");

                    b.HasKey("Id")
                        .HasName("pk_ticket_status_history");

                    b.HasIndex("TicketId", "TicketStatusId", "ChangedAt")
                        .IsUnique()
                        .HasDatabaseName("uq_tsh");

                    b.ToTable("ticket_status_history", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
