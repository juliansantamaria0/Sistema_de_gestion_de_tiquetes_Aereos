# ✈️ Sistema de Gestión de Tiquetes Aéreos

Aplicación de consola en C# para la gestión integral de tiquetes aéreos, conectada a MySQL mediante Entity Framework Core.

---

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Arquitectura](#arquitectura)
- [Funcionalidades](#funcionalidades)
- [Módulo de Fidelización por Millas](#módulo-de-fidelización-por-millas)
- [Modelo de Datos](#modelo-de-datos)
- [Reglas de Negocio](#reglas-de-negocio)
- [Consultas LINQ](#consultas-linq)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Tecnologías](#tecnologías)

---

## 📖 Descripción

El sistema permite administrar de forma estructurada la operación completa de tiquetes aéreos: vuelos, aerolíneas, rutas, clientes, reservas, emisión de tiquetes y programa de fidelización por millas. Todos los datos se persisten en una base de datos MySQL, gestionada mediante Entity Framework Core con consultas LINQ.

### Objetivos

- Diseñar un modelo de datos que represente aerolíneas, vuelos, rutas, clientes, reservas y tiquetes.
- Implementar una aplicación de consola con navegación por menús.
- Configurar la conexión entre la aplicación y MySQL vía EF Core.
- Registrar y administrar clientes, vuelos y destinos.
- Gestionar reservas, emisión de tiquetes y control de asientos.
- Implementar consultas y reportes mediante LINQ.
- Implementar un programa de fidelización por millas con acumulación, redención y reportes analíticos.
- Aplicar buenas prácticas de organización y separación de responsabilidades.

---

## 🏗️ Arquitectura

El proyecto sigue una arquitectura modular por capas:

```
📁 src/
└── 📂 Modules/
    ├── 📂 Airline/
    ├── 📂 Reservation/
    ├── 📂 Ticket/
    ├── 📂 LoyaltyProgram/
    ├── 📂 LoyaltyAccount/
    ├── 📂 LoyaltyTransaction/
    ├── 📂 LoyaltyAnalytics/        ← Módulo de reportes LINQ (Examen 5)
    │   ├── 📂 Domain/
    │   │   └── 📂 Repositories/    → Contratos e interfaces
    │   ├── 📂 Application/
    │   │   ├── 📂 Interfaces/      → ILoyaltyAnalyticsService
    │   │   └── 📂 Services/        → LoyaltyAnalyticsService
    │   ├── 📂 Infrastructure/
    │   │   └── 📂 Repository/      → Consultas LINQ reales
    │   └── 📂 UI/                  → Menú de consola con Spectre
    └── 📂 ... (demás módulos)
📂 src/Shared/
    ├── 📂 context/                 → AppDbContext (EF Core)
    ├── 📂 Infrastructure/          → DI, Seeder, AuthService
    └── 📂 UI/                      → ConsoleDashboard, Menús
```

Cada módulo respeta la separación: **Domain → Application → Infrastructure → UI**.

---

## ⚙️ Funcionalidades

### ✈️ Gestión de Aerolíneas
- Registro y administración de aerolíneas

### 🌍 Gestión de Ubicaciones
- Registro de países y ciudades
- Administración de aeropuertos

### 🛫 Gestión de Vuelos
- Creación de rutas
- Gestión de vuelos base y programados
- Asignación de aeronaves

### 👤 Gestión de Clientes
- Registro de personas
- Administración de clientes y pasajeros

### 🎟️ Reservas
- Creación de reservas con código único
- Asociación con cliente y vuelo programado
- Validación de disponibilidad en tiempo real
- Lista de espera y reprogramación

### 💺 Gestión de Asientos
- Control de asientos por vuelo
- Validación de disponibilidad antes de confirmar reserva

### 📊 Estados del Sistema
- Estados de reservas (pendiente, confirmada, cancelada, etc.)
- Estados de vuelos (programado, en vuelo, aterrizado, cancelado, etc.)
- Estados de asientos (disponible, ocupado, bloqueado, etc.)

---

## 🏆 Módulo de Fidelización por Millas

Implementado como parte del **Examen 5**, este módulo agrega un programa completo de fidelización con acumulación y redención de millas, más una capa de analítica avanzada con LINQ.

### Lógica del programa

| Acción | Efecto |
|--------|--------|
| Vuelo completado | Se acumulan millas automáticamente (`EARN`) según `MilesPerDollar` del programa |
| Redención en reserva | Se descuentan millas del saldo disponible (`REDEEM`) |
| Saldo disponible | `TotalMiles - MillasRedimidas` |
| Subida de tier | Automática cuando `TotalMiles` supera el `MinMiles` del siguiente nivel |

### Tiers disponibles

| Tier | Millas mínimas | Beneficios |
|------|---------------|------------|
| Member | 0 | Acumulación básica |
| Silver | 25.000 | Embarque preferencial, maleta extra gratis |
| Gold | 50.000 | Acceso sala VIP, upgrade preferencial |
| Platinum | 100.000 | Upgrade garantizado, equipaje ilimitado |

### Cómo acceder en el sistema

```
dotnet run
→ [1] Administración
→ PIN de administrador
→ 99 — Reportes
→ Reportes de fidelización (millas)
```

---

## 🗄️ Modelo de Datos

| Entidad | Descripción |
|---------|-------------|
| `Airline` | Aerolíneas registradas |
| `Country / City` | Ubicaciones geográficas |
| `Airport` | Aeropuertos asociados a ciudades |
| `Aircraft` | Aeronaves de cada aerolínea |
| `Route` | Rutas entre aeropuertos |
| `BaseFlight` | Definición base de un vuelo |
| `ScheduledFlight` | Instancia programada de un vuelo |
| `FlightSeat` | Asientos por vuelo programado |
| `Person / Customer` | Datos personales y de cliente |
| `Reservation` | Reservas de clientes en vuelos |
| `Ticket` | Tiquetes emitidos |
| `CheckIn / BoardingPass` | Procesos de embarque |
| `LoyaltyProgram` | Programas de millas por aerolínea |
| `LoyaltyTier` | Niveles de fidelización (Member, Silver, Gold, Platinum) |
| `LoyaltyAccount` | Cuenta de millas por pasajero |
| `LoyaltyTransaction` | Historial de acumulaciones y redenciones |

---

## 🧠 Reglas de Negocio

- ✅ Verificación de existencia de cliente, vuelo y estados antes de crear una reserva.
- ✅ Control de disponibilidad de asientos antes de confirmar reservas.
- ✅ Validación de unicidad en los códigos de reserva.
- ✅ Consistencia en las relaciones entre entidades.
- ✅ Las millas solo se acumulan en vuelos completados (transacción tipo `EARN`).
- ✅ No se pueden redimir más millas de las disponibles en el saldo.
- ✅ El tier sube automáticamente al superar el umbral `MinMiles` del siguiente nivel.

---

## 🔍 Consultas LINQ

### Reportes generales

```csharp
// Vuelos disponibles ordenados por fecha
var vuelosDisponibles = context.ScheduledFlights
    .Where(v => v.FlightStatusId == 1)
    .OrderBy(v => v.DepartureDate)
    .ToList();

// Clientes frecuentes ordenados por número de reservas
var clientesFrecuentes = context.Reservations
    .GroupBy(r => r.CustomerId)
    .Select(g => new {
        Cliente       = g.Key,
        TotalReservas = g.Count()
    })
    .OrderByDescending(x => x.TotalReservas)
    .ToList();

// Total de reservas agrupadas por estado
var reservasPorEstado = context.Reservations
    .GroupBy(r => r.ReservationStatusId)
    .Select(g => new {
        Estado = g.Key,
        Total  = g.Count()
    })
    .ToList();
```

---

### 📊 Consultas LINQ — Examen 5: Analítica de Fidelización

#### 1. Top clientes con más millas acumuladas

Join de 5 tablas ordenado por `TotalMiles` descendente.

```csharp
var topAcumuladas = await (
    from account   in db.LoyaltyAccounts.AsNoTracking()
    join passenger in db.Passengers.AsNoTracking()
        on account.PassengerId equals passenger.Id
    join person    in db.Persons.AsNoTracking()
        on passenger.PersonId equals person.Id
    join program   in db.LoyaltyPrograms.AsNoTracking()
        on account.LoyaltyProgramId equals program.Id
    join tier      in db.LoyaltyTiers.AsNoTracking()
        on account.LoyaltyTierId equals tier.Id
    orderby account.TotalMiles descending
    select new
    {
        Pasajero       = person.FirstName + " " + person.LastName,
        Programa       = program.Name,
        Tier           = tier.Name,
        TotalMillas    = account.TotalMiles,
        Disponibles    = account.AvailableMiles,
        Redimidas      = account.TotalMiles - account.AvailableMiles
    }
).Take(10).ToListAsync();
```

---

#### 2. Clientes que más millas han redimido

Primero se trae el dato plano a memoria (EF Core no traduce `GroupBy` con subqueries), luego se agrupa con LINQ to Objects.

```csharp
// Paso 1: traer datos planos desde la BD
var raw = await (
    from tx        in db.LoyaltyTransactions.AsNoTracking()
    join account   in db.LoyaltyAccounts.AsNoTracking()
        on tx.LoyaltyAccountId equals account.Id
    join passenger in db.Passengers.AsNoTracking()
        on account.PassengerId equals passenger.Id
    join person    in db.Persons.AsNoTracking()
        on passenger.PersonId equals person.Id
    select new
    {
        account.PassengerId,
        FullName          = person.FirstName + " " + person.LastName,
        tx.TransactionType,
        tx.Miles
    }
).ToListAsync();

// Paso 2: agrupar en memoria y calcular EARN vs REDEEM
var topRedimidas = raw
    .GroupBy(x => new { x.PassengerId, x.FullName })
    .Select(g =>
    {
        var earned   = g.Where(t => t.TransactionType == "EARN")  .Sum(t => t.Miles);
        var redeemed = g.Where(t => t.TransactionType == "REDEEM").Sum(t => t.Miles);
        return new
        {
            Pasajero      = g.Key.FullName,
            Redimidas     = redeemed,
            Ganadas       = earned,
            TasaRedencion = earned > 0
                ? Math.Round((double)redeemed / earned * 100, 1)
                : 0.0
        };
    })
    .Where(r => r.Redimidas > 0)
    .OrderByDescending(r => r.Redimidas)
    .Take(10)
    .ToList();
```

---

#### 3. Aerolíneas con mayor volumen de fidelización

Se hace en pasos porque EF Core no traduce `IQueryable` dentro de `let` en el `select`.

```csharp
// Paso 1: traer programas con su aerolínea
var programs = await (
    from program in db.LoyaltyPrograms.AsNoTracking()
    join airline in db.Airlines.AsNoTracking()
        on program.AirlineId equals airline.AirlineId
    select new
    {
        program.Id,
        AirlineName    = airline.Name,
        ProgramName    = program.Name,
        program.MilesPerDollar
    }
).ToListAsync();

// Paso 2: para cada programa calcular cuentas y totales de millas
var volumenAerolineas = new List<object>();
foreach (var prog in programs)
{
    var accountIds = await db.LoyaltyAccounts.AsNoTracking()
        .Where(a => a.LoyaltyProgramId == prog.Id)
        .Select(a => a.Id)
        .ToListAsync();

    var earned = accountIds.Count == 0 ? 0L :
        await db.LoyaltyTransactions.AsNoTracking()
            .Where(tx => accountIds.Contains(tx.LoyaltyAccountId)
                         && tx.TransactionType == "EARN")
            .SumAsync(tx => (long?)tx.Miles) ?? 0L;

    var redeemed = accountIds.Count == 0 ? 0L :
        await db.LoyaltyTransactions.AsNoTracking()
            .Where(tx => accountIds.Contains(tx.LoyaltyAccountId)
                         && tx.TransactionType == "REDEEM")
            .SumAsync(tx => (long?)tx.Miles) ?? 0L;

    volumenAerolineas.Add(new
    {
        Aerolinea       = prog.AirlineName,
        Programa        = prog.ProgramName,
        MillasPorDolar  = prog.MilesPerDollar,
        CuentasActivas  = accountIds.Count,
        MillasGeneradas = earned,
        MillasRedimidas = redeemed
    });
}

var resultado = volumenAerolineas
    .OrderByDescending(x => ((dynamic)x).MillasGeneradas)
    .ToList();
```

---

#### 4. Ranking de viajeros frecuentes

Cuenta transacciones `EARN` por pasajero usando subqueries `let` dentro del `from`.

```csharp
var viajerosFrecuentes = await (
    from account   in db.LoyaltyAccounts.AsNoTracking()
    join passenger in db.Passengers.AsNoTracking()
        on account.PassengerId equals passenger.Id
    join person    in db.Persons.AsNoTracking()
        on passenger.PersonId equals person.Id
    join tier      in db.LoyaltyTiers.AsNoTracking()
        on account.LoyaltyTierId equals tier.Id
    let totalVuelos = db.LoyaltyTransactions
        .Count(tx => tx.LoyaltyAccountId == account.Id
                     && tx.TransactionType == "EARN")
    let totalMillas = db.LoyaltyTransactions
        .Where(tx => tx.LoyaltyAccountId == account.Id
                     && tx.TransactionType == "EARN")
        .Sum(tx => (int?)tx.Miles) ?? 0
    where totalVuelos > 0
    orderby totalVuelos descending, account.TotalMiles descending
    select new
    {
        Pasajero          = person.FirstName + " " + person.LastName,
        NumeroViajero     = passenger.FrequentFlyerNumber ?? "—",
        VuelosCompletados = totalVuelos,
        MillasGanadas     = totalMillas,
        Disponibles       = account.AvailableMiles,
        Tier              = tier.Name
    }
).Take(10).ToListAsync();
```

---

#### 5. Resumen ejecutivo del programa de millas

Múltiples queries simples con `CountAsync` y `SumAsync`, más un join para el pasajero top.

```csharp
// Total de cuentas y programas
var totalCuentas   = await db.LoyaltyAccounts.AsNoTracking().CountAsync();
var totalProgramas = await db.LoyaltyPrograms.AsNoTracking().CountAsync();

// Total millas acumuladas (EARN)
var totalGanadas = await db.LoyaltyTransactions.AsNoTracking()
    .Where(tx => tx.TransactionType == "EARN")
    .SumAsync(tx => (long?)tx.Miles) ?? 0L;

// Total millas redimidas (REDEEM)
var totalRedimidas = await db.LoyaltyTransactions.AsNoTracking()
    .Where(tx => tx.TransactionType == "REDEEM")
    .SumAsync(tx => (long?)tx.Miles) ?? 0L;

// Tasa de redención global
var tasaRedencion = totalGanadas > 0
    ? Math.Round((double)totalRedimidas / totalGanadas * 100, 1)
    : 0.0;

// Pasajero con más millas acumuladas
var pasajeroTop = await (
    from account   in db.LoyaltyAccounts.AsNoTracking()
    join passenger in db.Passengers.AsNoTracking()
        on account.PassengerId equals passenger.Id
    join person    in db.Persons.AsNoTracking()
        on passenger.PersonId equals person.Id
    orderby account.TotalMiles descending
    select new
    {
        Nombre = person.FirstName + " " + person.LastName,
        account.TotalMiles
    }
).FirstOrDefaultAsync();

var resumen = new
{
    TotalCuentas            = totalCuentas,
    TotalProgramas          = totalProgramas,
    MillasTotalesGanadas    = totalGanadas,
    MillasTotalesRedimidas  = totalRedimidas,
    MillasEnCirculacion     = totalGanadas - totalRedimidas,
    TasaRedencionGlobal     = $"{tasaRedencion}%",
    PasajeroTop             = pasajeroTop?.Nombre ?? "—",
    MillasPasajeroTop       = pasajeroTop?.TotalMiles ?? 0
};
```

---

## 📦 Requisitos Previos

- .NET SDK 8.0 o superior
- MySQL Server
- Entity Framework Core CLI

```bash
dotnet tool install --global dotnet-ef
```

---

## 🚀 Instalación y Ejecución

### 1. Clonar el repositorio

```bash
git clone <url-del-repositorio>
cd Sistema_de_gestion_de_tiquetes_Aereos
```

### 2. Configurar la cadena de conexión

Editar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=tiquetes_aereos;User=root;Password=tu_password;"
  }
}
```

### 3. Aplicar migraciones

```bash
dotnet ef database update
```

### 4. Ejecutar

```bash
dotnet run
```

El sistema ejecuta el seeder automáticamente al iniciar, poblando los datos base incluyendo programas de millas, tiers y cuentas de demostración.

---

## 🛠️ Tecnologías

| Tecnología | Rol |
|------------|-----|
| C# / .NET 8 | Lenguaje y plataforma principal |
| Entity Framework Core | ORM y gestión de migraciones |
| MySQL | Motor de base de datos |
| LINQ | Consultas y procesamiento de datos |
| Spectre.Console | Interfaz de consola con tablas y menús |
| appsettings.json | Configuración de la aplicación |
-----------------------------------------------------