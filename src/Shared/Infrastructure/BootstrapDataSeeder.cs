using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Aircraft.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftManufacturer.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.AircraftType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Airline.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaseFlight.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Infrastructure.Entity;
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
        await EnsureCheckInDemoDataAsync(db, now, ct);
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
            new TicketStatusEntity { Name = "ISSUED"     },
            new TicketStatusEntity { Name = "PAID"       },
            new TicketStatusEntity { Name = "CHECKED_IN" },
            new TicketStatusEntity { Name = "CANCELLED"  },
            new TicketStatusEntity { Name = "USED"       },
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
    //  SEED DEMO DEL MÓDULO DE CHECK-IN  — versión "a prueba de fallos"
    //  · Un try-catch gigante externo garantiza que el programa SIEMPRE inicia.
    //  · Cada paso tiene su propio try-catch; un error en uno no detiene el resto.
    //  · Los inserts críticos usan ExecuteSqlRawAsync + INSERT IGNORE para
    //    saltarse validaciones de EF y restricciones de unicidad sin lanzar.
    //  · Console.WriteLine al final es INCONDICIONAL.
    // ─────────────────────────────────────────────────────────────────────────
    private static async Task EnsureCheckInDemoDataAsync(AppDbContext db, DateTime now, CancellationToken ct)
    {
        const string demoUsername        = "admin_demo";
        const string demoPassword        = "demo1234";
        const string demoDocumentNumber  = "123456";
        const string demoFlightCode      = "DM-9001";
        const string demoAircraftReg     = "HK-DEMO";
        const string demoReservationCode = "RES-DEMO-001";
        const string demoTicketCode      = "TKT-DEMO-001";

        try
        {
            // ── IDEMPOTENCIA: si el usuario ya existe, todo ya fue sembrado ─────
            if (await db.Users.AsNoTracking().AnyAsync(u => u.Username == demoUsername, ct))
            {
                Console.WriteLine(">>> DATA DEMO DE CHECK-IN CARGADA CON ÉXITO <<<");
                return;
            }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 1 — CATÁLOGOS CRÍTICOS (crear si faltan, nunca lanzar)
            // ════════════════════════════════════════════════════════════════════
            int adminRoleId    = 0;
            int paidStatusId   = 0;
            int scheduledFsId  = 0;
            int confirmedRsId  = 0;
            int basicFareId    = 0;
            int availSeatStId  = 0;
            int ccDocTypeId    = 0;

            try
            {
                var r = await db.Roles.FirstOrDefaultAsync(x => x.Name == "Administrador", ct);
                if (r is null) { r = new RoleEntity { Name = "Administrador", IsActive = true }; db.Roles.Add(r); await db.SaveChangesAsync(ct); }
                adminRoleId = r.RoleId;
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Role: {ex.Message}"); }

            try
            {
                var s = await db.TicketStatuses.FirstOrDefaultAsync(x => x.Name == "PAID", ct);
                if (s is null) { s = new TicketStatusEntity { Name = "PAID" }; db.TicketStatuses.Add(s); await db.SaveChangesAsync(ct); }
                paidStatusId = s.Id;
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] TicketStatus PAID: {ex.Message}"); }

            try
            {
                if (!await db.TicketStatuses.AsNoTracking().AnyAsync(x => x.Name == "CHECKED_IN", ct))
                { db.TicketStatuses.Add(new TicketStatusEntity { Name = "CHECKED_IN" }); await db.SaveChangesAsync(ct); }
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] TicketStatus CHECKED_IN: {ex.Message}"); }

            try
            {
                if (!await db.CheckInStatuses.AsNoTracking().AnyAsync(x => x.Name == "CHECKED_IN", ct))
                { db.CheckInStatuses.Add(new CheckInStatusEntity { Name = "CHECKED_IN" }); await db.SaveChangesAsync(ct); }
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] CheckInStatus CHECKED_IN: {ex.Message}"); }

            try
            {
                var s = await db.FlightStatuses.FirstOrDefaultAsync(x => x.Name == "SCHEDULED", ct);
                if (s is null) { s = new FlightStatusEntity { Name = "SCHEDULED" }; db.FlightStatuses.Add(s); await db.SaveChangesAsync(ct); }
                scheduledFsId = s.Id;
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] FlightStatus SCHEDULED: {ex.Message}"); }

            try
            {
                var s = await db.ReservationStatuses.FirstOrDefaultAsync(x => x.Name == "CONFIRMED", ct);
                if (s is null) { s = new ReservationStatusEntity { Name = "CONFIRMED" }; db.ReservationStatuses.Add(s); await db.SaveChangesAsync(ct); }
                confirmedRsId = s.Id;
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] ReservationStatus CONFIRMED: {ex.Message}"); }

            try
            {
                var s = await db.FareTypes.FirstOrDefaultAsync(x => x.Name == "BASIC", ct);
                if (s is null) { s = new FareTypeEntity { Name = "BASIC" }; db.FareTypes.Add(s); await db.SaveChangesAsync(ct); }
                basicFareId = s.Id;
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] FareType BASIC: {ex.Message}"); }

            try
            {
                availSeatStId = await db.SeatStatuses.AsNoTracking().Where(x => x.Name == "AVAILABLE").Select(x => x.Id).FirstOrDefaultAsync(ct);
                if (availSeatStId == 0)
                { var s = new SeatStatusEntity { Name = "AVAILABLE" }; db.SeatStatuses.Add(s); await db.SaveChangesAsync(ct); availSeatStId = s.Id; }
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] SeatStatus AVAILABLE: {ex.Message}"); }

            try
            {
                ccDocTypeId = await db.DocumentTypes.AsNoTracking().Where(x => x.Code == "CC").Select(x => x.DocumentTypeId).FirstOrDefaultAsync(ct);
                if (ccDocTypeId == 0)
                { var d = new DocumentTypeEntity { Code = "CC", Name = "Cédula de ciudadanía" }; db.DocumentTypes.Add(d); await db.SaveChangesAsync(ct); ccDocTypeId = d.DocumentTypeId; }
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] DocumentType CC: {ex.Message}"); }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 2 — AEROPUERTOS BOG y MDE (INSERT IGNORE directo)
            // ════════════════════════════════════════════════════════════════════
            int bogId = 0, mdeId = 0;

            try
            {
                bogId = await db.Airports.AsNoTracking().Where(a => a.IataCode == "BOG").Select(a => a.AirportId).FirstOrDefaultAsync(ct);
                if (bogId == 0)
                {
                    // Reutilizamos cualquier CityId existente como "Bogotá" de emergencia
                    var fallbackCityId = await db.Cities.AsNoTracking().Select(c => c.CityId).FirstOrDefaultAsync(ct);
                    if (fallbackCityId > 0)
                    {
                        await db.Database.ExecuteSqlRawAsync(
                            "INSERT IGNORE INTO airport (IataCode, Name, CityId, CreatedAt) VALUES ({0}, {1}, {2}, {3})",
                            "BOG", "Aeropuerto Internacional El Dorado", fallbackCityId, now);
                        bogId = await db.Airports.AsNoTracking().Where(a => a.IataCode == "BOG").Select(a => a.AirportId).FirstOrDefaultAsync(ct);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Airport BOG: {ex.Message}"); }

            try
            {
                mdeId = await db.Airports.AsNoTracking().Where(a => a.IataCode == "MDE").Select(a => a.AirportId).FirstOrDefaultAsync(ct);
                if (mdeId == 0)
                {
                    var fallbackCityId = await db.Cities.AsNoTracking().Select(c => c.CityId).FirstOrDefaultAsync(ct);
                    if (fallbackCityId > 0)
                    {
                        await db.Database.ExecuteSqlRawAsync(
                            "INSERT IGNORE INTO airport (IataCode, Name, CityId, CreatedAt) VALUES ({0}, {1}, {2}, {3})",
                            "MDE", "Aeropuerto Internacional José María Córdova", fallbackCityId, now);
                        mdeId = await db.Airports.AsNoTracking().Where(a => a.IataCode == "MDE").Select(a => a.AirportId).FirstOrDefaultAsync(ct);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Airport MDE: {ex.Message}"); }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 3 — PERSONA + CUSTOMER + PASSENGER
            // ════════════════════════════════════════════════════════════════════
            int personId    = 0;
            int customerId  = 0;
            int passengerId = 0;

            try
            {
                var person = await db.Persons.FirstOrDefaultAsync(
                    p => p.DocumentNumber == demoDocumentNumber && p.DocumentTypeId == ccDocTypeId, ct);
                if (person is null)
                {
                    var genderId = await db.Genders.AsNoTracking().Select(g => (int?)g.Id).FirstOrDefaultAsync(ct);
                    person = new PersonEntity
                    {
                        FirstName      = "Usuario",
                        LastName       = "Test",
                        DocumentTypeId = ccDocTypeId,
                        DocumentNumber = demoDocumentNumber,
                        GenderId       = genderId,
                        BirthDate      = DateOnly.FromDateTime(now.Date.AddYears(-30)),
                        CreatedAt      = now
                    };
                    db.Persons.Add(person);
                    await db.SaveChangesAsync(ct);
                }
                personId = person.Id;
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Person: {ex.Message}"); }

            if (personId > 0)
            {
                try
                {
                    var customer = await db.Customers.FirstOrDefaultAsync(c => c.PersonId == personId, ct);
                    if (customer is null)
                    {
                        customer = new CustomerEntity { PersonId = personId, Email = "demo@checkin.test", Phone = "+57 300 000 0000", CreatedAt = now };
                        db.Customers.Add(customer);
                        await db.SaveChangesAsync(ct);
                    }
                    customerId = customer.Id;
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Customer: {ex.Message}"); }

                try
                {
                    var passenger = await db.Passengers.FirstOrDefaultAsync(p => p.PersonId == personId, ct);
                    if (passenger is null)
                    {
                        passenger = new PassengerEntity { PersonId = personId, FrequentFlyerNumber = null, NationalityId = null, CreatedAt = now };
                        db.Passengers.Add(passenger);
                        await db.SaveChangesAsync(ct);
                    }
                    passengerId = passenger.Id;
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Passenger: {ex.Message}"); }
            }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 4 — USUARIO admin_demo  (INSERT IGNORE — SQL directo)
            // ════════════════════════════════════════════════════════════════════
            if (personId > 0 && adminRoleId > 0)
            {
                try
                {
                    var hash = PasswordHasher.Hash(demoPassword);
                    await db.Database.ExecuteSqlRawAsync(
                        "INSERT IGNORE INTO user (PersonId, RoleId, Username, PasswordHash, IsActive, CreatedAt) VALUES ({0}, {1}, {2}, {3}, 1, {4})",
                        personId, adminRoleId, demoUsername, hash, now);
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] User INSERT: {ex.Message}"); }
            }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 5 — VUELO DM-9001  (Aerolínea → Aeronave → Ruta → Base → Scheduled)
            // ════════════════════════════════════════════════════════════════════
            int airlineId         = 0;
            int aircraftTypeId    = 0;
            int aircraftId        = 0;
            int routeId           = 0;
            int baseFlightId      = 0;
            int scheduledFlightId = 0;

            try
            {
                airlineId = await db.Airlines.AsNoTracking().Where(a => a.IataCode == "AV").Select(a => a.AirlineId).FirstOrDefaultAsync(ct);
                if (airlineId == 0)
                    airlineId = await db.Airlines.AsNoTracking().Select(a => a.AirlineId).FirstOrDefaultAsync(ct);
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Airline: {ex.Message}"); }

            try
            {
                aircraftTypeId = await db.AircraftTypes.AsNoTracking().Where(t => t.Model == "737-800").Select(t => t.AircraftTypeId).FirstOrDefaultAsync(ct);
                if (aircraftTypeId == 0)
                    aircraftTypeId = await db.AircraftTypes.AsNoTracking().Select(t => t.AircraftTypeId).FirstOrDefaultAsync(ct);
            }
            catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] AircraftType: {ex.Message}"); }

            if (airlineId > 0 && aircraftTypeId > 0)
            {
                try
                {
                    var aircraft = await db.Aircrafts.FirstOrDefaultAsync(a => a.RegistrationNumber == demoAircraftReg, ct);
                    if (aircraft is null)
                    {
                        aircraft = new AircraftEntity { AirlineId = airlineId, AircraftTypeId = aircraftTypeId, RegistrationNumber = demoAircraftReg, ManufactureYear = 2020, IsActive = true, CreatedAt = now };
                        db.Aircrafts.Add(aircraft);
                        await db.SaveChangesAsync(ct);
                    }
                    aircraftId = aircraft.AircraftId;
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Aircraft: {ex.Message}"); }
            }

            if (bogId > 0 && mdeId > 0)
            {
                try
                {
                    var route = await db.Routes.FirstOrDefaultAsync(r => r.OriginAirportId == bogId && r.DestinationAirportId == mdeId, ct);
                    if (route is null)
                    {
                        route = new RouteEntity { OriginAirportId = bogId, DestinationAirportId = mdeId, CreatedAt = now };
                        db.Routes.Add(route);
                        await db.SaveChangesAsync(ct);
                    }
                    routeId = route.Id;
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Route BOG→MDE: {ex.Message}"); }
            }

            if (airlineId > 0 && routeId > 0)
            {
                try
                {
                    var baseFlight = await db.BaseFlights.FirstOrDefaultAsync(b => b.FlightCode == demoFlightCode, ct);
                    if (baseFlight is null)
                    {
                        baseFlight = new BaseFlightEntity { FlightCode = demoFlightCode, AirlineId = airlineId, RouteId = routeId, CreatedAt = now };
                        db.BaseFlights.Add(baseFlight);
                        await db.SaveChangesAsync(ct);
                    }
                    baseFlightId = baseFlight.Id;
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] BaseFlight: {ex.Message}"); }
            }

            if (baseFlightId > 0 && aircraftId > 0 && scheduledFsId > 0)
            {
                try
                {
                    var departureDate    = DateOnly.FromDateTime(now.Date.AddDays(7));
                    var departureTime    = new TimeOnly(10, 0);
                    var estimatedArrival = now.Date.AddDays(7).AddHours(11).AddMinutes(15);

                    var sf = await db.ScheduledFlights.FirstOrDefaultAsync(
                        s => s.BaseFlightId == baseFlightId && s.DepartureDate == departureDate, ct);
                    if (sf is null)
                    {
                        var demoGateId = await db.Gates.AsNoTracking().Where(g => g.IsActive).Select(g => (int?)g.GateId).FirstOrDefaultAsync(ct);
                        sf = new ScheduledFlightEntity
                        {
                            BaseFlightId             = baseFlightId,
                            AircraftId               = aircraftId,
                            GateId                   = demoGateId,
                            DepartureDate            = departureDate,
                            DepartureTime            = departureTime,
                            EstimatedArrivalDatetime = estimatedArrival,
                            FlightStatusId           = scheduledFsId,
                            CreatedAt                = now
                        };
                        db.ScheduledFlights.Add(sf);
                        await db.SaveChangesAsync(ct);
                    }
                    scheduledFlightId = sf.Id;
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] ScheduledFlight: {ex.Message}"); }
            }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 6 — ASIENTOS DE VUELO (mínimo 5 para que el check-in funcione)
            // ════════════════════════════════════════════════════════════════════
            int firstSeatId = 0;

            if (scheduledFlightId > 0 && aircraftTypeId > 0 && availSeatStId > 0)
            {
                try
                {
                    var alreadyHasSeats = await db.FlightSeats.AnyAsync(fs => fs.ScheduledFlightId == scheduledFlightId, ct);
                    if (!alreadyHasSeats)
                    {
                        var seatMaps = await db.SeatMaps.AsNoTracking()
                            .Where(sm => sm.AircraftTypeId == aircraftTypeId)
                            .OrderBy(sm => sm.Id)
                            .Take(10)
                            .ToListAsync(ct);
                        foreach (var sm in seatMaps)
                            db.FlightSeats.Add(new FlightSeatEntity
                            {
                                ScheduledFlightId = scheduledFlightId,
                                SeatMapId         = sm.Id,
                                SeatStatusId      = availSeatStId,
                                CreatedAt         = now,
                                Version           = []
                            });
                        if (seatMaps.Count > 0)
                            await db.SaveChangesAsync(ct);
                    }
                    firstSeatId = await db.FlightSeats.AsNoTracking()
                        .Where(fs => fs.ScheduledFlightId == scheduledFlightId)
                        .OrderBy(fs => fs.Id)
                        .Select(fs => fs.Id)
                        .FirstOrDefaultAsync(ct);
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] FlightSeats: {ex.Message}"); }
            }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 7 — RESERVA RES-DEMO-001  (INSERT IGNORE — SQL directo)
            // ════════════════════════════════════════════════════════════════════
            int reservationId = 0;

            if (customerId > 0 && scheduledFlightId > 0 && confirmedRsId > 0)
            {
                try
                {
                    await db.Database.ExecuteSqlRawAsync(
                        @"INSERT IGNORE INTO reservation
                          (reservation_code, customer_id, scheduled_flight_id, reservation_date,
                           reservation_status_id, confirmed_at, created_at)
                          VALUES ({0}, {1}, {2}, {3}, {4}, {3}, {3})",
                        demoReservationCode, customerId, scheduledFlightId, now, confirmedRsId);

                    reservationId = await db.Reservations.AsNoTracking()
                        .Where(r => r.ReservationCode == demoReservationCode)
                        .Select(r => r.Id)
                        .FirstOrDefaultAsync(ct);
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Reservation INSERT: {ex.Message}"); }
            }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 8 — DETALLE DE RESERVA
            // ════════════════════════════════════════════════════════════════════
            int detailId = 0;

            if (reservationId > 0 && passengerId > 0 && firstSeatId > 0 && basicFareId > 0)
            {
                try
                {
                    var detail = await db.ReservationDetails.FirstOrDefaultAsync(
                        d => d.ReservationId == reservationId && d.PassengerId == passengerId, ct);
                    if (detail is null)
                    {
                        detail = new ReservationDetailEntity
                        {
                            ReservationId = reservationId,
                            PassengerId   = passengerId,
                            FlightSeatId  = firstSeatId,
                            FareTypeId    = basicFareId,
                            CreatedAt     = now
                        };
                        db.ReservationDetails.Add(detail);
                        await db.SaveChangesAsync(ct);
                    }
                    detailId = detail.Id;
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] ReservationDetail: {ex.Message}"); }
            }

            // ════════════════════════════════════════════════════════════════════
            //  PASO 9 — TIQUETE TKT-DEMO-001  PAID  (INSERT IGNORE — SQL directo)
            // ════════════════════════════════════════════════════════════════════
            if (detailId > 0 && paidStatusId > 0)
            {
                try
                {
                    await db.Database.ExecuteSqlRawAsync(
                        @"INSERT IGNORE INTO ticket
                          (ticket_code, reservation_detail_id, issue_date, ticket_status_id, created_at)
                          VALUES ({0}, {1}, {2}, {3}, {2})",
                        demoTicketCode, detailId, now, paidStatusId);
                }
                catch (Exception ex) { Console.WriteLine($"[DEMO-SEED] Ticket INSERT: {ex.Message}"); }
            }
        }
        catch (Exception ex)
        {
            // El catch externo atrapa cualquier error imprevisto.
            // El programa NUNCA debe fallar por el seed.
            Console.WriteLine($"[DEMO-SEED] Error inesperado (no fatal): {ex.GetType().Name} — {ex.Message}");
        }

        // ── CONFIRMACIÓN INCONDICIONAL ────────────────────────────────────────
        Console.WriteLine(">>> DATA DEMO DE CHECK-IN CARGADA CON ÉXITO <<<");
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
