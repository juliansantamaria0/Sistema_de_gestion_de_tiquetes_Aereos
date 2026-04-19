namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

// ── MÓDULO 1: Geografía (5 tablas) ──────────────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;

// // ── MÓDULO 2: Aerolíneas y Aeronaves (4 tablas) ─────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Aircraft.Infrastructure.Entity;

// // ── MÓDULO 3: Rutas y Vuelos (5 tablas) ─────────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity;

// // ── MÓDULO 4: Tripulación (4 tablas) ────────────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CrewRole.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Infrastructure.Entity;

// // ── MÓDULO 5: Personas (8 tablas) ───────────────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Infrastructure.Entity;

// // ── MÓDULO 6: Cabina y Asientos (5 tablas) ──────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Infrastructure.Entity;

// // ── MÓDULO 7: Tarifas y Descuentos (5 tablas) ───────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Infrastructure.Entity;

// // ── MÓDULO 8: Reservas (3 tablas) ───────────────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;

// // ── MÓDULO 9: Tiquetes y Equipaje (5 tablas) ────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageType.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Infrastructure.Entity;

// // ── MÓDULO 10: Check-in y Abordaje (3 tablas) ───────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Infrastructure.Entity;

// // ── MÓDULO 11: Pagos y Reembolsos (6 tablas) ────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Currency.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Infrastructure.Entity;

// // ── MÓDULO 12: Incidencias Operativas (4 tablas) ────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Infrastructure.Entity;

// // ── MÓDULO 13: Historial de Estados (3 tablas) ──────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Infrastructure.Entity;

// // ── MÓDULO 14: Fidelización (4 tablas) ──────────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity;
// using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Infrastructure.Entity;

/// <summary>
/// Contexto principal de EF Core.
/// Implementa <see cref="IUnitOfWork"/> para que los servicios de aplicación
/// puedan confirmar transacciones sin depender directamente de EF Core.
/// Convención de plurales: plurales reales en inglés
/// (Countries, Cities, FlightStatuses, CheckInStatuses, BoardingPasses, etc.).
/// </summary>
public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // =========================================================================
    // MÓDULO 1 — Geografía (5 tablas)
    // =========================================================================

    /// <summary>country</summary>
    // public DbSet<CountryEntity> Countries { get; set; } = null!;

    /// <summary>city</summary>
    // public DbSet<CityEntity> Cities { get; set; } = null!;

    // /// <summary>airport</summary>
    // public DbSet<AirportEntity> Airports { get; set; } = null!;

    // /// <summary>terminal</summary>
    // public DbSet<TerminalEntity> Terminals { get; set; } = null!;

    // /// <summary>gate</summary>
    // public DbSet<GateEntity> Gates { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 2 — Aerolíneas y Aeronaves (4 tablas)
    // // =========================================================================

    // /// <summary>airline</summary>
    // public DbSet<AirlineEntity> Airlines { get; set; } = null!;

    // /// <summary>aircraft_manufacturer</summary>
    // public DbSet<AircraftManufacturerEntity> AircraftManufacturers { get; set; } = null!;

    // /// <summary>aircraft_type</summary>
    // public DbSet<AircraftTypeEntity> AircraftTypes { get; set; } = null!;

    // /// <summary>aircraft</summary>
    // public DbSet<AircraftEntity> Aircrafts { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 3 — Rutas y Vuelos (5 tablas)
    // // =========================================================================

    // /// <summary>route</summary>
    // public DbSet<RouteEntity> Routes { get; set; } = null!;

    // /// <summary>flight_status — plural irregular inglés</summary>
    // public DbSet<FlightStatusEntity> FlightStatuses { get; set; } = null!;

    // /// <summary>base_flight</summary>
    // public DbSet<BaseFlightEntity> BaseFlights { get; set; } = null!;

    // /// <summary>route_schedule</summary>
    // public DbSet<RouteScheduleEntity> RouteSchedules { get; set; } = null!;

    // /// <summary>scheduled_flight</summary>
    // public DbSet<ScheduledFlightEntity> ScheduledFlights { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 4 — Tripulación (4 tablas)
    // // =========================================================================

    // /// <summary>crew_role</summary>
    // public DbSet<CrewRoleEntity> CrewRoles { get; set; } = null!;

    // /// <summary>job_position</summary>
    // public DbSet<JobPositionEntity> JobPositions { get; set; } = null!;

    // /// <summary>employee</summary>
    // public DbSet<EmployeeEntity> Employees { get; set; } = null!;

    // /// <summary>flight_crew</summary>
    // public DbSet<FlightCrewEntity> FlightCrews { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 5 — Personas (8 tablas)
    // // =========================================================================

    // /// <summary>document_type</summary>
    // public DbSet<DocumentTypeEntity> DocumentTypes { get; set; } = null!;

    // /// <summary>gender</summary>
    // public DbSet<GenderEntity> Genders { get; set; } = null!;

    // /// <summary>nationality</summary>
    // public DbSet<NationalityEntity> Nationalities { get; set; } = null!;

    // /// <summary>person</summary>
    // public DbSet<PersonEntity> Persons { get; set; } = null!;

    // /// <summary>customer</summary>
    // public DbSet<CustomerEntity> Customers { get; set; } = null!;

    // /// <summary>passenger</summary>
    // public DbSet<PassengerEntity> Passengers { get; set; } = null!;

    // /// <summary>contact_type</summary>
    // public DbSet<ContactTypeEntity> ContactTypes { get; set; } = null!;

    // /// <summary>passenger_contact</summary>
    // public DbSet<PassengerContactEntity> PassengerContacts { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 6 — Cabina y Asientos (5 tablas)
    // // =========================================================================

    // /// <summary>cabin_class</summary>
    // public DbSet<CabinClassEntity> CabinClasses { get; set; } = null!;

    // /// <summary>seat_status</summary>
    // public DbSet<SeatStatusEntity> SeatStatuses { get; set; } = null!;

    // /// <summary>seat_map</summary>
    // public DbSet<SeatMapEntity> SeatMaps { get; set; } = null!;

    // /// <summary>flight_seat</summary>
    // public DbSet<FlightSeatEntity> FlightSeats { get; set; } = null!;

    // /// <summary>flight_cabin_price</summary>
    // public DbSet<FlightCabinPriceEntity> FlightCabinPrices { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 7 — Tarifas y Descuentos (5 tablas)
    // // =========================================================================

    // /// <summary>fare_type</summary>
    // public DbSet<FareTypeEntity> FareTypes { get; set; } = null!;

    // /// <summary>discount_type</summary>
    // public DbSet<DiscountTypeEntity> DiscountTypes { get; set; } = null!;

    // /// <summary>passenger_discount</summary>
    // public DbSet<PassengerDiscountEntity> PassengerDiscounts { get; set; } = null!;

    // /// <summary>promotion</summary>
    // public DbSet<PromotionEntity> Promotions { get; set; } = null!;

    // /// <summary>flight_promotion</summary>
    // public DbSet<FlightPromotionEntity> FlightPromotions { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 8 — Reservas (3 tablas)
    // // =========================================================================

    // /// <summary>reservation_status</summary>
    // public DbSet<ReservationStatusEntity> ReservationStatuses { get; set; } = null!;

    // /// <summary>reservation</summary>
    // public DbSet<ReservationEntity> Reservations { get; set; } = null!;

    // /// <summary>reservation_detail</summary>
    // public DbSet<ReservationDetailEntity> ReservationDetails { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 9 — Tiquetes y Equipaje (5 tablas)
    // // =========================================================================

    // /// <summary>ticket_status</summary>
    // public DbSet<TicketStatusEntity> TicketStatuses { get; set; } = null!;

    // /// <summary>ticket</summary>
    // public DbSet<TicketEntity> Tickets { get; set; } = null!;

    // /// <summary>baggage_allowance</summary>
    // public DbSet<BaggageAllowanceEntity> BaggageAllowances { get; set; } = null!;

    // /// <summary>baggage_type</summary>
    // public DbSet<BaggageTypeEntity> BaggageTypes { get; set; } = null!;

    // /// <summary>ticket_baggage</summary>
    // public DbSet<TicketBaggageEntity> TicketBaggages { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 10 — Check-in y Abordaje (3 tablas)
    // // =========================================================================

    // /// <summary>check_in_status</summary>
    // public DbSet<CheckInStatusEntity> CheckInStatuses { get; set; } = null!;

    // /// <summary>check_in</summary>
    // public DbSet<CheckInEntity> CheckIns { get; set; } = null!;

    // /// <summary>boarding_pass</summary>
    // public DbSet<BoardingPassEntity> BoardingPasses { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 11 — Pagos y Reembolsos (6 tablas)
    // // =========================================================================

    // /// <summary>currency</summary>
    // public DbSet<CurrencyEntity> Currencies { get; set; } = null!;

    // /// <summary>payment_status</summary>
    // public DbSet<PaymentStatusEntity> PaymentStatuses { get; set; } = null!;

    // /// <summary>payment_method</summary>
    // public DbSet<PaymentMethodEntity> PaymentMethods { get; set; } = null!;

    // /// <summary>payment</summary>
    // public DbSet<PaymentEntity> Payments { get; set; } = null!;

    // /// <summary>refund_status</summary>
    // public DbSet<RefundStatusEntity> RefundStatuses { get; set; } = null!;

    // /// <summary>refund</summary>
    // public DbSet<RefundEntity> Refunds { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 12 — Incidencias Operativas (4 tablas)
    // // =========================================================================

    // /// <summary>delay_reason</summary>
    // public DbSet<DelayReasonEntity> DelayReasons { get; set; } = null!;

    // /// <summary>flight_delay</summary>
    // public DbSet<FlightDelayEntity> FlightDelays { get; set; } = null!;

    // /// <summary>cancellation_reason</summary>
    // public DbSet<CancellationReasonEntity> CancellationReasons { get; set; } = null!;

    // /// <summary>flight_cancellation</summary>
    // public DbSet<FlightCancellationEntity> FlightCancellations { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 13 — Historial de Estados (3 tablas)
    // // =========================================================================

    // /// <summary>reservation_status_history</summary>
    // public DbSet<ReservationStatusHistoryEntity> ReservationStatusHistories { get; set; } = null!;

    // /// <summary>ticket_status_history</summary>
    // public DbSet<TicketStatusHistoryEntity> TicketStatusHistories { get; set; } = null!;

    // /// <summary>flight_status_history</summary>
    // public DbSet<FlightStatusHistoryEntity> FlightStatusHistories { get; set; } = null!;

    // // =========================================================================
    // // MÓDULO 14 — Fidelización (4 tablas)
    // // =========================================================================

    // /// <summary>loyalty_program</summary>
    // public DbSet<LoyaltyProgramEntity> LoyaltyPrograms { get; set; } = null!;

    // /// <summary>loyalty_tier</summary>
    // public DbSet<LoyaltyTierEntity> LoyaltyTiers { get; set; } = null!;

    // /// <summary>loyalty_account</summary>
    // public DbSet<LoyaltyAccountEntity> LoyaltyAccounts { get; set; } = null!;

    // /// <summary>loyalty_transaction</summary>
    // public DbSet<LoyaltyTransactionEntity> LoyaltyTransactions { get; set; } = null!;

    // =========================================================================
    // OnModelCreating
    // =========================================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas las configuraciones de entidad definidas
        // en el ensamblado. Cualquier clase que implemente
        // IEntityTypeConfiguration<TEntity> será detectada y aplicada
        // sin necesidad de registrarla individualmente.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    // =========================================================================
    // IUnitOfWork
    // =========================================================================

    /// <inheritdoc />
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await SaveChangesAsync(cancellationToken);
}