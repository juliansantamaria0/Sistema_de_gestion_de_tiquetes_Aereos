namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Aircraft.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CrewRole.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Currency.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Infrastructure.Entity;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Infrastructure.Entity;





using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Infrastructure.Entity;










public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    
    
    

    
    public DbSet<CountryEntity> Countries { get; set; } = null!;

    
    public DbSet<CityEntity> Cities { get; set; } = null!;

    
    public DbSet<AirportEntity> Airports { get; set; } = null!;

    
    public DbSet<TerminalEntity> Terminals { get; set; } = null!;

    
    public DbSet<GateEntity> Gates { get; set; } = null!;

    
    
    

    
    public DbSet<AirlineEntity> Airlines { get; set; } = null!;

    
    public DbSet<AircraftManufacturerEntity> AircraftManufacturers { get; set; } = null!;

    
    public DbSet<AircraftTypeEntity> AircraftTypes { get; set; } = null!;

    
    public DbSet<AircraftEntity> Aircrafts { get; set; } = null!;

    
    
    
    

    
    public DbSet<RouteEntity> Routes { get; set; } = null!;

    
    public DbSet<FlightStatusEntity> FlightStatuses { get; set; } = null!;

    
    public DbSet<BaseFlightEntity> BaseFlights { get; set; } = null!;

    
    public DbSet<RouteScheduleEntity> RouteSchedules { get; set; } = null!;

    
    public DbSet<ScheduledFlightEntity> ScheduledFlights { get; set; } = null!;

    
    
    

    
    public DbSet<CrewRoleEntity> CrewRoles { get; set; } = null!;

    
    public DbSet<JobPositionEntity> JobPositions { get; set; } = null!;

    
    public DbSet<EmployeeEntity> Employees { get; set; } = null!;

    
    public DbSet<FlightCrewEntity> FlightCrews { get; set; } = null!;

    
    
    

    
    public DbSet<DocumentTypeEntity> DocumentTypes { get; set; } = null!;

    
    public DbSet<GenderEntity> Genders { get; set; } = null!;

    
    public DbSet<NationalityEntity> Nationalities { get; set; } = null!;

    
    public DbSet<PersonEntity> Persons { get; set; } = null!;

    
    public DbSet<CustomerEntity> Customers { get; set; } = null!;

    
    public DbSet<PassengerEntity> Passengers { get; set; } = null!;

    
    public DbSet<ContactTypeEntity> ContactTypes { get; set; } = null!;

    
    public DbSet<PassengerContactEntity> PassengerContacts { get; set; } = null!;

    
    
    

    
    public DbSet<CabinClassEntity> CabinClasses { get; set; } = null!;

    
    public DbSet<SeatStatusEntity> SeatStatuses { get; set; } = null!;

    
    public DbSet<SeatMapEntity> SeatMaps { get; set; } = null!;

    
    public DbSet<FlightSeatEntity> FlightSeats { get; set; } = null!;

    
    public DbSet<FlightCabinPriceEntity> FlightCabinPrices { get; set; } = null!;

    
    
    

    
    public DbSet<FareTypeEntity> FareTypes { get; set; } = null!;

    
    public DbSet<DiscountTypeEntity> DiscountTypes { get; set; } = null!;

    
    public DbSet<PassengerDiscountEntity> PassengerDiscounts { get; set; } = null!;

    
    public DbSet<PromotionEntity> Promotions { get; set; } = null!;

    
    public DbSet<FlightPromotionEntity> FlightPromotions { get; set; } = null!;

    
    
    

    
    public DbSet<ReservationStatusEntity> ReservationStatuses { get; set; } = null!;

    
    public DbSet<ReservationEntity> Reservations { get; set; } = null!;

    
    public DbSet<ReservationDetailEntity> ReservationDetails { get; set; } = null!;

    
    
    

    
    public DbSet<TicketStatusEntity> TicketStatuses { get; set; } = null!;

    
    public DbSet<TicketEntity> Tickets { get; set; } = null!;

    
    public DbSet<BaggageAllowanceEntity> BaggageAllowances { get; set; } = null!;

    
    public DbSet<BaggageTypeEntity> BaggageTypes { get; set; } = null!;

    
    public DbSet<TicketBaggageEntity> TicketBaggages { get; set; } = null!;

    
    
    

    
    public DbSet<CheckInStatusEntity> CheckInStatuses { get; set; } = null!;

    
    public DbSet<CheckInEntity> CheckIns { get; set; } = null!;

    
    public DbSet<BoardingPassEntity> BoardingPasses { get; set; } = null!;

    
    
    

    
    public DbSet<CurrencyEntity> Currencies { get; set; } = null!;

    
    public DbSet<PaymentStatusEntity> PaymentStatuses { get; set; } = null!;

    
    public DbSet<PaymentMethodEntity> PaymentMethods { get; set; } = null!;

    
    public DbSet<PaymentEntity> Payments { get; set; } = null!;

    
    public DbSet<RefundStatusEntity> RefundStatuses { get; set; } = null!;

    
    public DbSet<RefundEntity> Refunds { get; set; } = null!;

    
    
    

    
    public DbSet<DelayReasonEntity> DelayReasons { get; set; } = null!;

    
    public DbSet<FlightDelayEntity> FlightDelays { get; set; } = null!;

    
    public DbSet<CancellationReasonEntity> CancellationReasons { get; set; } = null!;

    
    public DbSet<FlightCancellationEntity> FlightCancellations { get; set; } = null!;

    
    
    

    
    public DbSet<ReservationStatusHistoryEntity> ReservationStatusHistories { get; set; } = null!;

    
    public DbSet<TicketStatusHistoryEntity> TicketStatusHistories { get; set; } = null!;

    
    public DbSet<FlightStatusHistoryEntity> FlightStatusHistories { get; set; } = null!;
    

    
    
    

    
    public DbSet<LoyaltyProgramEntity> LoyaltyPrograms { get; set; } = null!;

    
    public DbSet<LoyaltyTierEntity> LoyaltyTiers { get; set; } = null!;

    
    public DbSet<LoyaltyAccountEntity> LoyaltyAccounts { get; set; } = null!;

    
    public DbSet<LoyaltyTransactionEntity> LoyaltyTransactions { get; set; } = null!;

    
    
    

      
       public DbSet<PermissionEntity> Permissions { get; set; } = null!;

        
       public DbSet<RoleEntity> Roles { get; set; } = null!;


          
        public DbSet<RolePermissionEntity> RolePermissions { get; set; } = null!;


    
        public DbSet<UserEntity> Users { get; set; } = null!;

    
    
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        
        
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    
    
    

    
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await SaveChangesAsync(cancellationToken);
}
