# Guía de Estudio — Módulo de Check-in y Pase de Abordar

Esta guía sirve como brújula para reconstruir el módulo si el profesor borra archivos durante la sustentación. Está organizada en tres bloques: mapa de dependencias, qué escribir por capa si se borran piezas y el flujo end-to-end del dato.

> Convenciones del proyecto que se respetan:
> - Hexagonal por módulo: `Domain/{Aggregate, ValueObject, Repositories}`, `Application/{Interfaces, UseCases, Services}`, `Infrastructure/{Entity, Repository}`, `UI/`.
> - DI por convención (`DependencyInjection.RegisterByConventions`): cualquier clase en un namespace que contenga `.UseCases` se registra como `Scoped`. Los repositorios se enlazan con su interfaz por convención. Los UI no estándar (wizards) se registran a mano en `Program.cs`.
> - `IUnitOfWork` está implementado por `AppDbContext` (alias `CommitAsync` → `SaveChangesAsync`).
> - Para acciones que cruzan varias tablas se usa `_context.Database.CreateExecutionStrategy()` + `BeginTransactionAsync` (ver `TicketRepository.IssueTicketWithHistoryAsync`).

---

## 1) Mapa de Dependencias

### `BoardingPassAggregate` (Dominio)
- **No depende de nada de fuera de `BoardingPass.Domain`.**
- Encapsula: `BoardingPassCode`, `CheckInId`, `GateId?`, `BoardingGroup?`, `FlightSeatId`, `IssuedAt`.
- Importa solo `BoardingPass.Domain.ValueObject` (para `BoardingPassId`).
- Reglas: código no vacío y ≤ 30 chars; `CheckInId > 0`; `GateId` (si llega) > 0; `FlightSeatId > 0`; grupo ≤ 10 chars.

### `IBoardingPassRepository` (puerto en Dominio)
- Métodos: `GetByIdAsync`, `GetAllAsync`, `GetByCheckInAsync`, `GetByCodeAsync`, `BoardingPassCodeExistsAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.
- **Solo depende del Dominio**: `BoardingPassAggregate` y `BoardingPassId`.

### `BoardingPassRepository` (adaptador en Infraestructura)
- Depende de:
  - `AppDbContext` (acceso a `_context.BoardingPasses`).
  - `BoardingPassEntity` (tabla `boarding_pass`).
  - `IBoardingPassRepository` (lo implementa).
- **Atención**: el repositorio NO llama a `CommitAsync`. El `IUnitOfWork` lo invoca el UseCase.

### `CheckInPassengerUseCase` (Aplicación — orquestador)
Es el motor del módulo. Constructor inyecta:
- `AppDbContext _context` — para validar tiquete, vuelo y para historial.
- `IUnitOfWork _unitOfWork` — para `CommitAsync` por etapa.
- `ICheckInRepository _checkInRepository` — `AddAsync`.
- `IBoardingPassRepository _boardingPassRepository` — `BoardingPassCodeExistsAsync`, `AddAsync`.
- `IFlightSeatRepository _flightSeatRepository` — `GetAvailableByFlightAsync` (asignación de asiento).

Tablas que toca dentro de la transacción: `flight_seat`, `check_in`, `boarding_pass`, `ticket`, `ticket_status_history`.

### `CheckInWizardUI` (UI del Portal de Clientes)
Wizard manual (no `ReflectiveModuleUI`). Inyecta:
- `AppDbContext _db` — para listar tiquetes elegibles y armar el pase con datos del vuelo.
- `CheckInPassengerUseCase _checkInPassenger` — el orquestador.
- `ICheckInService _checkInService` — utilidades del módulo CheckIn.
- `IBoardingPassService _boardingPassService` — `GetByCheckInAsync`, `GetAllAsync` (ya filtra por `CurrentUser.CustomerId`).

Se conecta al portal en `ClientPortalApp` con dos acciones nuevas:
- `MainMenuAction.CheckIn` → `RunCheckInAsync`.
- `MainMenuAction.ViewBoardingPass` → `RunViewBoardingPassAsync`.

Ambas viven en una nueva `MainCategory.CheckInArea` con el título "Check-in y pase de abordar".

### Otros puertos consultados
- `ITicketRepository` no se usa directamente — el UseCase consulta `_context.Tickets` para mutar el estado y registrar historial vía `AppDbContext.AddTicketStatusHistoryAsync` (extensión en `Shared.Extensions.StatusHistoryExtensions`).
- `SeatStatusNames` (constantes de `Shared.Constants`) — usamos `OCCUPIED`.

---

## 2) Explicación por Capas

### 2.1 Si borran el UseCase `CheckInPassengerUseCase`

Reescríbelo en `src/Modules/CheckIn/Application/UseCases/CheckInPassengerUseCase.cs`. Pasos exactos:

1. **Validar entrada**: `ticketId > 0` o lanzar `InvalidOperationException`.
2. **Cargar tiquete con tracking** (`_context.Tickets.FirstOrDefaultAsync`) — necesitamos mutarlo después.
3. **Resolver el estado del tiquete** y rechazar si no es `PAID` / `PAGADO` / `ISSUED` (case-insensitive). Esa es la regla "tiquete pagado".
4. **No duplicar check-in**: `_context.CheckIns.AsNoTracking().AnyAsync(c => c.TicketId == ticketId)` debe ser `false`.
5. **Resolver vuelo programado**: `ticket → reservation_detail → reservation → scheduled_flight`. Cada salto es un `FirstOrDefaultAsync`. Si alguno es null, lanzar `InvalidOperationException` describiendo el eslabón.
6. **Validar que el vuelo está habilitado**: leer `flight_status.name` por `scheduledFlight.FlightStatusId`. Aceptar `SCHEDULED`, `BOARDING` o `HABILITADO`. Cualquier otro valor lanza error.
7. **Resolver/asignar asiento**:
   - Si `detail.FlightSeatId > 0` y el asiento pertenece al vuelo, úsalo.
   - Si no, llama a `_flightSeatRepository.GetAvailableByFlightAsync(scheduledFlightId, ct)` y toma el primero. Si no hay → `InvalidOperationException("No hay asientos disponibles…")`.
8. **Resolver IDs de estado destino**:
   - `TicketStatusEntity.Name == "CHECKED_IN"` → `checkedInTicketStatusId`.
   - `CheckInStatusEntity.Name == "CHECKED_IN"` → `checkedInCheckInStatusId`.
   - Si no existen, error claro (significa que falta el seed).
9. **Generar `boardingPassCode` único**: prefijo `BP-` + slice de `Guid.NewGuid().ToString("N")[..10]`. Verificar contra `BoardingPassCodeExistsAsync` con un loop corto por seguridad.
10. **Transacción** (`CreateExecutionStrategy().ExecuteAsync(...)`):
    - `BeginTransactionAsync`.
    - Marcar el asiento `OCCUPIED` (lookup en `seat_status` y update sobre el `FlightSeatEntity`). `_unitOfWork.CommitAsync`.
    - Construir `CheckInAggregate` (status CHECKED_IN, `CheckInTime = DateTime.UtcNow`, `counterNumber`) y `_checkInRepository.AddAsync`. `CommitAsync`.
    - **Recuperar el Id real del CheckIn** con `_checkInRepository.GetByTicketAsync(ticketId, ct)` y guardarlo en `persistedCheckInId`. Ver §2.7 para el detalle del bug.
    - Construir `BoardingPassAggregate` con `persistedCheckInId`, `scheduledFlight.GateId`, código generado, asiento, `IssuedAt = UtcNow`. `_boardingPassRepository.AddAsync`. `CommitAsync`.
    - Mutar `ticket.TicketStatusId = checkedInTicketStatusId`, `ticket.UpdatedAt = now`. Llamar `_context.AddTicketStatusHistoryAsync(ticket.Id, checkedInTicketStatusId, "Check-in realizado", now, ct)`. `CommitAsync`.
    - `transaction.CommitAsync`.
11. **Retornar `CheckInPassengerResult`** con `(CheckInId, TicketId, FlightSeatId, BoardingPassId, BoardingPassCode, GateId, BoardingGroup, IssuedAt)` para que la UI lo pinte sin volver a la base.

### 2.2 Si borran el Repositorio `BoardingPassRepository`

Reescríbelo en `src/Modules/BoardingPass/Infrastructure/repository/BoardingPassRepository.cs`. Las consultas EF Core que **no pueden faltar**:

```csharp
// GetByIdAsync
var entity = await _context.BoardingPasses
    .AsNoTracking()
    .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

// GetAllAsync (más reciente primero — es lo que usa la UI)
var entities = await _context.BoardingPasses
    .AsNoTracking()
    .OrderByDescending(e => e.IssuedAt)
    .ToListAsync(cancellationToken);

// GetByCheckInAsync — única clave funcional con CheckIn
var entity = await _context.BoardingPasses
    .AsNoTracking()
    .FirstOrDefaultAsync(e => e.CheckInId == checkInId, cancellationToken);

// GetByCodeAsync — búsqueda por código de pase
var entity = await _context.BoardingPasses
    .AsNoTracking()
    .FirstOrDefaultAsync(e => e.BoardingPassCode == normalizedCode, cancellationToken);

// BoardingPassCodeExistsAsync — usado por el UseCase para garantizar unicidad
return await _context.BoardingPasses
    .AsNoTracking()
    .AnyAsync(e => e.BoardingPassCode == normalizedCode, cancellationToken);

// AddAsync — copia el agregado a la entidad y lo mete al DbSet (NO hace Commit)
var entity = new BoardingPassEntity
{
    BoardingPassCode = boardingPass.BoardingPassCode,
    CheckInId        = boardingPass.CheckInId,
    GateId           = boardingPass.GateId,
    BoardingGroup    = boardingPass.BoardingGroup,
    FlightSeatId     = boardingPass.FlightSeatId,
    IssuedAt         = boardingPass.IssuedAt
};
await _context.BoardingPasses.AddAsync(entity, cancellationToken);

// UpdateAsync — solo permite cambiar gate y group (resto es inmutable)
entity.GateId        = boardingPass.GateId;
entity.BoardingGroup = boardingPass.BoardingGroup;
_context.BoardingPasses.Update(entity);
```

Reglas no negociables:
- `AsNoTracking()` en todas las lecturas.
- `ToDomain` reconstruye el agregado pasando los 7 campos al constructor (no usar `private` para hidratar — el constructor existe para validar invariantes).
- `Add/Update/Delete` **nunca** llaman `SaveChangesAsync` directamente: el `IUnitOfWork` lo hace en el UseCase.

### 2.3 Si borran la `Entity` o la `EntityConfiguration` de BoardingPass

`BoardingPassEntity` es un POCO simple con `Id`, `BoardingPassCode`, `CheckInId`, `GateId?`, `BoardingGroup?`, `FlightSeatId`, `IssuedAt`. Sin métodos.

`BoardingPassEntityConfiguration` mapea:
- Tabla: `boarding_pass`. PK `boarding_pass_id` con `ValueGeneratedOnAdd()`.
- `boarding_pass_code` `varchar(30)` con índice único `uq_boarding_pass_code`.
- `check_in_id` con índice único `uq_boarding_pass_check_in` (1:1 con CheckIn).
- `gate_id` nullable. FK a `GateEntity` (`fk_bp_gate`, Restrict).
- `boarding_group` `varchar(10)` nullable.
- `flight_seat_id` requerido. FK a `FlightSeatEntity` (`fk_bp_flight_seat`, Restrict).
- `issued_at` `datetime(6)` con default `CURRENT_TIMESTAMP(6)`.
- FK a `CheckInEntity` (`fk_bp_check_in`, Restrict).

Importes obligatorios: `Sistema_de_gestion_de_tiquetes_Aereos.Modules.{Gate, FlightSeat, CheckIn}.Infrastructure.Entity`.

### 2.4 Si borran la UI `CheckInWizardUI`

Reescríbela en `src/Modules/CheckIn/UI/CheckInWizardUI.cs`. Tres rutinas:
- `RunCheckInAsync`: arma la lista de tiquetes elegibles con **LEFT JOIN** contra `flight_seat` y `seat_map` (ver §2.4.1), filtra `FlightStatus` en memoria a `SCHEDULED/BOARDING/HABILITADO`. Pinta una `ConsoleDashboard.NewDataTable()`, pide el tiquete con `SelectionPrompt`, pide `mostrador` opcional, confirma y llama al UseCase. Al final llama a `RenderBoardingPassByCheckInIdAsync`.
- `RunViewBoardingPassAsync`: lista pases del cliente (`_boardingPassService.GetAllAsync` ya filtra por customer), permite escoger uno y lo pinta.
- `RenderBoardingPassByCheckInIdAsync`: panel doble (cabecera "PASE DE ABORDAR"), panel de ruta `OAC → DST`, grid de detalle (pasajero, tiquete, vuelo, fecha, hora, asiento, puerta, grupo, código, emitido). Bordes con `BoxBorder.Double`/`Rounded` y estilos `deepskyblue2`, `seagreen1`, `slateblue1`.

`ConsoleDashboard` ya provee `Success/Warning/Error/Info`, `NewDataTable`, `ShowTablePanel`, `SubScreenTitle`, `NavigationHint`. Reutilízalos para mantener el look del resto del portal.

#### 2.4.1 ¿Por qué LEFT JOIN y no INNER JOIN?

En `BuildEligibleTicketsAsync` el join contra `flight_seat` (y por extensión `seat_map`) **debe ser LEFT JOIN**. La razón:

- En este modelo, un `reservation_detail.flight_seat_id` puede quedar en `0` cuando el cliente reservó pero todavía no asignó asiento (compra anticipada, lista de espera promovida, etc.).
- Si usamos **INNER JOIN** contra `flight_seat`, **esos tiquetes desaparecen** de la lista — el join exige que el FK exista. El usuario ve "no hay tiquetes para check-in" cuando en realidad sí los tiene.
- Eso anula la regla del módulo: *"Si el tiquete no tiene asiento, busca el primero disponible y asígnalo automáticamente"*. La asignación automática solo puede ocurrir si primero **dejamos que el tiquete sin asiento aparezca en la UI**.

Patrón LINQ correcto:

```csharp
join fs in _db.FlightSeats.AsNoTracking()
    on d.FlightSeatId equals fs.Id into fsj
from fs in fsj.DefaultIfEmpty()
join sm in _db.SeatMaps.AsNoTracking()
    on (fs != null ? fs.SeatMapId : -1) equals sm.Id into smj
from sm in smj.DefaultIfEmpty()
...
SeatNumber = sm != null ? sm.SeatNumber : null
```

Reglas para no romper el LEFT JOIN:
1. `into <alias>j` + `from x in <alias>j.DefaultIfEmpty()` — esa es la traducción de LEFT JOIN en LINQ con sintaxis de query.
2. Cualquier acceso posterior a `fs` o `sm` se protege con `?:` (`fs != null ? fs.X : null`).
3. La columna `SeatNumber` en el `select` queda `string?` — la UI ya lo pinta como "[grey]auto[/]" cuando es null, dándole pista al cliente de que recibirá un asiento al hacer check-in.
4. El UseCase es quien luego decide si el asiento existente sirve o si toca llamar a `IFlightSeatRepository.GetAvailableByFlightAsync`. La UI **no** decide nada — solo lista.

### 2.5 Si borran las entradas en `ClientPortalApp`

Hay que tocar 6 sitios en `src/Shared/ui/Client/ClientPortalApp.cs`:
1. `enum MainMenuAction` añade `CheckIn` y `ViewBoardingPass`.
2. `enum MainCategory` añade `CheckInArea`.
3. `MainCategories[]` añade la fila `(MainCategory.CheckInArea, "3) Check-in y pase de abordar")`.
4. `CheckInActions[]` lista las dos acciones nuevas.
5. `PickActionInCategoryAsync` aprende el caso `CheckInArea` (lista, título y hint).
6. El `switch` dentro de `RunAsync` despacha `CheckIn` → `RunCheckInWizardAsync`, `ViewBoardingPass` → `RunViewBoardingPassAsync`. Ambas resuelven el wizard con `_scopeFactory.CreateAsyncScope().ServiceProvider.GetRequiredService<CheckInWizardUI>()`.

### 2.7 ¿Por qué hay que recuperar el Id del CheckIn manualmente?

Este es el bug más sutil del módulo y vale puntos en la sustentación.

**Síntoma**: al construir el `BoardingPassAggregate` justo después de `await _unitOfWork.CommitAsync(...)`, `checkInAgg.Id.Value` sigue siendo `0`. Resultado: el FK `boarding_pass.check_in_id` apunta a 0 y la transacción explota con violación de la FK `fk_bp_check_in`.

**Causa**: nuestro `CheckInRepository.AddAsync` hace `_context.CheckIns.AddAsync(entity)` donde `entity` es un nuevo `CheckInEntity` (POCO de Infraestructura). Cuando MySQL devuelve el `LAST_INSERT_ID()` durante `SaveChangesAsync`, EF Core lo escribe sobre **esa instancia tracked**, no sobre nuestro `CheckInAggregate` (que es un objeto del Dominio que vive en otra referencia y nunca fue tracked). Por eso `checkInAgg.Id.Value` no se rehidrata.

**Fix profesional** — el que está hoy en el código:

```csharp
await _checkInRepository.AddAsync(checkInAgg, cancellationToken);
await _unitOfWork.CommitAsync(cancellationToken);

// El Id real lo asigna MySQL al hacer commit; lo recuperamos por TicketId (uq_check_in_ticket).
var persistedCheckIn = await _checkInRepository.GetByTicketAsync(ticketId, cancellationToken)
    ?? throw new InvalidOperationException(
        $"No se pudo recuperar el check-in recién creado para el tiquete {ticketId}.");
persistedCheckInId = persistedCheckIn.Id.Value;
```

Funciona porque:
- `check_in.ticket_id` tiene el índice único `uq_check_in_ticket`, así que `GetByTicketAsync` siempre devuelve la fila recién creada (no hay ambigüedad).
- La consulta usa `AsNoTracking()` y materializa un agregado nuevo — evitamos cualquier interferencia con el change tracker que ya hizo flush.
- Estamos dentro de la misma transacción abierta por `BeginTransactionAsync`, así que la lectura ve la fila aún no comprometida.

**Alternativas que descartamos**:
- Devolver el `int` desde `AddAsync` después del save: rompería la convención del proyecto, donde los repositorios solo añaden al `DbSet` y el `IUnitOfWork` (en el UseCase) hace `CommitAsync`.
- Exponer la entidad EF al UseCase: rompería la frontera entre Dominio e Infraestructura.
- Usar el aggregate como tracked: el agregado está construido con `private set;` y validaciones; convertirlo en entidad EF lo contaminaría.

> Regla mental: **agregado != entidad**. Mientras EF rehidrata el Id sobre la entidad tracked, nuestro agregado se queda con `0` hasta que lo releemos del repositorio.

### 2.6 Si borran el seed

`BootstrapDataSeeder.EnsureStatusesAsync` debe contener para `TicketStatuses`:
```
ISSUED, PAID, CHECKED_IN, CANCELLED, USED
```
y para `CheckInStatuses`:
```
PENDING, CHECKED_IN, BOARDED
```
Si falta `CHECKED_IN` en cualquiera de las dos tablas, el UseCase falla con un mensaje claro.

---

### 2.8 Si borran la integración con `ClientPortalApp` (recordatorio)

Ya cubierto en §2.5. Solo recuerda que el `CheckInWizardUI` se resuelve **dentro del scope** porque consume `AppDbContext` (Scoped) y los UseCases (Scoped). Crear un nuevo `_scopeFactory.CreateAsyncScope()` por acción es el patrón correcto y es el mismo que usan `RunBookingWizardAsync` y los `RunModuleUiAsync`.

---

## 3) Diagrama de Flujo (input → MySQL)

```
┌──────────────────────────────────────────────────────────────────────────┐
│  Usuario en Portal de Clientes                                           │
│  Menú principal → "3) Check-in y pase de abordar" → "Hacer check-in"     │
└──────────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────────────────┐
│  ClientPortalApp.RunCheckInWizardAsync(ct)                               │
│   - Crea AsyncScope                                                      │
│   - Resuelve CheckInWizardUI desde DI                                    │
└──────────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────────────────┐
│  CheckInWizardUI.RunCheckInAsync(ct)                                     │
│   - BuildEligibleTicketsAsync(customerId)                                │
│   - SelectionPrompt → ticketId                                           │
│   - TextPrompt(opcional) → counterNumber                                 │
└──────────────────────────────────────────────────────────────────────────┘
                                │   ticketId, counterNumber
                                ▼
┌──────────────────────────────────────────────────────────────────────────┐
│  CheckInPassengerUseCase.ExecuteAsync(ticketId, counterNumber, ct)       │
│                                                                          │
│  1) _context.Tickets.FirstOrDefaultAsync(Id == ticketId)                 │
│  2) _context.TicketStatuses → name ∈ {PAID, PAGADO, ISSUED}              │
│  3) _context.CheckIns.AnyAsync(TicketId == ticketId) == false            │
│  4) _context.ReservationDetails → Reservations → ScheduledFlights        │
│  5) _context.FlightStatuses → name ∈ {SCHEDULED, BOARDING, HABILITADO}   │
│  6) ResolveOrAssignSeatAsync                                             │
│      ├─ detail.FlightSeatId válido? → reusar                             │
│      └─ no → _flightSeatRepository.GetAvailableByFlightAsync(...)        │
│  7) Lookup TicketStatus[CHECKED_IN], CheckInStatus[CHECKED_IN]           │
│  8) GenerateUniqueBoardingPassCodeAsync → "BP-XXXXXXXXXX"                │
│                                                                          │
│  TRANSACCIÓN (ExecutionStrategy)                                         │
│  ┌──────────────────────────────────────────────────────────────────┐    │
│  │ BeginTransactionAsync                                            │    │
│  │ ─ FlightSeats.Update(SeatStatusId = OCCUPIED) → CommitAsync      │    │
│  │ ─ CheckIns.AddAsync(new CheckInAggregate(...)) → CommitAsync     │    │
│  │ ─ persistedCheckInId =                                           │    │
│  │     _checkInRepository.GetByTicketAsync(ticketId).Id.Value       │    │
│  │     ↑ recupera el Id real generado por MySQL                     │    │
│  │ ─ BoardingPasses.AddAsync(new BoardingPassAggregate(             │    │
│  │       BoardingPassId(0), code, persistedCheckInId, gate,         │    │
│  │       group, seat, now)) → CommitAsync                           │    │
│  │ ─ ticket.TicketStatusId = CHECKED_IN, UpdatedAt = now            │    │
│  │   AddTicketStatusHistoryAsync(ticket, CHECKED_IN, nota, now)     │    │
│  │       → CommitAsync                                              │    │
│  │ transaction.CommitAsync                                          │    │
│  └──────────────────────────────────────────────────────────────────┘    │
│                                                                          │
│  Retorna CheckInPassengerResult                                          │
└──────────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────────────────┐
│  CheckInWizardUI.RenderBoardingPassByCheckInIdAsync(checkInId, ct)       │
│   - _boardingPassService.GetByCheckInAsync                               │
│   - Joins (CheckIn, Ticket, ReservationDetail, ScheduledFlight,          │
│     BaseFlight, Route, Airports, FlightSeat, SeatMap, Gate, Person)      │
│   - Spectre.Console: panel doble + grid coloreado                        │
└──────────────────────────────────────────────────────────────────────────┘
                                │
                                ▼
                          MySQL persistido
                          (5 tablas tocadas en una sola transacción)
```

### Tablas afectadas en orden

| # | Tabla                  | Operación                      | Por qué                                                |
|---|------------------------|--------------------------------|--------------------------------------------------------|
| 1 | `flight_seat`          | UPDATE `seat_status_id`        | Marcar el asiento como `OCCUPIED`                      |
| 2 | `check_in`             | INSERT                         | Registrar el evento (`ticket_id`, `check_in_status_id`)|
| 3 | `boarding_pass`        | INSERT                         | Emitir el pase con código único e `issued_at`          |
| 4 | `ticket`               | UPDATE `ticket_status_id`      | Cambio a `CHECKED_IN`                                  |
| 5 | `ticket_status_history`| INSERT                         | Auditoría con nota "Check-in realizado"                |

Si alguna de las cinco operaciones falla, el `transaction.CommitAsync` no llega y MySQL hace rollback completo (incluido el cambio de `seat_status`).

---

## 4) Cheat-sheet de invariantes (las que más caen en sustentación)

- "El check-in solo es posible si el tiquete está pagado" → se traduce en: el `ticket_status.name` está en `{PAID, PAGADO, ISSUED}`. ID dinámico (no hardcodear), siempre lookup por `Name`.
- "El vuelo está habilitado" → `flight_status.name` ∈ `{SCHEDULED, BOARDING, HABILITADO}`.
- "Si no tiene asiento, asignar el primer disponible" → `IFlightSeatRepository.GetAvailableByFlightAsync(scheduledFlightId)` y `.FirstOrDefault()`. El propio repo ya filtra por `seat_status.name == "AVAILABLE"`.
- "Cambia el estado del tiquete a Check-in realizado" → upsert al status `CHECKED_IN` en `ticket` y log en `ticket_status_history` con nota `"Check-in realizado"`.
- Código único de pase → `BP-` + 10 hex de `Guid.N`. La unicidad se valida con `BoardingPassCodeExistsAsync` antes de insertar.
- 1 CheckIn por Ticket → unique index `uq_check_in_ticket`. 1 BoardingPass por CheckIn → `uq_boarding_pass_check_in`.

---

## 5) Inyección de dependencias — qué se registra dónde

| Tipo                                    | Registro                                                      |
|-----------------------------------------|---------------------------------------------------------------|
| `AppDbContext`                          | `AddDbContext` en `DependencyInjection.AddSharedInfrastructure` |
| `IUnitOfWork`                           | `AddScoped(sp => sp.GetRequiredService<AppDbContext>())`      |
| `*UseCase` (incluido `CheckInPassengerUseCase`) | Convención: cualquier clase en namespace `.UseCases` se registra como `Scoped` |
| `IBoardingPassRepository → BoardingPassRepository` | Convención: el escáner enlaza la interfaz con la implementación concreta |
| `ICheckInRepository → CheckInRepository`           | Convención (idem)                                              |
| `IFlightSeatRepository → FlightSeatRepository`     | Convención (idem)                                              |
| `IBoardingPassService → BoardingPassService`       | Convención (idem)                                              |
| `ICheckInService → CheckInService`                 | Convención (idem)                                              |
| `CheckInWizardUI` (no tiene interfaz)             | **Registro manual** en `Program.cs` con `AddScoped<CheckInWizardUI>()` |
| `ClientPortalApp`                                  | Manual en `Program.cs`                                         |

---

## 6) Mini-prueba mental para defender el módulo

1. ¿Por qué BoardingPass se relaciona con Ticket vía CheckIn y no con FK directa?  
   Porque el modelo separa responsabilidades: `CheckIn` es el evento (cuándo, mostrador, estado) y `BoardingPass` es el documento emitido. Saltarse `CheckIn` rompería la trazabilidad y haría imposible aplicar la regla "1 check-in por tiquete".

2. ¿Qué pasa si el seat ya estaba `RESERVED`?  
   Se acepta y se promueve a `OCCUPIED`. La regla solo bloquea cuando no hay asiento o el asignado no pertenece al vuelo.

3. ¿Por qué `ExecutionStrategy`?  
   Pomelo MySQL tiene retry-on-failure habilitado (`EnableRetryOnFailure`). EF exige envolver las transacciones manuales con la estrategia para que el reintento las repita completas.

4. ¿Qué ID de TicketStatus tendría "Pagado"?  
   No se hardcodea. Se hace lookup por nombre cada vez. En la base actual, `PAID` (recién sembrado) o el `ISSUED` que ya implica pagos aprobados (ver `CreateTicketUseCase`, que solo deja emitir si el `paidTotal >= expectedTotal`).

5. ¿Cómo se vería la query del repositorio de asientos disponibles?  
   ```csharp
   _context.FlightSeats
     .AsNoTracking()
     .Where(e => e.ScheduledFlightId == flightId && e.SeatStatusId == availableStatusId)
     .OrderBy(e => e.SeatMapId)
     .ToListAsync(ct);
   ```
   `availableStatusId` se resuelve con `_context.SeatStatuses.Where(ss => ss.Name == "AVAILABLE")` (ver `FlightSeatRepository.GetAvailableByFlightAsync`).

6. ¿Por qué el listado de tiquetes elegibles usa LEFT JOIN contra `flight_seat`?  
   Porque un detalle puede no tener asiento (`flight_seat_id = 0`). Con INNER JOIN esos tiquetes desaparecerían de la UI y nunca llegarían al UseCase, anulando la regla "asignar el primer asiento disponible automáticamente". El LEFT JOIN garantiza que el tiquete aparece con asiento en blanco y la columna se pinta `[grey]auto[/]`.

7. ¿Por qué después del primer `CommitAsync` el `Id` del agregado sigue en 0?  
   Porque EF Core escribe el `LAST_INSERT_ID()` sobre la **entidad tracked** (el POCO `CheckInEntity` que armamos dentro del repositorio), no sobre el agregado del Dominio. El agregado vive en otra referencia y no participa del change tracker. Solución: releer por `TicketId` (índice único `uq_check_in_ticket`) para obtener el `Id` real.

---

## 7) Archivos del módulo (mapa rápido)

```
src/Modules/BoardingPass/
├── Domain/
│   ├── aggregate/BoardingPassAggregate.cs        ← invariantes del pase
│   ├── valueObject/BoardingPassId.cs             ← VO id
│   └── Repositories/IBoardingPassRepository.cs   ← puerto
├── Application/
│   ├── Interfaces/IBoardingPassService.cs        ← contrato + DTO
│   ├── UseCases/{Create,Delete,Update,GetAll,GetById,GetByCheckIn,GetByCode}BoardingPassUseCase.cs
│   └── Services/BoardingPassService.cs           ← fachada
├── Infrastructure/
│   ├── entity/BoardingPassEntity.cs              ← POCO
│   ├── entity/BoardingPassEntityConfiguration.cs ← mapping
│   └── repository/BoardingPassRepository.cs      ← adaptador
└── UI/BoardingPassConsoleUI.cs                   ← módulo CRUD reflectivo

src/Modules/CheckIn/
├── Domain/                                       (igual estructura que BoardingPass)
├── Application/UseCases/CheckInPassengerUseCase.cs   ← orquestador del módulo
├── Application/UseCases/{Create,Delete,Update,GetAll,GetById,GetByTicket,ChangeStatus}CheckInUseCase.cs
├── Application/Services/CheckInService.cs
├── Infrastructure/                                (entity + config + repository)
└── UI/CheckInWizardUI.cs                         ← wizard del Portal de Clientes

src/Shared/ui/Client/ClientPortalApp.cs           ← integra el wizard al menú
src/Shared/Infrastructure/BootstrapDataSeeder.cs  ← agrega CHECKED_IN/PAID
Program.cs                                        ← registra CheckInWizardUI
```

> Si tras borrar archivos vuelve a aparecer un error de compilación tipo "no se encuentra `IBoardingPassRepository`", revisa primero los `using` (los namespaces son largos: `Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories`). Es la falla más común.

---

## 8) Análisis técnico — Gestión de estados de tiquete

El tiquete es la entidad transaccional central del sistema y su estado refleja el ciclo de vida del derecho de viaje. La tabla `ticket_status` contiene el catálogo seedeado:

| Nombre        | Significado de negocio                                                       | Quién lo escribe                                  |
|---------------|------------------------------------------------------------------------------|---------------------------------------------------|
| `ISSUED`      | Tiquete emitido tras pagar la reserva. Implícitamente "pagado".              | `CreateTicketUseCase.IssueTicketWithHistoryAsync` |
| `PAID`        | Estado explícito alternativo cuando un proceso de cobro confirma el pago.    | Cualquier flujo que use `ChangeTicketStatusUseCase` |
| `CHECKED_IN`  | El pasajero hizo check-in y existe un `boarding_pass` asociado.              | `CheckInPassengerUseCase`                         |
| `CANCELLED`   | El tiquete fue anulado por reembolso o cancelación de la reserva.            | Flujos de reembolso/cancelación                   |
| `USED`        | El pasajero abordó el vuelo (transición posterior al check-in).              | Reservado para el módulo de embarque             |

### Reglas de transición (state machine implícita)

```
        emitir                     check-in                   embarque
ISSUED ──────────────► CHECKED_IN ──────────► USED
   │
   │ pago confirmado externo
   ▼
  PAID  ─── (también permite check-in) ───┐
                                          ▼
                                       CHECKED_IN
   │
   │ reembolso/cancelación (cualquier estado anterior a USED)
   ▼
CANCELLED
```

Lo que hace el `CheckInPassengerUseCase`:
1. Lee `ticket.TicketStatusId` con tracking (necesitamos mutarlo).
2. Resuelve el nombre del estado vía `ticket_status` y exige que esté en `{ISSUED, PAID, PAGADO}`. La lista es flexible para soportar dialectos del catálogo y no acoplarse a un único string.
3. Valida que **no exista ya un check-in** para el tiquete (el índice único `uq_check_in_ticket` lo respalda en BD; aquí se valida en aplicación para devolver un error claro al usuario antes de tocar la transacción).
4. Tras emitir el pase, escribe `ticket.TicketStatusId = checkedInTicketStatusId` y registra una fila en `ticket_status_history` con la nota `"Check-in realizado"`. Esa tabla es el log de auditoría que permite reconstruir cualquier transición.

### Por qué el lookup del estado es por nombre, no por Id

En todo el módulo nunca se hardcodea un Id. La razón es que cada base (dev, staging, prod) puede tener autoincrementales distintos. Hardcodear `ticketStatusId = 3` para `CHECKED_IN` es un bug latente: en cuanto migres a otra base con orden de seed distinto, el sistema marcará tiquetes con el estado equivocado. La regla del proyecto es:

```csharp
var statusId = await _context.TicketStatuses
    .AsNoTracking()
    .Where(s => s.Name == "CHECKED_IN")
    .Select(s => s.Id)
    .FirstOrDefaultAsync(cancellationToken);
if (statusId == 0)
    throw new InvalidOperationException("...");
```

El cero es la sentinela de "no existe en el catálogo". Si aparece, es un fallo de seed, no de datos.

### Auditoría (`ticket_status_history`)

Cada cambio de estado del tiquete deja huella en `ticket_status_history` vía la extensión `AppDbContext.AddTicketStatusHistoryAsync`. Esta tabla guarda `(ticket_id, ticket_status_id, changed_at, notes)`. La nota `"Check-in realizado"` es la firma del UseCase y permite responder en sustentación: *"¿cómo sé que un tiquete pasó por mi módulo?"* — `SELECT * FROM ticket_status_history WHERE ticket_id = X AND notes = 'Check-in realizado'`.

---

## 9) Análisis técnico — Recuperación de identidades generadas por la BD

> Esta sección expande §2.7 con el detalle conceptual.

### El problema en frío

`AppDbContext` está configurado con la convención de `EFCore.NamingConventions` (snake_case) y todas las PKs son autoincrementales. Mira por ejemplo `TicketEntityConfiguration`:

```csharp
builder.Property(e => e.Id)
       .HasColumnName("ticket_id")
       .ValueGeneratedOnAdd();
```

`ValueGeneratedOnAdd()` le dice a EF que el motor (MySQL) genera el Id durante el `INSERT`. Después del `SaveChangesAsync`, EF lee el `LAST_INSERT_ID()` y lo proyecta sobre la propiedad `Id` de **la entidad tracked** (la `TicketEntity` que se añadió al `DbSet`).

### Por qué falla con agregados del Dominio

Nuestros repositorios siguen un patrón de mapeo estricto:

```csharp
public async Task AddAsync(CheckInAggregate checkIn, CancellationToken ct)
{
    var entity = new CheckInEntity { ... };       // ← objeto NUEVO de Infra
    await _context.CheckIns.AddAsync(entity, ct);
}
```

`entity` es lo que EF rastrea. El `CheckInAggregate` que el UseCase tiene en su variable local es **otra referencia** que el change tracker nunca conoció. Cuando el commit termina, EF rehidrata `entity.Id`, pero `checkInAgg.Id.Value` se queda con el `0` que le pasamos al construirlo.

Esto es **deliberado**: el agregado del Dominio no debe ser una entidad EF. Mezclar ambos romper la frontera hexagonal y obliga a poner anotaciones de EF en el Dominio, contaminándolo.

### La recuperación correcta

Necesitamos un canal "frío" para preguntarle a MySQL: *"¿cuál es el Id del CheckIn que acabo de insertar?"*. Las opciones son:

| Opción                                              | Veredicto                                                             |
|-----------------------------------------------------|------------------------------------------------------------------------|
| `_context.Entry(entity).Property("Id").CurrentValue`| Rompe el repositorio (necesitaría devolver `entity` al UseCase).      |
| `SELECT LAST_INSERT_ID()` raw                       | Funciona pero acopla el código a MySQL y a Pomelo.                    |
| **Re-leer por una clave natural única**             | **Patrón elegido**: portable, semánticamente claro, sin acoplamientos. |

En nuestro caso, `check_in.ticket_id` tiene índice único (`uq_check_in_ticket`). El UseCase ya conoce el `ticketId`. Por tanto:

```csharp
await _checkInRepository.AddAsync(checkInAgg, cancellationToken);
await _unitOfWork.CommitAsync(cancellationToken);

var persistedCheckIn = await _checkInRepository.GetByTicketAsync(ticketId, cancellationToken)
    ?? throw new InvalidOperationException(
        $"No se pudo recuperar el check-in recién creado para el tiquete {ticketId}.");
var persistedCheckInId = persistedCheckIn.Id.Value;
```

Garantías que este patrón ofrece:
- **Atomicidad respetada**: la lectura ocurre dentro de la transacción abierta por `BeginTransactionAsync`, así que ve la fila aún no comprometida y no hay riesgo de race condition con otro proceso.
- **Determinismo**: el índice único elimina ambigüedad. No hace falta `OrderBy desc` ni `Take(1)` con riesgo.
- **Aislamiento**: el repositorio sigue siendo el único que mapea entidad ↔ agregado. El UseCase no toca EF directamente.
- **Diagnóstico**: si por alguna razón el commit no escribió la fila (ej. trigger MySQL la rechazó), el `?? throw` aborta la transacción con un mensaje accionable.

### Aplicabilidad a otras tablas con autoincremental

El mismo patrón sirve para cualquier tabla con una PK generada por la BD si tienes una clave natural única para releer:

| Entidad         | Clave natural única para releer                  |
|-----------------|--------------------------------------------------|
| `Ticket`        | `ticket_code` (`uq_ticket_code`)                 |
| `Reservation`   | `reservation_code`                               |
| `BoardingPass`  | `boarding_pass_code` (`uq_boarding_pass_code`)   |
| `CheckIn`       | `ticket_id` (`uq_check_in_ticket`)               |
| `FlightSeat`    | (no aplica — la combinación lógica no es única en el negocio actual) |

Para tablas sin clave natural única conviene devolver el `Id` desde el repositorio o usar el `Entry(entity).Property` antes de soltar la referencia.

---

## 10) Diagrama conceptual — Orquestación desde la capa de Aplicación

Este diagrama muestra **roles**, no llamadas a métodos. Es el que sirve para defender por qué el módulo está organizado así.

```
                            ┌──────────────────────────────────────┐
                            │       UI / Presentación              │
                            │   (ClientPortalApp + Wizard)         │
                            │                                      │
                            │  · Recoge datos del usuario          │
                            │  · Muestra resultados                │
                            │  · NO contiene reglas de negocio     │
                            └──────────────────┬───────────────────┘
                                               │  ticketId, counterNumber
                                               ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│                       Capa de Aplicación (Orquestador)                       │
│                       CheckInPassengerUseCase                                │
│                                                                              │
│   Responsabilidad: COORDINAR colaboradores para cumplir un caso de uso.      │
│   No contiene reglas de invariantes (eso está en el Dominio) ni queries      │
│   primitivas (eso está en repositorios), pero SÍ contiene la receta.         │
│                                                                              │
│   ┌──────────────┐  ┌─────────────────┐  ┌──────────────┐  ┌──────────────┐  │
│   │ Validaciones │→ │ Resolución de   │→ │ Mutaciones   │→ │ Composición  │  │
│   │ de pre-      │  │ identidad y     │  │ atómicas en  │  │ del resultado│  │
│   │ condición    │  │ catálogo        │  │ transacción  │  │ para la UI   │  │
│   └──────────────┘  └─────────────────┘  └──────────────┘  └──────────────┘  │
│                                                                              │
│   Colaboradores inyectados (todos por interfaz, nunca por implementación):   │
│     ICheckInRepository      → escribir CheckIn + releer Id por TicketId      │
│     IBoardingPassRepository → escribir BoardingPass + verificar unicidad     │
│     IFlightSeatRepository   → consultar asientos disponibles                 │
│     IUnitOfWork             → cerrar cada etapa de la transacción            │
│     AppDbContext            → consultas planas de catálogos (statuses)       │
│                                                                              │
└────────────────────────────────┬─────────────────────────────────────────────┘
                                 │
                  ┌──────────────┼─────────────┬───────────────┐
                  ▼              ▼             ▼               ▼
         ┌────────────────┐ ┌─────────┐ ┌─────────────┐ ┌──────────────┐
         │   Dominio      │ │ Domain  │ │  Domain     │ │   Domain     │
         │ CheckIn.       │ │ Boarding│ │ FlightSeat. │ │  Ticket.     │
         │  Aggregate     │ │ Pass.   │ │  Aggregate  │ │  Entity*     │
         │                │ │ Aggreg. │ │             │ │              │
         │  - invariantes │ │  - inv. │ │  - inv.     │ │  (*tracked   │
         │  - reglas      │ │  - reg. │ │             │ │   por EF en  │
         │    inmutables  │ │         │ │             │ │   este caso) │
         └────────────────┘ └─────────┘ └─────────────┘ └──────────────┘
                  ▲              ▲             ▲               ▲
                  │              │             │               │
                  └──────────────┴─────────────┴───────────────┘
                                 │  mapeo
                                 ▼
                      ┌────────────────────────┐
                      │   Infraestructura      │
                      │   (Repositorios EF)    │
                      │                        │
                      │  Traducen agregado ↔   │
                      │  entity y disparan     │
                      │  consultas al DbContext│
                      └───────────┬────────────┘
                                  │  EF Core + Pomelo
                                  ▼
                          ┌──────────────────┐
                          │      MySQL       │
                          │  (5 tablas en    │
                          │  una transacción)│
                          └──────────────────┘
```

### Lectura del diagrama

- **La UI nunca habla con la BD**. Solo conoce al UseCase y a los servicios de aplicación. Esto permite swapear Spectre.Console por una API REST sin tocar reglas.
- **El UseCase nunca habla con MySQL directamente**. Habla con interfaces (`I*Repository`) y con el `AppDbContext` solo para lecturas planas de catálogos (no para escribir agregados — eso lo delega al repositorio).
- **El Dominio no sabe que existe EF Core**. Los agregados son POCOs con invariantes; los `*Entity` son el reflejo en infraestructura.
- **El `IUnitOfWork` es la batuta**. Cada `CommitAsync` cierra una etapa; `BeginTransactionAsync` la envuelve para que MySQL pueda hacer rollback si la receta falla a la mitad.

### Por qué este reparto facilita las pruebas

- Para testear `CheckInPassengerUseCase` solo necesitas mocks de los repositorios y un `AppDbContext` en memoria (o un fake) — no necesitas levantar MySQL.
- Para testear el `BoardingPassAggregate` solo necesitas `xUnit` — no hace falta nada de la infraestructura.
- Para testear el `BoardingPassRepository` solo necesitas un `AppDbContext` apuntado a SQLite o MySQL en docker — no hace falta el UseCase.

### Por qué este reparto sobrevive a refactors

- Cambiar de MySQL a PostgreSQL es un cambio en `DependencyInjection.AddSharedInfrastructure` (provider del DbContext) y en las migraciones — el UseCase no se entera.
- Cambiar la consola Spectre por una API web es escribir un nuevo controlador que llame a `CheckInPassengerUseCase.ExecuteAsync(ticketId, counterNumber, ct)` — el dominio y la aplicación no se enteran.
- Cambiar la regla de "asignar el primer disponible" por "asignar el de menor distancia al pasillo" es un cambio en `ResolveOrAssignSeatAsync` — la UI y los repositorios no se enteran.

Esa es la promesa de la arquitectura hexagonal: cada capa cambia por sus propias razones, no por las razones de las otras.

---

## 11) Sincronización de Identidad de Objetos Relacionados

§9 cubre por qué hay que releer el `CheckIn` para obtener su Id real. La misma trampa aplica al `BoardingPass`: si lo construimos con `new BoardingPassId(0)` y solo confiamos en EF para rehidratar, la **referencia del agregado** que devolvemos a la UI conserva el `0`. La UI termina mostrando `BoardingPassId: 0` aunque la fila en MySQL ya tiene su `boarding_pass_id` real.

### Patrón aplicado en cadena

Cada agregado nuevo que entra a la transacción se relee por su clave natural única antes de cerrar la operación:

```csharp
// CheckIn → releer por TicketId (uq_check_in_ticket)
await _checkInRepository.AddAsync(checkInAgg, ct);
await _unitOfWork.CommitAsync(ct);
var persistedCheckIn = await _checkInRepository.GetByTicketAsync(ticketId, ct)
    ?? throw new InvalidOperationException("...");
persistedCheckInId = persistedCheckIn.Id.Value;

// BoardingPass → releer por CheckInId (uq_boarding_pass_check_in)
await _boardingPassRepository.AddAsync(boardingPass, ct);
await _unitOfWork.CommitAsync(ct);
var persistedBoardingPass = await _boardingPassRepository.GetByCheckInAsync(persistedCheckInId, ct)
    ?? throw new InvalidOperationException("...");
persistedBoardingPassId = persistedBoardingPass.Id.Value;
```

### Por qué este patrón merece su propia sección

Cuando un caso de uso crea **varios agregados que se referencian entre sí** (CheckIn → BoardingPass), la cadena de identidades se vuelve crítica:
- El BoardingPass necesita el Id real del CheckIn para que la FK `boarding_pass.check_in_id` apunte a la fila correcta. Si pasamos `0`, la transacción explota.
- La capa de presentación necesita el Id real del BoardingPass para enlazar pantallas posteriores ("ver detalle del pase #X", "compartir pase #X"). Si recibe `0`, todos los enlaces apuntan a un registro inexistente.

El nombre formal del patrón es **Sincronización de Identidad de Objetos Relacionados**: cada objeto persistido se reasigna desde la BD antes de pasarlo al siguiente paso del flujo o a la capa que lo va a consumir. Garantiza que **la capa de presentación siempre reciba datos persistidos reales**, nunca placeholders transitorios.

### Cómo verificar en sustentación que el fix funciona

Una prueba mental que vale puntos:
1. Hacer un check-in y mirar el log de pantalla. ¿`BoardingPassId` es distinto de cero? ✓
2. Volver al menú principal → "Ver mi pase de abordar" → ¿el Id mostrado coincide con el de paso 1? ✓
3. Consultar MySQL: `SELECT boarding_pass_id FROM boarding_pass ORDER BY boarding_pass_id DESC LIMIT 1;` → ¿coincide con lo que pintó la UI? ✓

Si los tres números coinciden, la sincronización de identidad está funcionando en cadena.

---

## 12) Manejo de Excepciones Global en UI

Toda capa de presentación que invoca lógica de negocio + IO debe tener un **escudo de tres capas** que decida cómo se le habla al usuario según el tipo de fallo. En `CheckInWizardUI` el escudo cubre `RunCheckInAsync` y `RunViewBoardingPassAsync` completos — incluida la carga inicial de datos (`BuildEligibleTicketsAsync`, `_boardingPassService.GetAllAsync`).

### Estructura

```csharp
try
{
    // toda la lógica del wizard, desde la primera consulta a la BD
    // hasta el render del pase, vive aquí dentro.
}
catch (InvalidOperationException ex)
{
    // Errores de negocio del UseCase: el mensaje ya viene
    // listo para mostrarse al usuario en español.
    ConsoleDashboard.Error(ex.Message);
}
catch (OperationCanceledException)
{
    // El usuario o el host pidió cancelar. No es un error que mostrar.
}
catch (Exception)
{
    // Red caída, MySQL no responde, dependencia null, bug residual.
    // Mensaje genérico y volvemos al menú principal sin tumbar la app.
    ConsoleDashboard.Error("Ha ocurrido un error inesperado en el sistema. Por favor, intente más tarde.");
}
```

### Por qué cada `catch` está donde está

| Tipo                          | Origen típico                                                | Decisión                                                  |
|-------------------------------|--------------------------------------------------------------|-----------------------------------------------------------|
| `InvalidOperationException`   | Reglas violadas en el UseCase (tiquete no pagado, vuelo cancelado, sin asientos, doble check-in) | Mostrar el mensaje del `ex.Message` directo — el UseCase ya redactó la frase pensando en el usuario. |
| `OperationCanceledException`  | El `CancellationToken` se disparó (Ctrl+C, host shutdown).   | Salir silenciosamente. No es ni error ni dato útil.       |
| `Exception` (genérico)        | DbUpdateException, MySqlException, NullReferenceException, IOException, fallo de red, etc. | Mensaje genérico. NO exponer el `ex.Message` técnico al usuario, eso es ruido y posible filtración de datos. |

### Por qué la carga inicial debe estar dentro del try

Es la trampa que reportó QA. Antes el código era:

```csharp
// FUERA del try → si falla, la app muere
var elegibles = await BuildEligibleTicketsAsync(customerId, ct);

try
{
    // dentro: solo la llamada al UseCase
    await _checkInPassenger.ExecuteAsync(...);
}
catch (InvalidOperationException) { ... }
```

Si `BuildEligibleTicketsAsync` lanzaba — porque MySQL se cayó, porque hubo timeout, porque la BD devolvió un schema inconsistente — la excepción burbujeaba al `ClientPortalApp`, y de ahí al `MainMenu.RunAsync`, que la propagaba hasta `Program.cs`. Resultado: process termination, el cliente ve un crash y pierde la sesión.

La regla es simple: **todo lo que toque red, BD o disco vive dentro del try del wizard**. El único código fuera del try son los `Console.Clear`, los headers visuales y las validaciones puramente locales (autenticación), que no pueden lanzar.

### Por qué no atrapar `Exception` directamente desde el principio

Atrapar todo desde la primera línea ocultaría errores de programación (`NullReferenceException` real en nuestro código, `ArgumentException` de validación de Spectre.Console, etc.) detrás del mensaje genérico. La pirámide `InvalidOperationException → OperationCanceledException → Exception` ordena los catch del más específico al más general, exactamente como recomienda CLR:
- Lo conocido se trata con su propio mensaje.
- Lo cancelable se trata como ruido.
- Lo desconocido se contiene con un mensaje seguro.

### Cómo verificar en sustentación que el escudo funciona

1. Apagar MySQL antes de entrar al wizard. Esperado: aparece "Ha ocurrido un error inesperado…", el menú vuelve a salir.
2. Entrar al wizard sin tiquetes pagados. Esperado: aparece el `Warning` "No hay tiquetes pagados pendientes…" — eso es flujo normal, no excepción.
3. Forzar un tiquete con estado `CANCELLED` y pasar su Id manualmente. Esperado: `InvalidOperationException` con "El check-in solo está disponible para tiquetes pagados…".

Los tres escenarios deben terminar **siempre con el menú principal a la vista**, jamás con el proceso muerto.

---

## 13) Trampa de DI — los DTOs y Results no van en `.UseCases`

`Shared.Infrastructure.DependencyInjection.RegisterByConventions` registra como `Scoped` todo tipo concreto cuyo namespace contenga `.UseCases`:

```csharp
foreach (var type in concreteTypes.Where(t => t.Namespace?.Contains(".UseCases", StringComparison.Ordinal) == true))
{
    services.AddScoped(type);
}
```

Eso es perfecto para los UseCases reales, pero **un `record` declarado al final del archivo de un UseCase también es una clase concreta y también vive en ese namespace**. El contenedor lo registra y, cuando arranca con `ValidateOnBuild` activo (default en `Host.CreateApplicationBuilder` durante Development), revienta porque no puede construir el `record`: sus parámetros son primitivos (`int`, `string`, `DateTime`) que DI no sabe resolver.

### Síntoma típico

Excepción tipo:

```
Some services are not able to be constructed
Error while validating the service descriptor
'ServiceType: ...CheckInPassengerResult Lifetime: Scoped':
Unable to resolve service for type 'System.Int32' while attempting
to activate '...CheckInPassengerResult'.
```

### La regla del proyecto

`Application.UseCases` es **solo para UseCases**. Cualquier compañero (DTOs, Results, command/query objects) debe vivir en otro sub-namespace de `Application/`. En este módulo, `CheckInPassengerResult` se ubica en:

```
src/Modules/CheckIn/Application/Results/CheckInPassengerResult.cs
   └── namespace: …Application.Results
```

Patrones equivalentes válidos (el proyecto los usa todos):
- DTOs como `CheckInDto` o `TicketDto` viven en el archivo `I*Service.cs` dentro de `Application.Interfaces`.
- Results de UseCases que devuelven varios datos viven en `Application.Results`.
- Commands/Queries (si llegaran a introducirse) podrían vivir en `Application.Contracts`.

Lo único que **no** puede vivir en `.UseCases` es algo que no sea un UseCase concreto orquestador.

### Cómo se diagnostica rápido

Si al arrancar la app aparece *"Unable to resolve service for type 'System.Xxx' while attempting to activate '…AlgúnTipo'"* y `AlgúnTipo` no es un UseCase de verdad, casi siempre es un DTO/record colado en el namespace `.UseCases`. La solución es siempre la misma: moverlo de carpeta y de namespace.

---

## 14) Datos demo precargados para sustentación

`BootstrapDataSeeder.EnsureCheckInDemoDataAsync` se ejecuta automáticamente al arrancar la app y siembra el escenario completo para que la sustentación del módulo no requiera crear datos a mano. Es **idempotente**: si ya existe la persona demo (documento `123456`, tipo `CC`), se salta sin tocar nada.

### Credenciales y códigos a memorizar

| Concepto                  | Valor                            |
|---------------------------|----------------------------------|
| **Username** (campo de login en `UserEntity`) | `admin_demo`     |
| **Contraseña**            | `demo1234`                       |
| **Rol**                   | Administrador (acceso total)     |
| **Documento**             | `123456` (CC)                    |
| **Nombre del pasajero**   | Usuario Test                     |
| **Email del cliente**     | demo@checkin.test                |
| **Código de reserva**     | `RES-DEMO-001`                   |
| **Código de tiquete**     | `TKT-DEMO-001`                   |
| **Estado del tiquete**    | `PAID` (pagado, listo para check-in) |
| **Código del vuelo**      | `DM-9001`                        |
| **Ruta**                  | BOG → MDE                        |
| **Fecha de salida**       | hoy + 7 días, 10:00 (siempre futura)  |
| **Aeronave**              | `HK-DEMO` (Boeing 737-800)       |
| **Asientos disponibles**  | 10 filas en `flight_seat` con estado `AVAILABLE` |

> Los Ids numéricos (PK autoincrementales) los asigna MySQL en cada base. Para identificar registros en sustentación se usan las **claves naturales** (códigos y documento), que son estables y memorizables.

### Vinculación Usuario → Tiquete

El sistema autentica por `UserEntity.Username` (no por email). El tiquete `TKT-DEMO-001` queda asociado a `admin_demo` por la siguiente cadena, toda creada por el seeder en una sola corrida:

```
admin_demo (User.Username)
    └── PersonId ─────────────────────► person (Usuario Test, doc 123456)
                                            │
                                            ├──► customer (PersonId match)
                                            │       └── Reservation (CustomerId)
                                            │             └── ReservationDetail
                                            │                   └── Ticket TKT-DEMO-001 [PAID]
                                            │
                                            └──► passenger (PersonId match) ◄── ReservationDetail.PassengerId
```

`CheckInWizardUI.BuildEligibleTicketsAsync` filtra por `r.CustomerId == CurrentUser.CustomerId.Value`, y `CurrentUser.CustomerId` se setea al hacer login con `admin_demo`. Resultado: el wizard ve `TKT-DEMO-001` como elegible apenas inicia sesión.

### Cómo correr la demo paso a paso

1. Arrancar la app (`dotnet run`).
2. En la pantalla de inicio elegir **Portal de clientes**.
3. Iniciar sesión con username `admin_demo` y contraseña `demo1234`.
4. Menú principal → **3) Check-in y pase de abordar** → **Hacer check-in**.
5. Aparece la tabla "Tiquetes elegibles para check-in" con una fila:
   `TKT-DEMO-001 · Vuelo DM-9001 · <fecha+7d> · (id <variable>)`.
6. Seleccionarla → mostrador opcional → confirmar.
7. La consola pinta el pase de abordar con el panel doble en `deepskyblue2` y el grid de detalle.
8. Volver al menú → **Ver mi pase de abordar** → seleccionar el pase → se vuelve a renderizar.

### Por qué el seed está donde está

- Vive en `BootstrapDataSeeder` para concentrar **todo** el bootstrapping en un solo archivo (mismo punto donde se siembran roles, estados, países, etc.).
- Se llama **después** de `EnsureMasterDataAsync` para garantizar que existan los catálogos de infraestructura que necesita (aeropuertos `BOG`/`MDE`, aerolínea `AV`, AircraftType `737-800`, seat maps).
- Catálogos pequeños y críticos (`Administrador`, `PAID`, `CHECKED_IN` en `TicketStatus` y `CheckInStatus`) se **force-create** dentro del propio método si llegan a faltar — la demo no se rinde por un seed previo borrado.
- Catálogos de infraestructura que no se pueden auto-crear (BOG, MDE, AV, 737-800, AVAILABLE, etc.) lanzan `InvalidOperationException` con un mensaje específico que apunta a qué método previo debe verificarse. Nada de fallos silenciosos.

### Idempotencia: por Username, no por email ni documento

La regla anterior idempotenciaba por documento (`123456`). El nuevo seeder lo hace por `User.Username == "admin_demo"`:

```csharp
if (await db.Users.AsNoTracking().AnyAsync(u => u.Username == demoUsername, ct))
    return;
```

Razones:
1. El campo de login es `Username` (verificado en `UserEntity.cs`), no `Email` ni `NombreUsuario`.
2. Si la persona/customer/passenger ya existen (por una corrida previa interrumpida), el seeder los **reutiliza** y solo crea lo que falte. La idempotencia "completa" se evalúa contra el `Users` porque ese es el último anclaje del flujo de login.
3. Hace que el reset manual entre corridas sea trivial: `DELETE FROM users WHERE username = 'admin_demo';` y a la siguiente arrancada todo se vuelve a sembrar.

### Política de errores: throws explícitos

Cualquier precondición que el seeder no puede auto-crear lanza `InvalidOperationException`. El mensaje incluye:
- Qué catálogo falta.
- Qué método previo debió haberlo creado.

Ejemplos de mensajes que verás en stderr si algo falla:

```
Seed demo: no existe el aeropuerto BOG. Verifica EnsureMasterDataAsync.
Seed demo: no existe la aerolínea AV (Avianca). Verifica EnsureMasterDataAsync.
Seed demo: no existe el AircraftType '737-800'. Verifica EnsureMasterDataAsync.
Seed demo: el AircraftType '737-800' tiene solo N entradas en seat_map (se requieren al menos 5).
```

La filosofía: **antes que la demo se siembre a medias, mejor que el bootstrap muera con un mensaje descriptivo**.

### Cómo "resetear" la demo (volver a sembrar)

La idempotencia ahora se basa en `users.username = 'admin_demo'`. Para volver a generar los datos basta con:

```sql
DELETE FROM boarding_pass        WHERE check_in_id IN (SELECT check_in_id FROM check_in WHERE ticket_id IN (SELECT ticket_id FROM ticket WHERE ticket_code = 'TKT-DEMO-001'));
DELETE FROM check_in             WHERE ticket_id IN (SELECT ticket_id FROM ticket WHERE ticket_code = 'TKT-DEMO-001');
DELETE FROM ticket               WHERE ticket_code = 'TKT-DEMO-001';
DELETE FROM reservation_detail   WHERE reservation_id IN (SELECT id FROM reservation WHERE reservation_code = 'RES-DEMO-001');
DELETE FROM reservation          WHERE reservation_code = 'RES-DEMO-001';
DELETE FROM users                WHERE username = 'admin_demo';
DELETE FROM passenger            WHERE person_id = (SELECT id FROM person WHERE document_number = '123456');
DELETE FROM customer             WHERE person_id = (SELECT id FROM person WHERE document_number = '123456');
DELETE FROM person               WHERE document_number = '123456';
```

> En la práctica, durante la sustentación, basta con bajar la app y subirla otra vez si la demo ya se consumió: la fila `TKT-DEMO-001` ya tendrá estado `CHECKED_IN` y no será elegible. Al rearrancar, el seed detecta que `admin_demo` existe y no hace nada — para una segunda corrida hay que limpiar con el script de arriba.

### Cómo verificar el seed funcionó

```sql
-- ¿Existe el usuario admin_demo y está activo?
SELECT u.username, u.is_active, r.name AS rol
FROM   users u
JOIN   role r ON r.role_id = u.role_id
WHERE  u.username = 'admin_demo';

-- ¿El tiquete está PAID y vinculado al customer de admin_demo?
SELECT t.ticket_code, ts.name AS status, r.reservation_code, c.email
FROM   ticket t
JOIN   ticket_status ts ON ts.id = t.ticket_status_id
JOIN   reservation_detail rd ON rd.id = t.reservation_detail_id
JOIN   reservation r ON r.id = rd.reservation_id
JOIN   customer c ON c.id = r.customer_id
JOIN   person p ON p.id = c.person_id
JOIN   users u ON u.person_id = p.id
WHERE  t.ticket_code = 'TKT-DEMO-001' AND u.username = 'admin_demo';

-- ¿Hay al menos 5 asientos disponibles en el vuelo demo?
SELECT COUNT(*) AS asientos_libres
FROM   flight_seat fs
JOIN   scheduled_flight sf ON sf.id = fs.scheduled_flight_id
JOIN   base_flight bf ON bf.id = sf.base_flight_id
JOIN   seat_status ss ON ss.id = fs.seat_status_id
WHERE  bf.flight_code = 'DM-9001' AND ss.name = 'AVAILABLE';
```

Las tres consultas deben devolver datos. Si alguna falla, mira el stderr del arranque: el seeder lanza `InvalidOperationException` con un mensaje específico apuntando al catálogo faltante.
