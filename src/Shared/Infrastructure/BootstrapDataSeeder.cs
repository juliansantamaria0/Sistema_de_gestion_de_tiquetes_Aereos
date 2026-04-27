using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airport.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckInStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CrewRole.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Currency.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Infrastructure.Entity;
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
        // Reparación idempotente: datos viejos podían quedar con confirmed_at y cancelled_at a la vez.
        // Eso rompe invariantes del dominio al cargar reservas.
        await db.Database.ExecuteSqlRawAsync(
            "UPDATE reservation SET confirmed_at = NULL WHERE cancelled_at IS NOT NULL AND confirmed_at IS NOT NULL;",
            ct);
        var now = DateTime.UtcNow;
        await EnsureStatusesAsync(db, ct);
        await EnsureMasterDataAsync(db, now, ct);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  CATÁLOGOS SIMPLES  (idempotente: sólo agrega los que falten)
    // ─────────────────────────────────────────────────────────────────────────
    private static async Task EnsureStatusesAsync(AppDbContext db, CancellationToken ct)
    {
        // Roles del sistema (Administrador = 1, Cliente = 2 — coincide con AuthService)
        await SeedSimpleAsync(db, db.Roles, x => x.Name,
        [
            new RoleEntity { Name = "Administrador", IsActive = true },
            new RoleEntity { Name = "Cliente",       IsActive = true },
        ], ct);

        // Géneros
        await SeedSimpleAsync(db, db.Genders, x => x.Name,
        [
            new GenderEntity { Name = "MASCULINO" },
            new GenderEntity { Name = "FEMENINO"  },
            new GenderEntity { Name = "OTRO"      },
        ], ct);

        // Tipos de documento
        await SeedSimpleAsync(db, db.DocumentTypes, x => x.Code,
        [
            new DocumentTypeEntity { Code = "CC",  Name = "Cédula de ciudadanía"              },
            new DocumentTypeEntity { Code = "CE",  Name = "Cédula de extranjería"             },
            new DocumentTypeEntity { Code = "PA",  Name = "Pasaporte"                         },
            new DocumentTypeEntity { Code = "TI",  Name = "Tarjeta de identidad"              },
            new DocumentTypeEntity { Code = "DNI", Name = "Documento Nacional de Identidad"   },
        ], ct);

        // Estados de reserva
        await SeedSimpleAsync(db, db.ReservationStatuses, x => x.Name,
        [
            new ReservationStatusEntity { Name = "CREATED"   },
            new ReservationStatusEntity { Name = "CONFIRMED" },
            new ReservationStatusEntity { Name = "WAITLIST"  },
            new ReservationStatusEntity { Name = "CANCELLED" },
        ], ct);

        // Estados de tiquete
        await SeedSimpleAsync(db, db.TicketStatuses, x => x.Name,
        [
            new TicketStatusEntity { Name = "ISSUED"    },
            new TicketStatusEntity { Name = "CANCELLED" },
            new TicketStatusEntity { Name = "USED"      },
        ], ct);

        // Estados de pago
        await SeedSimpleAsync(db, db.PaymentStatuses, x => x.Name,
        [
            new PaymentStatusEntity { Name = "PENDING"  },
            new PaymentStatusEntity { Name = "PAID"     },
            new PaymentStatusEntity { Name = "REJECTED" },
        ], ct);

        // Métodos de pago reales
        await SeedSimpleAsync(db, db.PaymentMethods, x => x.Name,
        [
            new PaymentMethodEntity { Name = "Tarjeta de crédito"    },
            new PaymentMethodEntity { Name = "Tarjeta de débito"     },
            new PaymentMethodEntity { Name = "Efectivo"              },
            new PaymentMethodEntity { Name = "Transferencia bancaria"},
            new PaymentMethodEntity { Name = "PSE"                   },
            new PaymentMethodEntity { Name = "Nequi"                 },
            new PaymentMethodEntity { Name = "Daviplata"             },
        ], ct);

        // Estados de reembolso
        await SeedSimpleAsync(db, db.RefundStatuses, x => x.Name,
        [
            new RefundStatusEntity { Name = "REQUESTED" },
            new RefundStatusEntity { Name = "APPROVED"  },
            new RefundStatusEntity { Name = "REJECTED"  },
        ], ct);

        // Estados de vuelo
        await SeedSimpleAsync(db, db.FlightStatuses, x => x.Name,
        [
            new FlightStatusEntity { Name = "SCHEDULED" },
            new FlightStatusEntity { Name = "BOARDING"  },
            new FlightStatusEntity { Name = "DEPARTED"  },
            new FlightStatusEntity { Name = "DELAYED"   },
            new FlightStatusEntity { Name = "CANCELLED" },
            new FlightStatusEntity { Name = "ARRIVED"   },
        ], ct);

        // Estados de asiento
        await SeedSimpleAsync(db, db.SeatStatuses, x => x.Name,
        [
            new SeatStatusEntity { Name = "AVAILABLE" },
            new SeatStatusEntity { Name = "RESERVED"  },
            new SeatStatusEntity { Name = "OCCUPIED"  },
            new SeatStatusEntity { Name = "BLOCKED"   },
        ], ct);

        // Estados de check-in
        await SeedSimpleAsync(db, db.CheckInStatuses, x => x.Name,
        [
            new CheckInStatusEntity { Name = "PENDING"    },
            new CheckInStatusEntity { Name = "CHECKED_IN" },
            new CheckInStatusEntity { Name = "BOARDED"    },
        ], ct);

        // Tipos de contacto
        await SeedSimpleAsync(db, db.ContactTypes, x => x.Name,
        [
            new ContactTypeEntity { Name = "EMAIL"      },
            new ContactTypeEntity { Name = "TELÉFONO"   },
            new ContactTypeEntity { Name = "EMERGENCIA" },
            new ContactTypeEntity { Name = "WHATSAPP"   },
        ], ct);

        // Roles de tripulación
        await SeedSimpleAsync(db, db.CrewRoles, x => x.Name,
        [
            new CrewRoleEntity { Name = "COMANDANTE"        },
            new CrewRoleEntity { Name = "PRIMER OFICIAL"    },
            new CrewRoleEntity { Name = "AUXILIAR DE CABINA"},
            new CrewRoleEntity { Name = "JEFE DE CABINA"    },
        ], ct);

        // Cargos laborales
        await SeedSimpleAsync(db, db.JobPositions, x => x.Name,
        [
            new JobPositionEntity { Name = "Comandante",            Department = "Operaciones"         },
            new JobPositionEntity { Name = "Primer Oficial",        Department = "Operaciones"         },
            new JobPositionEntity { Name = "Auxiliar de Cabina",    Department = "Cabina"              },
            new JobPositionEntity { Name = "Jefe de Cabina",        Department = "Cabina"              },
            new JobPositionEntity { Name = "Mecánico de Aeronaves", Department = "Mantenimiento"       },
            new JobPositionEntity { Name = "Agente de Rampa",       Department = "Operaciones en Tierra"},
            new JobPositionEntity { Name = "Agente de Taquilla",    Department = "Comercial"           },
            new JobPositionEntity { Name = "Supervisor de Pista",   Department = "Operaciones en Tierra"},
        ], ct);

        // Clases de cabina
        await SeedSimpleAsync(db, db.CabinClasses, x => x.Name,
        [
            new CabinClassEntity { Name = "ECONOMY"         },
            new CabinClassEntity { Name = "PREMIUM ECONOMY" },
            new CabinClassEntity { Name = "BUSINESS"        },
            new CabinClassEntity { Name = "FIRST"           },
        ], ct);

        // Monedas
        await SeedSimpleAsync(db, db.Currencies, x => x.IsoCode,
        [
            new CurrencyEntity { IsoCode = "COP", Name = "Peso colombiano",      Symbol = "$"  },
            new CurrencyEntity { IsoCode = "USD", Name = "Dólar estadounidense", Symbol = "$"  },
            new CurrencyEntity { IsoCode = "EUR", Name = "Euro",                 Symbol = "€"  },
            new CurrencyEntity { IsoCode = "BRL", Name = "Real brasileño",       Symbol = "R$" },
            new CurrencyEntity { IsoCode = "MXN", Name = "Peso mexicano",        Symbol = "$"  },
            new CurrencyEntity { IsoCode = "ARS", Name = "Peso argentino",       Symbol = "$"  },
            new CurrencyEntity { IsoCode = "CLP", Name = "Peso chileno",         Symbol = "$"  },
            new CurrencyEntity { IsoCode = "PEN", Name = "Sol peruano",          Symbol = "S/" },
        ], ct);

        // Tipos de tarifa
        await SeedSimpleAsync(db, db.FareTypes, x => x.Name,
        [
            new FareTypeEntity { Name = "PROMO",     IsRefundable = false, IsChangeable = false, AdvancePurchaseDays = 21, BaggageIncluded = false },
            new FareTypeEntity { Name = "BASIC",     IsRefundable = false, IsChangeable = false, AdvancePurchaseDays = 0,  BaggageIncluded = false },
            new FareTypeEntity { Name = "CLASSIC",   IsRefundable = false, IsChangeable = true,  AdvancePurchaseDays = 7,  BaggageIncluded = false },
            new FareTypeEntity { Name = "FLEX",      IsRefundable = true,  IsChangeable = true,  AdvancePurchaseDays = 0,  BaggageIncluded = true  },
            new FareTypeEntity { Name = "BUSINESS",  IsRefundable = true,  IsChangeable = true,  AdvancePurchaseDays = 0,  BaggageIncluded = true  },
            new FareTypeEntity { Name = "FULL FLEX", IsRefundable = true,  IsChangeable = true,  AdvancePurchaseDays = 0,  BaggageIncluded = true  },
        ], ct);

        // Tipos de equipaje
        await SeedSimpleAsync(db, db.BaggageTypes, x => x.Name,
        [
            new BaggageTypeEntity { Name = "Equipaje de mano",         MaxWeightKg = 10m, ExtraFee = 0m   },
            new BaggageTypeEntity { Name = "Equipaje facturado 23 kg", MaxWeightKg = 23m, ExtraFee = 30m  },
            new BaggageTypeEntity { Name = "Equipaje facturado 32 kg", MaxWeightKg = 32m, ExtraFee = 60m  },
            new BaggageTypeEntity { Name = "Equipaje especial",        MaxWeightKg = 30m, ExtraFee = 90m  },
            new BaggageTypeEntity { Name = "Equipaje deportivo",       MaxWeightKg = 25m, ExtraFee = 50m  },
        ], ct);

        // Tipos de descuento
        await SeedSimpleAsync(db, db.DiscountTypes, x => x.Name,
        [
            new DiscountTypeEntity { Name = "Infante (en brazos)", Percentage = 90m, AgeMin = 0,    AgeMax = 2    },
            new DiscountTypeEntity { Name = "Niño",                Percentage = 25m, AgeMin = 2,    AgeMax = 11   },
            new DiscountTypeEntity { Name = "Estudiante",          Percentage = 10m, AgeMin = 14,   AgeMax = 25   },
            new DiscountTypeEntity { Name = "Adulto mayor",        Percentage = 15m, AgeMin = 60,   AgeMax = null },
            new DiscountTypeEntity { Name = "Militar",             Percentage = 20m, AgeMin = null, AgeMax = null },
        ], ct);
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  DATOS MAESTROS RELACIONALES  (todo-o-nada: sólo si la tabla está vacía)
    // ─────────────────────────────────────────────────────────────────────────
    private static async Task EnsureMasterDataAsync(AppDbContext db, DateTime now, CancellationToken ct)
    {
        // ── PAÍSES ──────────────────────────────────────────────────────────
        if (!await db.Countries.AnyAsync(ct))
        {
            await db.Countries.AddRangeAsync(
            [
                new CountryEntity { Name = "Argentina"      },
                new CountryEntity { Name = "Brasil"         },
                new CountryEntity { Name = "Chile"          },
                new CountryEntity { Name = "Colombia"       },
                new CountryEntity { Name = "Costa Rica"     },
                new CountryEntity { Name = "Ecuador"        },
                new CountryEntity { Name = "España"         },
                new CountryEntity { Name = "Estados Unidos" },
                new CountryEntity { Name = "Francia"        },
                new CountryEntity { Name = "México"         },
                new CountryEntity { Name = "Panamá"         },
                new CountryEntity { Name = "Perú"           },
                new CountryEntity { Name = "Venezuela"      },
            ], ct);
            await db.SaveChangesAsync(ct);
        }

        var allCountries = await db.Countries.AsNoTracking().ToListAsync(ct);
        var countryByName = allCountries.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        // ── CIUDADES ─────────────────────────────────────────────────────────
        if (!await db.Cities.AnyAsync(ct))
        {
            var cityRows = new List<CityEntity>();

            void AddCities(string country, params string[] names)
            {
                if (!countryByName.TryGetValue(country, out var c)) return;
                foreach (var n in names)
                    cityRows.Add(new CityEntity { Name = n, CountryId = c.Id, CreatedAt = now });
            }

            AddCities("Argentina",      "Buenos Aires", "Córdoba", "Mendoza", "Rosario", "Salta");
            AddCities("Brasil",         "Belo Horizonte", "Brasilia", "Manaos", "Rio de Janeiro", "São Paulo");
            AddCities("Chile",          "Antofagasta", "Concepción", "Santiago", "Valparaíso");
            AddCities("Colombia",       "Barranquilla", "Bogotá", "Bucaramanga", "Cali", "Cartagena",
                                        "Manizales", "Medellín", "Pereira", "Santa Marta");
            AddCities("Costa Rica",     "Liberia", "San José");
            AddCities("Ecuador",        "Cuenca", "Guayaquil", "Manta", "Quito");
            AddCities("España",         "Barcelona", "Madrid", "Sevilla", "Valencia");
            AddCities("Estados Unidos", "Atlanta", "Dallas", "Houston", "Los Ángeles", "Miami", "Nueva York");
            AddCities("Francia",        "París");
            AddCities("México",         "Cancún", "Ciudad de México", "Guadalajara", "Monterrey");
            AddCities("Panamá",         "Ciudad de Panamá", "Colón");
            AddCities("Perú",           "Arequipa", "Cusco", "Lima");
            AddCities("Venezuela",      "Caracas", "Maracaibo", "Valencia");

            await db.Cities.AddRangeAsync(cityRows, ct);
            await db.SaveChangesAsync(ct);
        }

        var cityLookup = await db.Cities.AsNoTracking()
            .Join(db.Countries.AsNoTracking(), ci => ci.CountryId, co => co.Id,
                  (ci, co) => new { City = ci, CountryName = co.Name })
            .ToListAsync(ct);
        var cityByKey = cityLookup.ToDictionary(
            x => $"{x.City.Name}|{x.CountryName}",
            x => x.City,
            StringComparer.OrdinalIgnoreCase);

        CityEntity? City(string name, string country)
            => cityByKey.TryGetValue($"{name}|{country}", out var c) ? c : null;

        // ── AEROPUERTOS ──────────────────────────────────────────────────────
        if (!await db.Airports.AnyAsync(ct))
        {
            var rows = new List<AirportEntity>();

            void AddAirport(string iata, string name, string city, string country)
            {
                var c = City(city, country);
                if (c is null) return;
                rows.Add(new AirportEntity { IataCode = iata, Name = name, CityId = c.CityId, CreatedAt = now });
            }

            // Colombia
            AddAirport("BOG", "Aeropuerto Internacional El Dorado",                     "Bogotá",           "Colombia");
            AddAirport("MDE", "Aeropuerto Internacional José María Córdova",             "Medellín",         "Colombia");
            AddAirport("CLO", "Aeropuerto Internacional Alfonso Bonilla Aragón",         "Cali",             "Colombia");
            AddAirport("BAQ", "Aeropuerto Internacional Ernesto Cortissoz",              "Barranquilla",     "Colombia");
            AddAirport("CTG", "Aeropuerto Internacional Rafael Núñez",                   "Cartagena",        "Colombia");
            AddAirport("BGA", "Aeropuerto Internacional Palonegro",                      "Bucaramanga",      "Colombia");
            AddAirport("PEI", "Aeropuerto Internacional Matecaña",                       "Pereira",          "Colombia");
            AddAirport("SMR", "Aeropuerto Internacional Simón Bolívar",                  "Santa Marta",      "Colombia");
            // Ecuador
            AddAirport("UIO", "Aeropuerto Internacional Mariscal Sucre",                 "Quito",            "Ecuador");
            AddAirport("GYE", "Aeropuerto Internacional José Joaquín de Olmedo",         "Guayaquil",        "Ecuador");
            AddAirport("CUE", "Aeropuerto Internacional Mariscal Lamar",                 "Cuenca",           "Ecuador");
            // Perú
            AddAirport("LIM", "Aeropuerto Internacional Jorge Chávez",                   "Lima",             "Perú");
            AddAirport("CUZ", "Aeropuerto Internacional Alejandro Velasco Astete",       "Cusco",            "Perú");
            AddAirport("AQP", "Aeropuerto Internacional Rodríguez Ballón",               "Arequipa",         "Perú");
            // Venezuela
            AddAirport("CCS", "Aeropuerto Internacional Simón Bolívar",                  "Caracas",          "Venezuela");
            AddAirport("MAR", "Aeropuerto Internacional La Chinita",                     "Maracaibo",        "Venezuela");
            // Brasil
            AddAirport("GRU", "Aeropuerto Internacional de Guarulhos",                   "São Paulo",        "Brasil");
            AddAirport("GIG", "Aeropuerto Internacional Galeão",                         "Rio de Janeiro",   "Brasil");
            AddAirport("BSB", "Aeropuerto Internacional Pres. Juscelino Kubitschek",     "Brasilia",         "Brasil");
            AddAirport("CNF", "Aeropuerto Internacional de Confins",                     "Belo Horizonte",   "Brasil");
            // Argentina
            AddAirport("EZE", "Aeropuerto Internacional Ministro Pistarini",             "Buenos Aires",     "Argentina");
            AddAirport("COR", "Aeropuerto Internacional Ing. Ambrosio Taravella",        "Córdoba",          "Argentina");
            AddAirport("MDZ", "Aeropuerto Internacional El Plumerillo",                  "Mendoza",          "Argentina");
            // Chile
            AddAirport("SCL", "Aeropuerto Internacional Arturo Merino Benítez",          "Santiago",         "Chile");
            AddAirport("ANF", "Aeropuerto Internacional Andrés Sabella Gálvez",          "Antofagasta",      "Chile");
            // México
            AddAirport("MEX", "Aeropuerto Internacional Benito Juárez",                  "Ciudad de México", "México");
            AddAirport("CUN", "Aeropuerto Internacional de Cancún",                      "Cancún",           "México");
            AddAirport("GDL", "Aeropuerto Internacional Miguel Hidalgo y Costilla",      "Guadalajara",      "México");
            AddAirport("MTY", "Aeropuerto Internacional General Mariano Escobedo",       "Monterrey",        "México");
            // Panamá
            AddAirport("PTY", "Aeropuerto Internacional de Tocumen",                     "Ciudad de Panamá", "Panamá");
            // Costa Rica
            AddAirport("SJO", "Aeropuerto Internacional Juan Santamaría",                "San José",         "Costa Rica");
            AddAirport("LIR", "Aeropuerto Internacional Daniel Oduber Quirós",           "Liberia",          "Costa Rica");
            // España
            AddAirport("MAD", "Aeropuerto Adolfo Suárez Madrid-Barajas",                 "Madrid",           "España");
            AddAirport("BCN", "Aeropuerto Josep Tarradellas Barcelona-El Prat",          "Barcelona",        "España");
            AddAirport("SVQ", "Aeropuerto Internacional de Sevilla",                     "Sevilla",          "España");
            // Estados Unidos
            AddAirport("MIA", "Aeropuerto Internacional de Miami",                       "Miami",            "Estados Unidos");
            AddAirport("JFK", "Aeropuerto Internacional John F. Kennedy",                "Nueva York",       "Estados Unidos");
            AddAirport("LAX", "Aeropuerto Internacional de Los Ángeles",                 "Los Ángeles",      "Estados Unidos");
            AddAirport("IAH", "Aeropuerto Internacional George Bush",                    "Houston",          "Estados Unidos");
            AddAirport("DFW", "Aeropuerto Internacional Dallas/Fort Worth",              "Dallas",           "Estados Unidos");
            AddAirport("ATL", "Aeropuerto Internacional Hartsfield-Jackson Atlanta",     "Atlanta",          "Estados Unidos");
            // Francia
            AddAirport("CDG", "Aeropuerto Internacional Charles de Gaulle",              "París",            "Francia");

            await db.Airports.AddRangeAsync(rows, ct);
            await db.SaveChangesAsync(ct);
        }

        // ── TERMINALES ───────────────────────────────────────────────────────
        if (!await db.Terminals.AnyAsync(ct))
        {
            var allAirports = await db.Airports.AsNoTracking().ToListAsync(ct);
            var terminalRows = new List<TerminalEntity>();

            // Aeropuertos que tienen terminal nacional separada
            var dualTerminal = new HashSet<string>(["BOG", "GRU", "MEX", "MAD", "EZE", "JFK", "LIM", "SCL", "CDG"],
                StringComparer.OrdinalIgnoreCase);

            foreach (var airport in allAirports)
            {
                terminalRows.Add(new TerminalEntity
                {
                    AirportId = airport.AirportId, Name = "T1",
                    IsInternational = true, CreatedAt = now
                });
                if (dualTerminal.Contains(airport.IataCode))
                    terminalRows.Add(new TerminalEntity
                    {
                        AirportId = airport.AirportId, Name = "T2",
                        IsInternational = false, CreatedAt = now
                    });
            }

            await db.Terminals.AddRangeAsync(terminalRows, ct);
            await db.SaveChangesAsync(ct);
        }

        // ── PUERTAS DE EMBARQUE ──────────────────────────────────────────────
        if (!await db.Gates.AnyAsync(ct))
        {
            var allTerminals = await db.Terminals.AsNoTracking().ToListAsync(ct);
            var gateRows = new List<GateEntity>();
            var codes = new[] { "A1", "A2", "A3", "B1", "B2" };

            foreach (var terminal in allTerminals)
                foreach (var code in codes)
                    gateRows.Add(new GateEntity { TerminalId = terminal.TerminalId, Code = code, IsActive = true });

            await db.Gates.AddRangeAsync(gateRows, ct);
            await db.SaveChangesAsync(ct);
        }

        // ── AEROLÍNEAS ───────────────────────────────────────────────────────
        if (!await db.Airlines.AnyAsync(ct))
        {
            await db.Airlines.AddRangeAsync(
            [
                new AirlineEntity { IataCode = "AV", Name = "Avianca",           IsActive = true, CreatedAt = now },
                new AirlineEntity { IataCode = "LA", Name = "LATAM Airlines",    IsActive = true, CreatedAt = now },
                new AirlineEntity { IataCode = "CM", Name = "Copa Airlines",     IsActive = true, CreatedAt = now },
                new AirlineEntity { IataCode = "AA", Name = "American Airlines", IsActive = true, CreatedAt = now },
                new AirlineEntity { IataCode = "IB", Name = "Iberia",            IsActive = true, CreatedAt = now },
                new AirlineEntity { IataCode = "UA", Name = "United Airlines",   IsActive = true, CreatedAt = now },
                new AirlineEntity { IataCode = "DL", Name = "Delta Air Lines",   IsActive = true, CreatedAt = now },
                new AirlineEntity { IataCode = "VB", Name = "VivaAerobus",       IsActive = true, CreatedAt = now },
            ], ct);
            await db.SaveChangesAsync(ct);
        }

        // ── FABRICANTES DE AERONAVES ─────────────────────────────────────────
        if (!await db.AircraftManufacturers.AnyAsync(ct))
        {
            var mfrRows = new List<AircraftManufacturerEntity>();
            if (countryByName.TryGetValue("Estados Unidos", out var usa))
                mfrRows.Add(new AircraftManufacturerEntity { Name = "Boeing",  CountryId = usa.Id });
            if (countryByName.TryGetValue("Francia", out var france))
                mfrRows.Add(new AircraftManufacturerEntity { Name = "Airbus",  CountryId = france.Id });
            if (countryByName.TryGetValue("Brasil", out var brazil))
                mfrRows.Add(new AircraftManufacturerEntity { Name = "Embraer", CountryId = brazil.Id });

            if (mfrRows.Count > 0)
            {
                await db.AircraftManufacturers.AddRangeAsync(mfrRows, ct);
                await db.SaveChangesAsync(ct);
            }
        }

        // ── TIPOS DE AERONAVE ────────────────────────────────────────────────
        if (!await db.AircraftTypes.AnyAsync(ct))
        {
            var mfrByName = (await db.AircraftManufacturers.AsNoTracking().ToListAsync(ct))
                .ToDictionary(m => m.Name, StringComparer.OrdinalIgnoreCase);
            var typeRows = new List<AircraftTypeEntity>();

            if (mfrByName.TryGetValue("Boeing", out var boeing))
            {
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = boeing.ManufacturerId, Model = "737-800",          TotalSeats = 189, CargoCapacityKg = 20_000m });
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = boeing.ManufacturerId, Model = "737 MAX 9",        TotalSeats = 193, CargoCapacityKg = 22_000m });
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = boeing.ManufacturerId, Model = "787-9 Dreamliner", TotalSeats = 296, CargoCapacityKg = 50_000m });
            }
            if (mfrByName.TryGetValue("Airbus", out var airbus))
            {
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = airbus.ManufacturerId, Model = "A320neo",  TotalSeats = 165, CargoCapacityKg = 18_000m });
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = airbus.ManufacturerId, Model = "A321neo",  TotalSeats = 194, CargoCapacityKg = 22_000m });
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = airbus.ManufacturerId, Model = "A330-300", TotalSeats = 277, CargoCapacityKg = 40_000m });
            }
            if (mfrByName.TryGetValue("Embraer", out var embraer))
            {
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = embraer.ManufacturerId, Model = "E190", TotalSeats = 100, CargoCapacityKg =  8_000m });
                typeRows.Add(new AircraftTypeEntity { ManufacturerId = embraer.ManufacturerId, Model = "E195", TotalSeats = 122, CargoCapacityKg = 10_000m });
            }

            if (typeRows.Count > 0)
            {
                await db.AircraftTypes.AddRangeAsync(typeRows, ct);
                await db.SaveChangesAsync(ct);
            }
        }

        // ── NACIONALIDADES ───────────────────────────────────────────────────
        if (!await db.Nationalities.AnyAsync(ct))
        {
            var demonymMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Argentina"]      = "ARGENTINO/A",
                ["Brasil"]         = "BRASILEÑO/A",
                ["Chile"]          = "CHILENO/A",
                ["Colombia"]       = "COLOMBIANO/A",
                ["Costa Rica"]     = "COSTARRICENSE",
                ["Ecuador"]        = "ECUATORIANO/A",
                ["España"]         = "ESPAÑOL/A",
                ["Estados Unidos"] = "ESTADOUNIDENSE",
                ["Francia"]        = "FRANCÉS/ESA",
                ["México"]         = "MEXICANO/A",
                ["Panamá"]         = "PANAMEÑO/A",
                ["Perú"]           = "PERUANO/A",
                ["Venezuela"]      = "VENEZOLANO/A",
            };

            var natRows = allCountries
                .Where(c => demonymMap.ContainsKey(c.Name))
                .Select(c => new NationalityEntity { CountryId = c.Id, Demonym = demonymMap[c.Name] })
                .ToList();

            if (natRows.Count > 0)
            {
                await db.Nationalities.AddRangeAsync(natRows, ct);
                await db.SaveChangesAsync(ct);
            }
        }

        // ── SEAT MAPS (plantillas de asientos por tipo de aeronave) ─────────
        if (!await db.SeatMaps.AnyAsync(ct))
        {
            var allAircraftTypes = await db.AircraftTypes.AsNoTracking().ToListAsync(ct);
            var cabinByName = (await db.CabinClasses.AsNoTracking().ToListAsync(ct))
                .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            var seatMapRows = new List<SeatMapEntity>();

            if (cabinByName.TryGetValue("BUSINESS", out var bizClass) &&
                cabinByName.TryGetValue("ECONOMY",  out var ecoClass))
            {
                foreach (var type in allAircraftTypes)
                {
                    // Embraer (E190/E195): 2+2 → A B C D. Resto: 3+3 → A B C D E F
                    string[] letters  = type.Model.StartsWith("E", StringComparison.OrdinalIgnoreCase)
                        ? ["A", "B", "C", "D"]
                        : ["A", "B", "C", "D", "E", "F"];
                    int bizRowCount   = type.TotalSeats <= 120 ? 2 : 3;

                    int count = 0, row = 1;
                    while (count < type.TotalSeats)
                    {
                        bool isBiz = row <= bizRowCount;
                        foreach (var l in letters)
                        {
                            if (count >= type.TotalSeats) break;
                            seatMapRows.Add(new SeatMapEntity
                            {
                                AircraftTypeId = type.AircraftTypeId,
                                SeatNumber     = $"{row}{l}",
                                CabinClassId   = isBiz ? bizClass.Id : ecoClass.Id
                            });
                            count++;
                        }
                        row++;
                    }
                }
            }

            if (seatMapRows.Count > 0)
            {
                await db.SeatMaps.AddRangeAsync(seatMapRows, ct);
                await db.SaveChangesAsync(ct);
            }
        }

        // ── FLIGHT SEATS para vuelos programados sin asientos ────────────────
        {
            var availableStatusId = await db.SeatStatuses.AsNoTracking()
                .Where(s => s.Name == "AVAILABLE")
                .Select(s => s.Id)
                .FirstOrDefaultAsync(ct);

            if (availableStatusId > 0)
            {
                var scheduledFlights = await db.ScheduledFlights.AsNoTracking().ToListAsync(ct);
                var seatMapsByType   = (await db.SeatMaps.AsNoTracking().ToListAsync(ct))
                    .GroupBy(sm => sm.AircraftTypeId)
                    .ToDictionary(g => g.Key, g => g.ToList());
                var typeByAircraft   = await db.Aircrafts.AsNoTracking()
                    .ToDictionaryAsync(a => a.AircraftId, a => a.AircraftTypeId, ct);

                var flightSeatRows = new List<FlightSeatEntity>();
                foreach (var flight in scheduledFlights)
                {
                    var alreadyHasSeats = await db.FlightSeats
                        .AnyAsync(fs => fs.ScheduledFlightId == flight.Id, ct);
                    if (alreadyHasSeats) continue;

                    if (!typeByAircraft.TryGetValue(flight.AircraftId, out var typeId)) continue;
                    if (!seatMapsByType.TryGetValue(typeId, out var maps))             continue;

                    flightSeatRows.AddRange(maps.Select(sm => new FlightSeatEntity
                    {
                        ScheduledFlightId = flight.Id,
                        SeatMapId         = sm.Id,
                        SeatStatusId      = availableStatusId,
                        CreatedAt         = DateTime.UtcNow,
                        Version           = []
                    }));
                }

                if (flightSeatRows.Count > 0)
                {
                    await db.FlightSeats.AddRangeAsync(flightSeatRows, ct);
                    await db.SaveChangesAsync(ct);
                }
            }
        }

        // Precios (vuelo + cabina + tarifa). Sin esto fallan asistente y lista de espera.
        await EnsureFlightCabinPricesForExistingFlightsAsync(db, ct);

        // ── PROGRAMAS DE LEALTAD Y NIVELES ──────────────────────────────────
        if (!await db.LoyaltyPrograms.AnyAsync(ct))
        {
            var airlineByIata = (await db.Airlines.AsNoTracking().ToListAsync(ct))
                .ToDictionary(a => a.IataCode, StringComparer.OrdinalIgnoreCase);

            var programDefs = new (string Iata, string ProgramName, decimal MpD)[]
            {
                ("AV", "LifeMiles",    5m),
                ("LA", "LATAM Pass",  5m),
                ("CM", "ConnectMiles",5m),
                ("AA", "AAdvantage",  5m),
                ("IB", "Iberia Plus", 4m),
            };

            var loyaltyRows = programDefs
                .Where(p => airlineByIata.ContainsKey(p.Iata))
                .Select(p => new LoyaltyProgramEntity
                {
                    AirlineId      = airlineByIata[p.Iata].AirlineId,
                    Name           = p.ProgramName,
                    MilesPerDollar = p.MpD
                })
                .ToList();

            if (loyaltyRows.Count > 0)
            {
                await db.LoyaltyPrograms.AddRangeAsync(loyaltyRows, ct);
                await db.SaveChangesAsync(ct);

                var allPrograms = await db.LoyaltyPrograms.AsNoTracking().ToListAsync(ct);
                var tierRows = new List<LoyaltyTierEntity>();
                foreach (var prog in allPrograms)
                    tierRows.AddRange(
                    [
                        new LoyaltyTierEntity { LoyaltyProgramId = prog.Id, Name = "Member",   MinMiles =       0, Benefits = "Acumulación básica de millas"             },
                        new LoyaltyTierEntity { LoyaltyProgramId = prog.Id, Name = "Silver",   MinMiles =  25_000, Benefits = "Embarque preferencial, maleta extra gratis" },
                        new LoyaltyTierEntity { LoyaltyProgramId = prog.Id, Name = "Gold",     MinMiles =  50_000, Benefits = "Acceso sala VIP, upgrade preferencial"      },
                        new LoyaltyTierEntity { LoyaltyProgramId = prog.Id, Name = "Platinum", MinMiles = 100_000, Benefits = "Upgrade garantizado, equipaje ilimitado"     },
                    ]);

                await db.LoyaltyTiers.AddRangeAsync(tierRows, ct);
                await db.SaveChangesAsync(ct);
            }
        }
    }

    /// <summary>Precio por vuelo/cabina/tarifa. Idempotente: solo inserta filas faltantes.</summary>
    private static async Task EnsureFlightCabinPricesForExistingFlightsAsync(AppDbContext db, CancellationToken ct)
    {
        var fareByName = await db.FareTypes.AsNoTracking()
            .ToDictionaryAsync(f => f.Name, f => f.Id, StringComparer.OrdinalIgnoreCase, ct);
        if (fareByName.Count == 0) return;

        var cabinNameById = await db.CabinClasses.AsNoTracking()
            .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

        var flightCabinPairs = await (
            from fs in db.FlightSeats.AsNoTracking()
            join sm in db.SeatMaps.AsNoTracking() on fs.SeatMapId equals sm.Id
            select new { fs.ScheduledFlightId, sm.CabinClassId }
        ).Distinct().ToListAsync(ct);
        if (flightCabinPairs.Count == 0) return;

        var existing = await db.FlightCabinPrices.AsNoTracking()
            .Select(f => $"{f.ScheduledFlightId}\u001f{f.CabinClassId}\u001f{f.FareTypeId}")
            .ToListAsync(ct);
        var existingSet = new HashSet<string>(existing, StringComparer.Ordinal);

        (string Name, decimal Price)[] TiersFor(string cabinUpper) => cabinUpper switch
        {
            "ECONOMY" or "PREMIUM ECONOMY" => [
                ("PROMO", 149m), ("BASIC", 199m), ("CLASSIC", 259m), ("FLEX", 329m)
            ],
            "BUSINESS" or "FIRST" => [
                ("BASIC", 1200m), ("BUSINESS", 1600m), ("FLEX", 2000m), ("FULL FLEX", 2400m)
            ],
            _ => [("BASIC", 300m), ("FLEX", 450m)]
        };

        var toAdd = new List<FlightCabinPriceEntity>();
        foreach (var p in flightCabinPairs)
        {
            if (!cabinNameById.TryGetValue(p.CabinClassId, out var cname)) continue;
            var upper = cname.Trim().ToUpperInvariant();
            foreach (var (fareName, price) in TiersFor(upper))
            {
                if (!fareByName.TryGetValue(fareName, out var fareId)) continue;
                var key = $"{p.ScheduledFlightId}\u001f{p.CabinClassId}\u001f{fareId}";
                if (existingSet.Contains(key)) continue;
                toAdd.Add(new FlightCabinPriceEntity
                {
                    ScheduledFlightId = p.ScheduledFlightId,
                    CabinClassId      = p.CabinClassId,
                    FareTypeId        = fareId,
                    Price             = price
                });
                existingSet.Add(key);
            }
        }

        if (toAdd.Count == 0) return;
        await db.FlightCabinPrices.AddRangeAsync(toAdd, ct);
        await db.SaveChangesAsync(ct);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static async Task SeedSimpleAsync<TEntity>(
        AppDbContext db,
        DbSet<TEntity> set,
        Func<TEntity, string> keySelector,
        IEnumerable<TEntity> rows,
        CancellationToken ct)
        where TEntity : class
    {
        var existing     = await set.AsNoTracking().ToListAsync(ct);
        var existingKeys = existing
            .Select(keySelector)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var pending = rows.Where(x => !existingKeys.Contains(keySelector(x))).ToList();
        if (pending.Count == 0) return;

        await set.AddRangeAsync(pending, ct);
        await db.SaveChangesAsync(ct);
    }
}
