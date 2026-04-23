# ✈️ Sistema de Gestión de Tiquetes Aéreos

> Aplicación de consola en **C#** para la gestión integral de tiquetes aéreos, conectada a **MySQL** mediante **Entity Framework Core**.

---

## 📋 Tabla de Contenidos

- [Descripción](#-descripción)
- [Reglas de Negocio](#-reglas-de-negocio)
- [Consultas LINQ](#-consultas-linq)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Ejecución](#-instalación-y-ejecución)


---

## 📖 Descripción

El sistema permite administrar de forma estructurada la operación completa de tiquetes aéreos: vuelos, aerolíneas, rutas, clientes, reservas y emisión de tiquetes. Todos los datos se persisten en una base de datos **MySQL**, gestionada mediante **Entity Framework Core** con consultas **LINQ**.

### Objetivos

- Diseñar un modelo de datos que represente aerolíneas, vuelos, rutas, clientes, reservas y tiquetes.
- Implementar una aplicación de consola con navegación por menús.
- Configurar la conexión entre la aplicación y MySQL vía EF Core.
- Registrar y administrar clientes, vuelos y destinos.
- Gestionar reservas, emisión de tiquetes y control de asientos.
- Implementar consultas y reportes mediante LINQ.
- Aplicar buenas prácticas de organización y separación de responsabilidades.

---

## 🏗️ Arquitectura

El proyecto sigue una **arquitectura por capas** para una organización clara y mantenible:

```
📁 SistemaAereo/
├── 📂 Domain/           → Entidades y reglas de negocio
├── 📂 Application/      → Casos de uso y lógica de aplicación
├── 📂 Infrastructure/   → Persistencia de datos (EF Core / MySQL)
└── 📂 UI/               → Interfaz de consola y menús
```

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
- Administración de clientes

### 🎟️ Reservas
- Creación de reservas con código único
- Asociación con cliente y vuelo programado
- Validación de disponibilidad en tiempo real

### 💺 Gestión de Asientos
- Control de asientos por vuelo
- Validación de disponibilidad antes de confirmar reserva

### 📊 Estados del Sistema
- Estados de reservas (pendiente, confirmada, cancelada, etc.)
- Estados de vuelos (programado, en vuelo, aterrizado, cancelado, etc.)
- Estados de asientos (disponible, ocupado, bloqueado, etc.)

### 📈 Reportes
- Consultas y reportes mediante LINQ
- Clientes frecuentes
- Vuelos disponibles
- Reservas por estado

---

## 🗄️ Modelo de Datos

Las principales entidades del sistema son:

| Entidad | Descripción |
|---|---|
| `Airline` | Aerolíneas registradas |
| `Country` / `City` | Ubicaciones geográficas |
| `Airport` | Aeropuertos asociados a ciudades |
| `Aircraft` | Aeronaves de cada aerolínea |
| `Route` | Rutas entre aeropuertos |
| `BaseFlight` | Definición base de un vuelo |
| `ScheduledFlight` | Instancia programada de un vuelo |
| `FlightSeat` | Asientos por vuelo programado |
| `Person` / `Customer` | Datos personales y de cliente |
| `Reservation` | Reservas de clientes en vuelos |
| `Ticket` | Tiquetes emitidos |
| `CheckIn` / `Boarding` | Procesos de embarque |

---

## 🧠 Reglas de Negocio

El sistema garantiza la integridad de la información mediante las siguientes validaciones:

- ✅ Verificación de existencia de cliente, vuelo y estados antes de crear una reserva.
- ✅ Control de disponibilidad de asientos antes de confirmar reservas.
- ✅ Validación de unicidad en los códigos de reserva.
- ✅ Consistencia en las relaciones entre entidades.

---

## 🔍 Consultas LINQ

Ejemplos de consultas implementadas en el módulo de reportes:

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
        Cliente = g.Key,
        TotalReservas = g.Count()
    })
    .OrderByDescending(x => x.TotalReservas)
    .ToList();

// Total de reservas agrupadas por estado
var reservasPorEstado = context.Reservations
    .GroupBy(r => r.ReservationStatusId)
    .Select(g => new {
        Estado = g.Key,
        Total = g.Count()
    })
    .ToList();
```

---

## 📦 Requisitos Previos

- [.NET SDK](https://dotnet.microsoft.com/download) (versión compatible con el proyecto)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/)
- [Entity Framework Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
dotnet tool install --global dotnet-ef
```

---

## 🚀 Instalación y Ejecución

### 1. Clonar el repositorio

```bash
git clone 
cd sistema-tiquetes-aereos
```


## 🛠️ Tecnologías

| Tecnología | Rol |
|---|---|
| C# / .NET | Lenguaje y plataforma principal |
| Entity Framework Core | ORM y gestión de migraciones |
| MySQL | Motor de base de datos |
| LINQ | Consultas y procesamiento de datos |
| `appsettings.json` | Configuración de la aplicación |

---

