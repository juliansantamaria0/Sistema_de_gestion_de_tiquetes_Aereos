// =============================================================
// src/Shared/Infrastructure/DependencyInjection.cs
// Sistema_de_gestion_de_tiquetes_Aereos
// =============================================================
// Versión completa con registro de los 64 módulos.
// =============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

// ── Módulos del dominio ──────────────────────────────────────
// using Sistema_de_gestion_de_tiquetes_Aereos.Aircraft.Application.UseCases;
// using Sistema_de_gestion_de_tiquetes_Aereos.Aircraft.Domain.Interfaces;
// using Sistema_de_gestion_de_tiquetes_Aereos.Aircraft.Infrastructure.Repositories;
// using Sistema_de_gestion_de_tiquetes_Aereos.Aircraft.Application.Services;
// using Sistema_de_gestion_de_tiquetes_Aereos.Aircraft.UI;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

/// <summary>
/// Centraliza el registro de infraestructura compartida y los
/// 64 módulos del dominio (repositorios, use cases, servicios y UIs).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── 1. Cadena de conexión ────────────────────────────────────
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada.");

        var serverVersion = ServerVersion.AutoDetect(connectionString);

        // ── 2. DbContext + Pomelo ────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseMySql(connectionString, serverVersion, mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                })
                .UseSnakeCaseNamingConvention()
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
#endif
            ;
        });

        // ── 3. Unit of Work ──────────────────────────────────────────
        services.AddScoped<IUnitOfWork>(
            sp => sp.GetRequiredService<AppDbContext>());

        // ================================================================
        // ── 4. REGISTRO DE LOS 64 MÓDULOS ───────────────────────────────
        // ================================================================

// ================================================================
// BLOQUE DE REGISTRO DE 64 MÓDULOS — DependencyInjection.cs
// Insertar al final del método AddSharedInfrastructure(),
// justo DESPUÉS de: services.AddScoped<IUnitOfWork>(...)
//
// IMPORTANTE: Agrega los using necesarios por módulo, o usa
// 'global using' en un archivo Usings.cs centralizado.
// ================================================================

        // ── Aircraft ──────────────────────────────────────────
        // services.AddScoped<IAircraftRepository, AircraftRepository>();
        // services.AddScoped<CreateAircraftUseCase>();
        // services.AddScoped<DeleteAircraftUseCase>();
        // services.AddScoped<GetAllAircraftsUseCase>();
        // services.AddScoped<GetAircraftByIdUseCase>();
        // services.AddScoped<UpdateAircraftUseCase>();
        // services.AddScoped<IAircraftService, AircraftService>();
        // services.AddScoped<AircraftConsoleUI>();

        // // ── AircraftManufacturer ──────────────────────────────
        // services.AddScoped<IAircraftManufacturerRepository, AircraftManufacturerRepository>();
        // services.AddScoped<CreateAircraftManufacturerUseCase>();
        // services.AddScoped<DeleteAircraftManufacturerUseCase>();
        // services.AddScoped<GetAllAircraftManufacturersUseCase>();
        // services.AddScoped<GetAircraftManufacturerByIdUseCase>();
        // services.AddScoped<UpdateAircraftManufacturerUseCase>();
        // services.AddScoped<IAircraftManufacturerService, AircraftManufacturerService>();
        // services.AddScoped<AircraftManufacturerConsoleUI>();

        // // ── AircraftType ──────────────────────────────────────
        // services.AddScoped<IAircraftTypeRepository, AircraftTypeRepository>();
        // services.AddScoped<CreateAircraftTypeUseCase>();
        // services.AddScoped<DeleteAircraftTypeUseCase>();
        // services.AddScoped<GetAllAircraftTypesUseCase>();
        // services.AddScoped<GetAircraftTypeByIdUseCase>();
        // services.AddScoped<UpdateAircraftTypeUseCase>();
        // services.AddScoped<IAircraftTypeService, AircraftTypeService>();
        // services.AddScoped<AircraftTypeConsoleUI>();

        // // ── Airline ───────────────────────────────────────────
        // services.AddScoped<IAirlineRepository, AirlineRepository>();
        // services.AddScoped<CreateAirlineUseCase>();
        // services.AddScoped<DeleteAirlineUseCase>();
        // services.AddScoped<GetAllAirlinesUseCase>();
        // services.AddScoped<GetAirlineByIdUseCase>();
        // services.AddScoped<UpdateAirlineUseCase>();
        // services.AddScoped<IAirlineService, AirlineService>();
        // services.AddScoped<AirlineConsoleUI>();

        // // ── Airport ───────────────────────────────────────────
        // services.AddScoped<IAirportRepository, AirportRepository>();
        // services.AddScoped<CreateAirportUseCase>();
        // services.AddScoped<DeleteAirportUseCase>();
        // services.AddScoped<GetAllAirportsUseCase>();
        // services.AddScoped<GetAirportByIdUseCase>();
        // services.AddScoped<UpdateAirportUseCase>();
        // services.AddScoped<IAirportService, AirportService>();
        // services.AddScoped<AirportConsoleUI>();

        // // ── BaggageAllowance ──────────────────────────────────
        // services.AddScoped<IBaggageAllowanceRepository, BaggageAllowanceRepository>();
        // services.AddScoped<CreateBaggageAllowanceUseCase>();
        // services.AddScoped<DeleteBaggageAllowanceUseCase>();
        // services.AddScoped<GetAllBaggageAllowancesUseCase>();
        // services.AddScoped<GetBaggageAllowanceByIdUseCase>();
        // services.AddScoped<UpdateBaggageAllowanceUseCase>();
        // services.AddScoped<IBaggageAllowanceService, BaggageAllowanceService>();
        // services.AddScoped<BaggageAllowanceConsoleUI>();
        // services.AddScoped<GetBaggageAllowanceByCabinAndFareUseCase>();

        // // ── BaggageType ───────────────────────────────────────
        // services.AddScoped<IBaggageTypeRepository, BaggageTypeRepository>();
        // services.AddScoped<CreateBaggageTypeUseCase>();
        // services.AddScoped<DeleteBaggageTypeUseCase>();
        // services.AddScoped<GetAllBaggageTypesUseCase>();
        // services.AddScoped<GetBaggageTypeByIdUseCase>();
        // services.AddScoped<UpdateBaggageTypeUseCase>();
        // services.AddScoped<IBaggageTypeService, BaggageTypeService>();
        // services.AddScoped<BaggageTypeConsoleUI>();

        // // ── BaseFlight ────────────────────────────────────────
        // services.AddScoped<IBaseFlightRepository, BaseFlightRepository>();
        // services.AddScoped<CreateBaseFlightUseCase>();
        // services.AddScoped<DeleteBaseFlightUseCase>();
        // services.AddScoped<GetAllBaseFlightsUseCase>();
        // services.AddScoped<GetBaseFlightByIdUseCase>();
        // services.AddScoped<UpdateBaseFlightUseCase>();
        // services.AddScoped<IBaseFlightService, BaseFlightService>();
        // services.AddScoped<BaseFlightConsoleUI>();

        // // ── BoardingPass ──────────────────────────────────────
        // services.AddScoped<IBoardingPassRepository, BoardingPassRepository>();
        // services.AddScoped<CreateBoardingPassUseCase>();
        // services.AddScoped<DeleteBoardingPassUseCase>();
        // services.AddScoped<GetAllBoardingPassesUseCase>();
        // services.AddScoped<GetBoardingPassByIdUseCase>();
        // services.AddScoped<UpdateBoardingPassUseCase>();
        // services.AddScoped<IBoardingPassService, BoardingPassService>();
        // services.AddScoped<BoardingPassConsoleUI>();
        // services.AddScoped<GetBoardingPassByCheckInUseCase>();

        // // ── CabinClass ────────────────────────────────────────
        // services.AddScoped<ICabinClassRepository, CabinClassRepository>();
        // services.AddScoped<CreateCabinClassUseCase>();
        // services.AddScoped<DeleteCabinClassUseCase>();
        // services.AddScoped<GetAllCabinClassesUseCase>();
        // services.AddScoped<GetCabinClassByIdUseCase>();
        // services.AddScoped<UpdateCabinClassUseCase>();
        // services.AddScoped<ICabinClassService, CabinClassService>();
        // services.AddScoped<CabinClassConsoleUI>();

        // // ── CancellationReason ────────────────────────────────
        // services.AddScoped<ICancellationReasonRepository, CancellationReasonRepository>();
        // services.AddScoped<CreateCancellationReasonUseCase>();
        // services.AddScoped<DeleteCancellationReasonUseCase>();
        // services.AddScoped<GetAllCancellationReasonsUseCase>();
        // services.AddScoped<GetCancellationReasonByIdUseCase>();
        // services.AddScoped<UpdateCancellationReasonUseCase>();
        // services.AddScoped<ICancellationReasonService, CancellationReasonService>();
        // services.AddScoped<CancellationReasonConsoleUI>();

        // // ── CheckIn ───────────────────────────────────────────
        // services.AddScoped<ICheckInRepository, CheckInRepository>();
        // services.AddScoped<CreateCheckInUseCase>();
        // services.AddScoped<DeleteCheckInUseCase>();
        // services.AddScoped<GetAllCheckInsUseCase>();
        // services.AddScoped<GetCheckInByIdUseCase>();
        // services.AddScoped<UpdateCheckInUseCase>();
        // services.AddScoped<ICheckInService, CheckInService>();
        // services.AddScoped<CheckInConsoleUI>();
        // services.AddScoped<GetCheckInByTicketUseCase>();
        // services.AddScoped<ChangeCheckInStatusUseCase>();

        // // ── CheckInStatus ─────────────────────────────────────
        // services.AddScoped<ICheckInStatusRepository, CheckInStatusRepository>();
        // services.AddScoped<CreateCheckInStatusUseCase>();
        // services.AddScoped<DeleteCheckInStatusUseCase>();
        // services.AddScoped<GetAllCheckInStatusesUseCase>();
        // services.AddScoped<GetCheckInStatusByIdUseCase>();
        // services.AddScoped<UpdateCheckInStatusUseCase>();
        // services.AddScoped<ICheckInStatusService, CheckInStatusService>();
        // services.AddScoped<CheckInStatusConsoleUI>();

        // // ── City ──────────────────────────────────────────────
        // services.AddScoped<ICityRepository, CityRepository>();
        // services.AddScoped<CreateCityUseCase>();
        // services.AddScoped<DeleteCityUseCase>();
        // services.AddScoped<GetAllCitiesUseCase>();
        // services.AddScoped<GetCityByIdUseCase>();
        // services.AddScoped<UpdateCityUseCase>();
        // services.AddScoped<ICityService, CityService>();
        // services.AddScoped<CityConsoleUI>();

        // // ── ContactType ───────────────────────────────────────
        // services.AddScoped<IContactTypeRepository, ContactTypeRepository>();
        // services.AddScoped<CreateContactTypeUseCase>();
        // services.AddScoped<DeleteContactTypeUseCase>();
        // services.AddScoped<GetAllContactTypesUseCase>();
        // services.AddScoped<GetContactTypeByIdUseCase>();
        // services.AddScoped<UpdateContactTypeUseCase>();
        // services.AddScoped<IContactTypeService, ContactTypeService>();
        // services.AddScoped<ContactTypeConsoleUI>();

        // // ── Country ───────────────────────────────────────────
        // services.AddScoped<ICountryRepository, CountryRepository>();
        // services.AddScoped<CreateCountryUseCase>();
        // services.AddScoped<DeleteCountryUseCase>();
        // services.AddScoped<GetAllCountriesUseCase>();
        // services.AddScoped<GetCountryByIdUseCase>();
        // services.AddScoped<UpdateCountryUseCase>();
        // services.AddScoped<ICountryService, CountryService>();
        // services.AddScoped<CountryConsoleUI>();

        // // ── CrewRole ──────────────────────────────────────────
        // services.AddScoped<ICrewRoleRepository, CrewRoleRepository>();
        // services.AddScoped<CreateCrewRoleUseCase>();
        // services.AddScoped<DeleteCrewRoleUseCase>();
        // services.AddScoped<GetAllCrewRolesUseCase>();
        // services.AddScoped<GetCrewRoleByIdUseCase>();
        // services.AddScoped<UpdateCrewRoleUseCase>();
        // services.AddScoped<ICrewRoleService, CrewRoleService>();
        // services.AddScoped<CrewRoleConsoleUI>();

        // // ── Currency ──────────────────────────────────────────
        // services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        // services.AddScoped<CreateCurrencyUseCase>();
        // services.AddScoped<DeleteCurrencyUseCase>();
        // services.AddScoped<GetAllCurrenciesUseCase>();
        // services.AddScoped<GetCurrencyByIdUseCase>();
        // services.AddScoped<UpdateCurrencyUseCase>();
        // services.AddScoped<ICurrencyService, CurrencyService>();
        // services.AddScoped<CurrencyConsoleUI>();

        // // ── Customer ──────────────────────────────────────────
        // services.AddScoped<ICustomerRepository, CustomerRepository>();
        // services.AddScoped<CreateCustomerUseCase>();
        // services.AddScoped<DeleteCustomerUseCase>();
        // services.AddScoped<GetAllCustomersUseCase>();
        // services.AddScoped<GetCustomerByIdUseCase>();
        // services.AddScoped<UpdateCustomerUseCase>();
        // services.AddScoped<ICustomerService, CustomerService>();
        // services.AddScoped<CustomerConsoleUI>();
        // services.AddScoped<GetReservationsByCustomerUseCase>();

        // // ── DelayReason ───────────────────────────────────────
        // services.AddScoped<IDelayReasonRepository, DelayReasonRepository>();
        // services.AddScoped<CreateDelayReasonUseCase>();
        // services.AddScoped<DeleteDelayReasonUseCase>();
        // services.AddScoped<GetAllDelayReasonsUseCase>();
        // services.AddScoped<GetDelayReasonByIdUseCase>();
        // services.AddScoped<UpdateDelayReasonUseCase>();
        // services.AddScoped<IDelayReasonService, DelayReasonService>();
        // services.AddScoped<DelayReasonConsoleUI>();

        // // ── DiscountType ──────────────────────────────────────
        // services.AddScoped<IDiscountTypeRepository, DiscountTypeRepository>();
        // services.AddScoped<CreateDiscountTypeUseCase>();
        // services.AddScoped<DeleteDiscountTypeUseCase>();
        // services.AddScoped<GetAllDiscountTypesUseCase>();
        // services.AddScoped<GetDiscountTypeByIdUseCase>();
        // services.AddScoped<UpdateDiscountTypeUseCase>();
        // services.AddScoped<IDiscountTypeService, DiscountTypeService>();
        // services.AddScoped<DiscountTypeConsoleUI>();

        // // ── DocumentType ──────────────────────────────────────
        // services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
        // services.AddScoped<CreateDocumentTypeUseCase>();
        // services.AddScoped<DeleteDocumentTypeUseCase>();
        // services.AddScoped<GetAllDocumentTypesUseCase>();
        // services.AddScoped<GetDocumentTypeByIdUseCase>();
        // services.AddScoped<UpdateDocumentTypeUseCase>();
        // services.AddScoped<IDocumentTypeService, DocumentTypeService>();
        // services.AddScoped<DocumentTypeConsoleUI>();

        // // ── Employee ──────────────────────────────────────────
        // services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        // services.AddScoped<CreateEmployeeUseCase>();
        // services.AddScoped<DeleteEmployeeUseCase>();
        // services.AddScoped<GetAllEmployeesUseCase>();
        // services.AddScoped<GetEmployeeByIdUseCase>();
        // services.AddScoped<UpdateEmployeeUseCase>();
        // services.AddScoped<IEmployeeService, EmployeeService>();
        // services.AddScoped<EmployeeConsoleUI>();

        // // ── FareType ──────────────────────────────────────────
        // services.AddScoped<IFareTypeRepository, FareTypeRepository>();
        // services.AddScoped<CreateFareTypeUseCase>();
        // services.AddScoped<DeleteFareTypeUseCase>();
        // services.AddScoped<GetAllFareTypesUseCase>();
        // services.AddScoped<GetFareTypeByIdUseCase>();
        // services.AddScoped<UpdateFareTypeUseCase>();
        // services.AddScoped<IFareTypeService, FareTypeService>();
        // services.AddScoped<FareTypeConsoleUI>();

        // // ── FlightCabinPrice ──────────────────────────────────
        // services.AddScoped<IFlightCabinPriceRepository, FlightCabinPriceRepository>();
        // services.AddScoped<CreateFlightCabinPriceUseCase>();
        // services.AddScoped<DeleteFlightCabinPriceUseCase>();
        // services.AddScoped<GetAllFlightCabinPricesUseCase>();
        // services.AddScoped<GetFlightCabinPriceByIdUseCase>();
        // services.AddScoped<UpdateFlightCabinPriceUseCase>();
        // services.AddScoped<IFlightCabinPriceService, FlightCabinPriceService>();
        // services.AddScoped<FlightCabinPriceConsoleUI>();
        // services.AddScoped<GetFlightCabinPricesByFlightUseCase>();

        // // ── FlightCancellation ────────────────────────────────
        // services.AddScoped<IFlightCancellationRepository, FlightCancellationRepository>();
        // services.AddScoped<CreateFlightCancellationUseCase>();
        // services.AddScoped<DeleteFlightCancellationUseCase>();
        // services.AddScoped<GetAllFlightCancellationsUseCase>();
        // services.AddScoped<GetFlightCancellationByIdUseCase>();
        // services.AddScoped<UpdateFlightCancellationUseCase>();
        // services.AddScoped<IFlightCancellationService, FlightCancellationService>();
        // services.AddScoped<FlightCancellationConsoleUI>();
        // services.AddScoped<GetFlightCancellationByFlightUseCase>();

        // // ── FlightCrew ────────────────────────────────────────
        // services.AddScoped<IFlightCrewRepository, FlightCrewRepository>();
        // services.AddScoped<CreateFlightCrewUseCase>();
        // services.AddScoped<DeleteFlightCrewUseCase>();
        // services.AddScoped<GetAllFlightCrewsUseCase>();
        // services.AddScoped<GetFlightCrewByIdUseCase>();
        // services.AddScoped<UpdateFlightCrewUseCase>();
        // services.AddScoped<IFlightCrewService, FlightCrewService>();
        // services.AddScoped<FlightCrewConsoleUI>();
        // services.AddScoped<GetFlightCrewByFlightUseCase>();

        // // ── FlightDelay ───────────────────────────────────────
        // services.AddScoped<IFlightDelayRepository, FlightDelayRepository>();
        // services.AddScoped<CreateFlightDelayUseCase>();
        // services.AddScoped<DeleteFlightDelayUseCase>();
        // services.AddScoped<GetAllFlightDelaysUseCase>();
        // services.AddScoped<GetFlightDelayByIdUseCase>();
        // services.AddScoped<UpdateFlightDelayUseCase>();
        // services.AddScoped<IFlightDelayService, FlightDelayService>();
        // services.AddScoped<FlightDelayConsoleUI>();
        // services.AddScoped<GetFlightDelaysByFlightUseCase>();

        // // ── FlightPromotion ───────────────────────────────────
        // services.AddScoped<IFlightPromotionRepository, FlightPromotionRepository>();
        // services.AddScoped<CreateFlightPromotionUseCase>();
        // services.AddScoped<DeleteFlightPromotionUseCase>();
        // services.AddScoped<GetAllFlightPromotionsUseCase>();
        // services.AddScoped<GetFlightPromotionByIdUseCase>();
        // services.AddScoped<UpdateFlightPromotionUseCase>();
        // services.AddScoped<IFlightPromotionService, FlightPromotionService>();
        // services.AddScoped<FlightPromotionConsoleUI>();
        // services.AddScoped<GetFlightPromotionsByFlightUseCase>();
        // services.AddScoped<GetFlightPromotionsByPromotionUseCase>();
        // services.AddScoped<AssignFlightPromotionUseCase>();
        // services.AddScoped<RemoveFlightPromotionUseCase>();

        // // ── FlightSeat ────────────────────────────────────────
        // services.AddScoped<IFlightSeatRepository, FlightSeatRepository>();
        // services.AddScoped<CreateFlightSeatUseCase>();
        // services.AddScoped<DeleteFlightSeatUseCase>();
        // services.AddScoped<GetAllFlightSeatsUseCase>();
        // services.AddScoped<GetFlightSeatByIdUseCase>();
        // services.AddScoped<UpdateFlightSeatUseCase>();
        // services.AddScoped<IFlightSeatService, FlightSeatService>();
        // services.AddScoped<FlightSeatConsoleUI>();
        // services.AddScoped<GetFlightSeatsByFlightUseCase>();
        // services.AddScoped<GetAvailableFlightSeatsByFlightUseCase>();
        // services.AddScoped<ChangeFlightSeatStatusUseCase>();

        // // ── FlightStatus ──────────────────────────────────────
        // services.AddScoped<IFlightStatusRepository, FlightStatusRepository>();
        // services.AddScoped<CreateFlightStatusUseCase>();
        // services.AddScoped<DeleteFlightStatusUseCase>();
        // services.AddScoped<GetAllFlightStatusesUseCase>();
        // services.AddScoped<GetFlightStatusByIdUseCase>();
        // services.AddScoped<UpdateFlightStatusUseCase>();
        // services.AddScoped<IFlightStatusService, FlightStatusService>();
        // services.AddScoped<FlightStatusConsoleUI>();

        // // ── FlightStatusHistory ───────────────────────────────
        // services.AddScoped<IFlightStatusHistoryRepository, FlightStatusHistoryRepository>();
        // services.AddScoped<CreateFlightStatusHistoryUseCase>();
        // services.AddScoped<DeleteFlightStatusHistoryUseCase>();
        // services.AddScoped<GetAllFlightStatusHistoriesUseCase>();
        // services.AddScoped<GetFlightStatusHistoryByIdUseCase>();
        // services.AddScoped<UpdateFlightStatusHistoryUseCase>();
        // services.AddScoped<IFlightStatusHistoryService, FlightStatusHistoryService>();
        // services.AddScoped<FlightStatusHistoryConsoleUI>();

        // // ── Gate ──────────────────────────────────────────────
        // services.AddScoped<IGateRepository, GateRepository>();
        // services.AddScoped<CreateGateUseCase>();
        // services.AddScoped<DeleteGateUseCase>();
        // services.AddScoped<GetAllGatesUseCase>();
        // services.AddScoped<GetGateByIdUseCase>();
        // services.AddScoped<UpdateGateUseCase>();
        // services.AddScoped<IGateService, GateService>();
        // services.AddScoped<GateConsoleUI>();

        // // ── Gender ────────────────────────────────────────────
        // services.AddScoped<IGenderRepository, GenderRepository>();
        // services.AddScoped<CreateGenderUseCase>();
        // services.AddScoped<DeleteGenderUseCase>();
        // services.AddScoped<GetAllGendersUseCase>();
        // services.AddScoped<GetGenderByIdUseCase>();
        // services.AddScoped<UpdateGenderUseCase>();
        // services.AddScoped<IGenderService, GenderService>();
        // services.AddScoped<GenderConsoleUI>();

        // // ── JobPosition ───────────────────────────────────────
        // services.AddScoped<IJobPositionRepository, JobPositionRepository>();
        // services.AddScoped<CreateJobPositionUseCase>();
        // services.AddScoped<DeleteJobPositionUseCase>();
        // services.AddScoped<GetAllJobPositionsUseCase>();
        // services.AddScoped<GetJobPositionByIdUseCase>();
        // services.AddScoped<UpdateJobPositionUseCase>();
        // services.AddScoped<IJobPositionService, JobPositionService>();
        // services.AddScoped<JobPositionConsoleUI>();

        // // ── LoyaltyAccount ────────────────────────────────────
        // services.AddScoped<ILoyaltyAccountRepository, LoyaltyAccountRepository>();
        // services.AddScoped<CreateLoyaltyAccountUseCase>();
        // services.AddScoped<DeleteLoyaltyAccountUseCase>();
        // services.AddScoped<GetAllLoyaltyAccountsUseCase>();
        // services.AddScoped<GetLoyaltyAccountByIdUseCase>();
        // services.AddScoped<UpdateLoyaltyAccountUseCase>();
        // services.AddScoped<ILoyaltyAccountService, LoyaltyAccountService>();
        // services.AddScoped<LoyaltyAccountConsoleUI>();
        // services.AddScoped<GetLoyaltyAccountsByPassengerUseCase>();
        // services.AddScoped<AddMilesUseCase>();
        // services.AddScoped<RedeemMilesUseCase>();

        // // ── LoyaltyProgram ────────────────────────────────────
        // services.AddScoped<ILoyaltyProgramRepository, LoyaltyProgramRepository>();
        // services.AddScoped<CreateLoyaltyProgramUseCase>();
        // services.AddScoped<DeleteLoyaltyProgramUseCase>();
        // services.AddScoped<GetAllLoyaltyProgramsUseCase>();
        // services.AddScoped<GetLoyaltyProgramByIdUseCase>();
        // services.AddScoped<UpdateLoyaltyProgramUseCase>();
        // services.AddScoped<ILoyaltyProgramService, LoyaltyProgramService>();
        // services.AddScoped<LoyaltyProgramConsoleUI>();
        // services.AddScoped<GetLoyaltyProgramByAirlineUseCase>();

        // // ── LoyaltyTier ───────────────────────────────────────
        // services.AddScoped<ILoyaltyTierRepository, LoyaltyTierRepository>();
        // services.AddScoped<CreateLoyaltyTierUseCase>();
        // services.AddScoped<DeleteLoyaltyTierUseCase>();
        // services.AddScoped<GetAllLoyaltyTiersUseCase>();
        // services.AddScoped<GetLoyaltyTierByIdUseCase>();
        // services.AddScoped<UpdateLoyaltyTierUseCase>();
        // services.AddScoped<ILoyaltyTierService, LoyaltyTierService>();
        // services.AddScoped<LoyaltyTierConsoleUI>();
        // services.AddScoped<GetLoyaltyTiersByProgramUseCase>();
        // services.AddScoped<UpgradeTierUseCase>();

        // // ── LoyaltyTransaction ────────────────────────────────
        // services.AddScoped<ILoyaltyTransactionRepository, LoyaltyTransactionRepository>();
        // services.AddScoped<CreateLoyaltyTransactionUseCase>();
        // services.AddScoped<DeleteLoyaltyTransactionUseCase>();
        // services.AddScoped<GetAllLoyaltyTransactionsUseCase>();
        // services.AddScoped<GetLoyaltyTransactionByIdUseCase>();
        // services.AddScoped<UpdateLoyaltyTransactionUseCase>();
        // services.AddScoped<ILoyaltyTransactionService, LoyaltyTransactionService>();
        // services.AddScoped<LoyaltyTransactionConsoleUI>();
        // services.AddScoped<GetLoyaltyTransactionsByAccountUseCase>();
        // services.AddScoped<EarnMilesTransactionUseCase>();
        // services.AddScoped<RedeemMilesTransactionUseCase>();

        // // ── Nationality ───────────────────────────────────────
        // services.AddScoped<INationalityRepository, NationalityRepository>();
        // services.AddScoped<CreateNationalityUseCase>();
        // services.AddScoped<DeleteNationalityUseCase>();
        // services.AddScoped<GetAllNationalitiesUseCase>();
        // services.AddScoped<GetNationalityByIdUseCase>();
        // services.AddScoped<UpdateNationalityUseCase>();
        // services.AddScoped<INationalityService, NationalityService>();
        // services.AddScoped<NationalityConsoleUI>();

        // // ── Passenger ─────────────────────────────────────────
        // services.AddScoped<IPassengerRepository, PassengerRepository>();
        // services.AddScoped<CreatePassengerUseCase>();
        // services.AddScoped<DeletePassengerUseCase>();
        // services.AddScoped<GetAllPassengersUseCase>();
        // services.AddScoped<GetPassengerByIdUseCase>();
        // services.AddScoped<UpdatePassengerUseCase>();
        // services.AddScoped<IPassengerService, PassengerService>();
        // services.AddScoped<PassengerConsoleUI>();
        // services.AddScoped<GetPassengerByPersonUseCase>();

        // // ── PassengerContact ──────────────────────────────────
        // services.AddScoped<IPassengerContactRepository, PassengerContactRepository>();
        // services.AddScoped<CreatePassengerContactUseCase>();
        // services.AddScoped<DeletePassengerContactUseCase>();
        // services.AddScoped<GetAllPassengerContactsUseCase>();
        // services.AddScoped<GetPassengerContactByIdUseCase>();
        // services.AddScoped<UpdatePassengerContactUseCase>();
        // services.AddScoped<IPassengerContactService, PassengerContactService>();
        // services.AddScoped<PassengerContactConsoleUI>();
        // services.AddScoped<GetPassengerContactsByPassengerUseCase>();

        // // ── PassengerDiscount ─────────────────────────────────
        // services.AddScoped<IPassengerDiscountRepository, PassengerDiscountRepository>();
        // services.AddScoped<CreatePassengerDiscountUseCase>();
        // services.AddScoped<DeletePassengerDiscountUseCase>();
        // services.AddScoped<GetAllPassengerDiscountsUseCase>();
        // services.AddScoped<GetPassengerDiscountByIdUseCase>();
        // services.AddScoped<UpdatePassengerDiscountUseCase>();
        // services.AddScoped<IPassengerDiscountService, PassengerDiscountService>();
        // services.AddScoped<PassengerDiscountConsoleUI>();
        // services.AddScoped<GetPassengerDiscountsByDetailUseCase>();

        // // ── Payment ───────────────────────────────────────────
        // services.AddScoped<IPaymentRepository, PaymentRepository>();
        // services.AddScoped<CreatePaymentUseCase>();
        // services.AddScoped<DeletePaymentUseCase>();
        // services.AddScoped<GetAllPaymentsUseCase>();
        // services.AddScoped<GetPaymentByIdUseCase>();
        // services.AddScoped<UpdatePaymentUseCase>();
        // services.AddScoped<IPaymentService, PaymentService>();
        // services.AddScoped<PaymentConsoleUI>();
        // services.AddScoped<GetPaymentsByReservationUseCase>();
        // services.AddScoped<GetPaymentsByTicketUseCase>();

        // // ── PaymentMethod ─────────────────────────────────────
        // services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        // services.AddScoped<CreatePaymentMethodUseCase>();
        // services.AddScoped<DeletePaymentMethodUseCase>();
        // services.AddScoped<GetAllPaymentMethodsUseCase>();
        // services.AddScoped<GetPaymentMethodByIdUseCase>();
        // services.AddScoped<UpdatePaymentMethodUseCase>();
        // services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        // services.AddScoped<PaymentMethodConsoleUI>();

        // // ── PaymentStatus ─────────────────────────────────────
        // services.AddScoped<IPaymentStatusRepository, PaymentStatusRepository>();
        // services.AddScoped<CreatePaymentStatusUseCase>();
        // services.AddScoped<DeletePaymentStatusUseCase>();
        // services.AddScoped<GetAllPaymentStatusesUseCase>();
        // services.AddScoped<GetPaymentStatusByIdUseCase>();
        // services.AddScoped<UpdatePaymentStatusUseCase>();
        // services.AddScoped<IPaymentStatusService, PaymentStatusService>();
        // services.AddScoped<PaymentStatusConsoleUI>();

        // // ── Person ────────────────────────────────────────────
        // services.AddScoped<IPersonRepository, PersonRepository>();
        // services.AddScoped<CreatePersonUseCase>();
        // services.AddScoped<DeletePersonUseCase>();
        // services.AddScoped<GetAllPersonsUseCase>();
        // services.AddScoped<GetPersonByIdUseCase>();
        // services.AddScoped<UpdatePersonUseCase>();
        // services.AddScoped<IPersonService, PersonService>();
        // services.AddScoped<PersonConsoleUI>();
        // services.AddScoped<GetPersonByDocumentUseCase>();

        // // ── Promotion ─────────────────────────────────────────
        // services.AddScoped<IPromotionRepository, PromotionRepository>();
        // services.AddScoped<CreatePromotionUseCase>();
        // services.AddScoped<DeletePromotionUseCase>();
        // services.AddScoped<GetAllPromotionsUseCase>();
        // services.AddScoped<GetPromotionByIdUseCase>();
        // services.AddScoped<UpdatePromotionUseCase>();
        // services.AddScoped<IPromotionService, PromotionService>();
        // services.AddScoped<PromotionConsoleUI>();
        // services.AddScoped<GetPromotionsByAirlineUseCase>();
        // services.AddScoped<GetActivePromotionsUseCase>();

        // // ── Refund ────────────────────────────────────────────
        // services.AddScoped<IRefundRepository, RefundRepository>();
        // services.AddScoped<CreateRefundUseCase>();
        // services.AddScoped<DeleteRefundUseCase>();
        // services.AddScoped<GetAllRefundsUseCase>();
        // services.AddScoped<GetRefundByIdUseCase>();
        // services.AddScoped<UpdateRefundUseCase>();
        // services.AddScoped<IRefundService, RefundService>();
        // services.AddScoped<RefundConsoleUI>();
        // services.AddScoped<GetRefundsByPaymentUseCase>();

        // // ── RefundStatus ──────────────────────────────────────
        // services.AddScoped<IRefundStatusRepository, RefundStatusRepository>();
        // services.AddScoped<CreateRefundStatusUseCase>();
        // services.AddScoped<DeleteRefundStatusUseCase>();
        // services.AddScoped<GetAllRefundStatusesUseCase>();
        // services.AddScoped<GetRefundStatusByIdUseCase>();
        // services.AddScoped<UpdateRefundStatusUseCase>();
        // services.AddScoped<IRefundStatusService, RefundStatusService>();
        // services.AddScoped<RefundStatusConsoleUI>();

        // // ── Reservation ───────────────────────────────────────
        // services.AddScoped<IReservationRepository, ReservationRepository>();
        // services.AddScoped<CreateReservationUseCase>();
        // services.AddScoped<DeleteReservationUseCase>();
        // services.AddScoped<GetAllReservationsUseCase>();
        // services.AddScoped<GetReservationByIdUseCase>();
        // services.AddScoped<UpdateReservationUseCase>();
        // services.AddScoped<IReservationService, ReservationService>();
        // services.AddScoped<ReservationConsoleUI>();
        // services.AddScoped<GetReservationsByFlightUseCase>();
        // services.AddScoped<ConfirmReservationUseCase>();
        // services.AddScoped<CancelReservationUseCase>();

        // // ── ReservationDetail ─────────────────────────────────
        // services.AddScoped<IReservationDetailRepository, ReservationDetailRepository>();
        // services.AddScoped<CreateReservationDetailUseCase>();
        // services.AddScoped<DeleteReservationDetailUseCase>();
        // services.AddScoped<GetAllReservationDetailsUseCase>();
        // services.AddScoped<GetReservationDetailByIdUseCase>();
        // services.AddScoped<UpdateReservationDetailUseCase>();
        // services.AddScoped<IReservationDetailService, ReservationDetailService>();
        // services.AddScoped<ReservationDetailConsoleUI>();
        // services.AddScoped<GetReservationDetailsByReservationUseCase>();

        // // ── ReservationStatus ─────────────────────────────────
        // services.AddScoped<IReservationStatusRepository, ReservationStatusRepository>();
        // services.AddScoped<CreateReservationStatusUseCase>();
        // services.AddScoped<DeleteReservationStatusUseCase>();
        // services.AddScoped<GetAllReservationStatusesUseCase>();
        // services.AddScoped<GetReservationStatusByIdUseCase>();
        // services.AddScoped<UpdateReservationStatusUseCase>();
        // services.AddScoped<IReservationStatusService, ReservationStatusService>();
        // services.AddScoped<ReservationStatusConsoleUI>();

        // // ── ReservationStatusHistory ──────────────────────────
        // services.AddScoped<IReservationStatusHistoryRepository, ReservationStatusHistoryRepository>();
        // services.AddScoped<CreateReservationStatusHistoryUseCase>();
        // services.AddScoped<DeleteReservationStatusHistoryUseCase>();
        // services.AddScoped<GetAllReservationStatusHistoriesUseCase>();
        // services.AddScoped<GetReservationStatusHistoryByIdUseCase>();
        // services.AddScoped<UpdateReservationStatusHistoryUseCase>();
        // services.AddScoped<IReservationStatusHistoryService, ReservationStatusHistoryService>();
        // services.AddScoped<ReservationStatusHistoryConsoleUI>();

        // // ── Route ─────────────────────────────────────────────
        // services.AddScoped<IRouteRepository, RouteRepository>();
        // services.AddScoped<CreateRouteUseCase>();
        // services.AddScoped<DeleteRouteUseCase>();
        // services.AddScoped<GetAllRoutesUseCase>();
        // services.AddScoped<GetRouteByIdUseCase>();
        // services.AddScoped<UpdateRouteUseCase>();
        // services.AddScoped<IRouteService, RouteService>();
        // services.AddScoped<RouteConsoleUI>();
        // services.AddScoped<GetRouteByAirportsUseCase>();
        // services.AddScoped<GetRoutesByOriginUseCase>();

        // // ── RouteSchedule ─────────────────────────────────────
        // services.AddScoped<IRouteScheduleRepository, RouteScheduleRepository>();
        // services.AddScoped<CreateRouteScheduleUseCase>();
        // services.AddScoped<DeleteRouteScheduleUseCase>();
        // services.AddScoped<GetAllRouteSchedulesUseCase>();
        // services.AddScoped<GetRouteScheduleByIdUseCase>();
        // services.AddScoped<UpdateRouteScheduleUseCase>();
        // services.AddScoped<IRouteScheduleService, RouteScheduleService>();
        // services.AddScoped<RouteScheduleConsoleUI>();
        // services.AddScoped<GetRouteSchedulesByBaseFlightUseCase>();

        // // ── ScheduledFlight ───────────────────────────────────
        // services.AddScoped<IScheduledFlightRepository, ScheduledFlightRepository>();
        // services.AddScoped<CreateScheduledFlightUseCase>();
        // services.AddScoped<DeleteScheduledFlightUseCase>();
        // services.AddScoped<GetAllScheduledFlightsUseCase>();
        // services.AddScoped<GetScheduledFlightByIdUseCase>();
        // services.AddScoped<UpdateScheduledFlightUseCase>();
        // services.AddScoped<IScheduledFlightService, ScheduledFlightService>();
        // services.AddScoped<ScheduledFlightConsoleUI>();
        // services.AddScoped<GetScheduledFlightsByBaseFlightUseCase>();
        // services.AddScoped<GetScheduledFlightsByDateUseCase>();

        // // ── SeatMap ───────────────────────────────────────────
        // services.AddScoped<ISeatMapRepository, SeatMapRepository>();
        // services.AddScoped<CreateSeatMapUseCase>();
        // services.AddScoped<DeleteSeatMapUseCase>();
        // services.AddScoped<GetAllSeatMapsUseCase>();
        // services.AddScoped<GetSeatMapByIdUseCase>();
        // services.AddScoped<UpdateSeatMapUseCase>();
        // services.AddScoped<ISeatMapService, SeatMapService>();
        // services.AddScoped<SeatMapConsoleUI>();
        // services.AddScoped<GetSeatMapsByAircraftTypeUseCase>();

        // // ── SeatStatus ────────────────────────────────────────
        // services.AddScoped<ISeatStatusRepository, SeatStatusRepository>();
        // services.AddScoped<CreateSeatStatusUseCase>();
        // services.AddScoped<DeleteSeatStatusUseCase>();
        // services.AddScoped<GetAllSeatStatusesUseCase>();
        // services.AddScoped<GetSeatStatusByIdUseCase>();
        // services.AddScoped<UpdateSeatStatusUseCase>();
        // services.AddScoped<ISeatStatusService, SeatStatusService>();
        // services.AddScoped<SeatStatusConsoleUI>();

        // // ── Terminal ──────────────────────────────────────────
        // services.AddScoped<ITerminalRepository, TerminalRepository>();
        // services.AddScoped<CreateTerminalUseCase>();
        // services.AddScoped<DeleteTerminalUseCase>();
        // services.AddScoped<GetAllTerminalsUseCase>();
        // services.AddScoped<GetTerminalByIdUseCase>();
        // services.AddScoped<UpdateTerminalUseCase>();
        // services.AddScoped<ITerminalService, TerminalService>();
        // services.AddScoped<TerminalConsoleUI>();

        // // ── Ticket ────────────────────────────────────────────
        // services.AddScoped<ITicketRepository, TicketRepository>();
        // services.AddScoped<CreateTicketUseCase>();
        // services.AddScoped<DeleteTicketUseCase>();
        // services.AddScoped<GetAllTicketsUseCase>();
        // services.AddScoped<GetTicketByIdUseCase>();
        // services.AddScoped<UpdateTicketUseCase>();
        // services.AddScoped<ITicketService, TicketService>();
        // services.AddScoped<TicketConsoleUI>();
        // services.AddScoped<GetTicketByReservationDetailUseCase>();
        // services.AddScoped<ChangeTicketStatusUseCase>();

        // // ── TicketBaggage ─────────────────────────────────────
        // services.AddScoped<ITicketBaggageRepository, TicketBaggageRepository>();
        // services.AddScoped<CreateTicketBaggageUseCase>();
        // services.AddScoped<DeleteTicketBaggageUseCase>();
        // services.AddScoped<GetAllTicketBaggagesUseCase>();
        // services.AddScoped<GetTicketBaggageByIdUseCase>();
        // services.AddScoped<UpdateTicketBaggageUseCase>();
        // services.AddScoped<ITicketBaggageService, TicketBaggageService>();
        // services.AddScoped<TicketBaggageConsoleUI>();
        // services.AddScoped<GetTicketBaggagesByTicketUseCase>();

        // // ── TicketStatus ──────────────────────────────────────
        // services.AddScoped<ITicketStatusRepository, TicketStatusRepository>();
        // services.AddScoped<CreateTicketStatusUseCase>();
        // services.AddScoped<DeleteTicketStatusUseCase>();
        // services.AddScoped<GetAllTicketStatusesUseCase>();
        // services.AddScoped<GetTicketStatusByIdUseCase>();
        // services.AddScoped<UpdateTicketStatusUseCase>();
        // services.AddScoped<ITicketStatusService, TicketStatusService>();
        // services.AddScoped<TicketStatusConsoleUI>();

        // // ── TicketStatusHistory ───────────────────────────────
        // services.AddScoped<ITicketStatusHistoryRepository, TicketStatusHistoryRepository>();
        // services.AddScoped<CreateTicketStatusHistoryUseCase>();
        // services.AddScoped<DeleteTicketStatusHistoryUseCase>();
        // services.AddScoped<GetAllTicketStatusHistoriesUseCase>();
        // services.AddScoped<GetTicketStatusHistoryByIdUseCase>();
        // services.AddScoped<UpdateTicketStatusHistoryUseCase>();
        // services.AddScoped<ITicketStatusHistoryService, TicketStatusHistoryService>();
        // services.AddScoped<TicketStatusHistoryConsoleUI>();

        return services;
    }
}

