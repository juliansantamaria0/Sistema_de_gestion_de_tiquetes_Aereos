using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CrewRole.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Currency.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

internal static class BootstrapDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        var now = DateTime.UtcNow;

        await EnsureStatusesAsync(db, ct);
        await EnsureMasterDataAsync(db, now, ct);
    }

    private static async Task EnsureStatusesAsync(AppDbContext db, CancellationToken ct)
    {
        await SeedSimpleAsync(db, db.Genders, x => x.Name,
            [new GenderEntity { Name = "MASCULINO" }, new GenderEntity { Name = "FEMENINO" }, new GenderEntity { Name = "OTRO" }], ct);

        await SeedSimpleAsync(db, db.DocumentTypes, x => x.Code,
            [
                new DocumentTypeEntity { Code = "CC", Name = "Cédula de ciudadanía" },
                new DocumentTypeEntity { Code = "CE", Name = "Cédula de extranjería" },
                new DocumentTypeEntity { Code = "PA", Name = "Pasaporte" }
            ], ct);

        await SeedSimpleAsync(db, db.ReservationStatuses, x => x.Name,
            [new ReservationStatusEntity { Name = "CREATED" }, new ReservationStatusEntity { Name = "CONFIRMED" }, new ReservationStatusEntity { Name = "CANCELLED" }], ct);

        await SeedSimpleAsync(db, db.TicketStatuses, x => x.Name,
            [new TicketStatusEntity { Name = "ISSUED" }, new TicketStatusEntity { Name = "CANCELLED" }, new TicketStatusEntity { Name = "USED" }], ct);

        await SeedSimpleAsync(db, db.PaymentStatuses, x => x.Name,
            [new PaymentStatusEntity { Name = "PENDING" }, new PaymentStatusEntity { Name = "PAID" }, new PaymentStatusEntity { Name = "REJECTED" }], ct);

        await SeedSimpleAsync(db, db.PaymentMethods, x => x.Name,
            [new PaymentMethodEntity { Name = "CARD" }, new PaymentMethodEntity { Name = "CASH" }, new PaymentMethodEntity { Name = "TRANSFER" }], ct);

        await SeedSimpleAsync(db, db.RefundStatuses, x => x.Name,
            [new RefundStatusEntity { Name = "REQUESTED" }, new RefundStatusEntity { Name = "APPROVED" }, new RefundStatusEntity { Name = "REJECTED" }], ct);

        await SeedSimpleAsync(db, db.FlightStatuses, x => x.Name,
            [new FlightStatusEntity { Name = "SCHEDULED" }, new FlightStatusEntity { Name = "DELAYED" }, new FlightStatusEntity { Name = "CANCELLED" }], ct);

        await SeedSimpleAsync(db, db.SeatStatuses, x => x.Name,
            [new SeatStatusEntity { Name = "AVAILABLE" }, new SeatStatusEntity { Name = "OCCUPIED" }, new SeatStatusEntity { Name = "BLOCKED" }], ct);

        await SeedSimpleAsync(db, db.CheckInStatuses, x => x.Name,
            [new CheckInStatusEntity { Name = "PENDING" }, new CheckInStatusEntity { Name = "CHECKED_IN" }, new CheckInStatusEntity { Name = "BOARDED" }], ct);

        await SeedSimpleAsync(db, db.ContactTypes, x => x.Name,
            [new ContactTypeEntity { Name = "EMAIL" }, new ContactTypeEntity { Name = "PHONE" }, new ContactTypeEntity { Name = "EMERGENCY" }], ct);

        await SeedSimpleAsync(db, db.CrewRoles, x => x.Name,
            [new CrewRoleEntity { Name = "PILOT" }, new CrewRoleEntity { Name = "COPILOT" }, new CrewRoleEntity { Name = "CABIN_CREW" }], ct);

        await SeedSimpleAsync(db, db.JobPositions, x => x.Name,
            [
                new JobPositionEntity { Name = "Piloto", Department = "Operaciones" },
                new JobPositionEntity { Name = "Copiloto", Department = "Operaciones" },
                new JobPositionEntity { Name = "Auxiliar de cabina", Department = "Cabina" }
            ], ct);

        await SeedSimpleAsync(db, db.CabinClasses, x => x.Name,
            [new CabinClassEntity { Name = "ECONOMY" }, new CabinClassEntity { Name = "BUSINESS" }, new CabinClassEntity { Name = "FIRST" }], ct);

        await SeedSimpleAsync(db, db.Currencies, x => x.IsoCode,
            [
                new CurrencyEntity { IsoCode = "COP", Name = "Peso colombiano", Symbol = "$" },
                new CurrencyEntity { IsoCode = "USD", Name = "US Dollar", Symbol = "$" }
            ], ct);

        await SeedSimpleAsync(db, db.FareTypes, x => x.Name,
            [
                new FareTypeEntity { Name = "BASIC", IsRefundable = false, IsChangeable = false, AdvancePurchaseDays = 0, BaggageIncluded = false },
                new FareTypeEntity { Name = "FLEX", IsRefundable = true, IsChangeable = true, AdvancePurchaseDays = 0, BaggageIncluded = true }
            ], ct);
    }

    private static async Task EnsureMasterDataAsync(AppDbContext db, DateTime now, CancellationToken ct)
    {
        if (!await db.Countries.AnyAsync(ct))
        {
            await db.Countries.AddRangeAsync(
            [
                new CountryEntity { Name = "Colombia" },
                new CountryEntity { Name = "Ecuador" }
            ], ct);
            await db.SaveChangesAsync(ct);
        }

        var countries = await db.Countries.AsNoTracking().OrderBy(x => x.Id).Take(2).ToListAsync(ct);

        if (!await db.Cities.AnyAsync(ct) && countries.Count >= 2)
        {
            await db.Cities.AddRangeAsync(
            [
                new CityEntity { Name = "Bogotá", CountryId = countries[0].Id, CreatedAt = now },
                new CityEntity { Name = "Quito", CountryId = countries[1].Id, CreatedAt = now }
            ], ct);
            await db.SaveChangesAsync(ct);
        }

        var cities = await db.Cities.AsNoTracking().OrderBy(x => x.CityId).Take(2).ToListAsync(ct);

        if (!await db.Airports.AnyAsync(ct) && cities.Count >= 2)
        {
            await db.Airports.AddRangeAsync(
            [
                new AirportEntity { IataCode = "BOG", Name = "El Dorado", CityId = cities[0].CityId, CreatedAt = now },
                new AirportEntity { IataCode = "UIO", Name = "Mariscal Sucre", CityId = cities[1].CityId, CreatedAt = now }
            ], ct);
            await db.SaveChangesAsync(ct);
        }

        var airports = await db.Airports.AsNoTracking().OrderBy(x => x.AirportId).Take(2).ToListAsync(ct);

        if (!await db.Terminals.AnyAsync(ct) && airports.Count >= 2)
        {
            await db.Terminals.AddRangeAsync(
            [
                new TerminalEntity { AirportId = airports[0].AirportId, Name = "T1", IsInternational = true, CreatedAt = now },
                new TerminalEntity { AirportId = airports[1].AirportId, Name = "T1", IsInternational = true, CreatedAt = now }
            ], ct);
            await db.SaveChangesAsync(ct);
        }

        var terminals = await db.Terminals.AsNoTracking().OrderBy(x => x.TerminalId).Take(2).ToListAsync(ct);
        if (!await db.Gates.AnyAsync(ct) && terminals.Count >= 2)
        {
            await db.Gates.AddRangeAsync(
            [
                new GateEntity { TerminalId = terminals[0].TerminalId, Code = "A1", IsActive = true },
                new GateEntity { TerminalId = terminals[1].TerminalId, Code = "B1", IsActive = true }
            ], ct);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Airlines.AnyAsync(ct))
        {
            await db.Airlines.AddAsync(new AirlineEntity
            {
                IataCode = "AT",
                Name = "Air Tickets Demo",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = null
            }, ct);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.AircraftManufacturers.AnyAsync(ct) && countries.Count > 0)
        {
            await db.AircraftManufacturers.AddAsync(new AircraftManufacturerEntity
            {
                Name = "Boeing",
                CountryId = countries[0].Id
            }, ct);
            await db.SaveChangesAsync(ct);
        }

        var manufacturer = await db.AircraftManufacturers.AsNoTracking().OrderBy(x => x.ManufacturerId).FirstOrDefaultAsync(ct);
        if (!await db.AircraftTypes.AnyAsync(ct) && manufacturer is not null)
        {
            await db.AircraftTypes.AddAsync(new AircraftTypeEntity
            {
                ManufacturerId = manufacturer.ManufacturerId,
                Model = "737-800",
                TotalSeats = 180,
                CargoCapacityKg = 20000m
            }, ct);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Nationalities.AnyAsync(ct) && countries.Count > 0)
        {
            var rows = new List<NationalityEntity>();
            foreach (var country in countries)
            {
                if (!await db.Nationalities.AnyAsync(x => x.CountryId == country.Id, ct))
                {
                    var demonym = country.Name switch
                    {
                        "Colombia" => "COLOMBIANO",
                        "Ecuador" => "ECUATORIANO",
                        _ => country.Name.ToUpperInvariant()
                    };
                    rows.Add(new NationalityEntity { CountryId = country.Id, Demonym = demonym });
                }
            }

            if (rows.Count > 0)
            {
                await db.Nationalities.AddRangeAsync(rows, ct);
                await db.SaveChangesAsync(ct);
            }
        }
    }

    private static async Task SeedSimpleAsync<TEntity>(AppDbContext db, DbSet<TEntity> set, Func<TEntity, string> keySelector, IEnumerable<TEntity> rows, CancellationToken ct)
        where TEntity : class
    {
        var existing = await set.AsNoTracking().ToListAsync(ct);
        var existingKeys = existing.Select(keySelector).Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var pending = rows.Where(x => !existingKeys.Contains(keySelector(x))).ToList();
        if (pending.Count == 0)
            return;

        await set.AddRangeAsync(pending, ct);
        await db.SaveChangesAsync(ct);
    }
}
