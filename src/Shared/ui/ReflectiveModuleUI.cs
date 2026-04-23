using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public abstract class ReflectiveModuleUI<TService> : IModuleUI where TService : class
{
    public string Key { get; }
    public string Title { get; }

    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory _scopeFactory;

    protected ReflectiveModuleUI(string key, string title, TService service, IServiceProvider serviceProvider)
    {
        Key = key;
        Title = GetFriendlyTitle(key, title);
        _serviceProvider = serviceProvider;
        _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    private IModuleUI? ResolveModuleUI(string relatedServiceName)
    {
        using var scope = _scopeFactory.CreateScope();
        var modules = scope.ServiceProvider.GetServices<IModuleUI>();

        return modules.FirstOrDefault(m =>
            string.Equals(m.Key, relatedServiceName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(m.Title, relatedServiceName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                m.Title.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase),
                relatedServiceName.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase),
                StringComparison.OrdinalIgnoreCase));
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        AnsiConsole.Clear();
        while (!cancellationToken.IsCancellationRequested)
        {
            var actions = BuildMenuActions();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{Title}[/]")
                    .PageSize(15)
                    .AddChoices(actions.Select(a => a.Label).Append("Volver")));

            if (choice == "Volver")
                return;

            var selected = actions.First(a => a.Label == choice);
            try
            {
                await selected.Handler(cancellationToken);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.InnerException.Message)}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            }

            AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
            Console.ReadKey(intercept: true);
            AnsiConsole.Clear();
        }
    }

    private List<MenuAction> BuildMenuActions()
    {
        var methods = typeof(TService).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.EndsWith("Async", StringComparison.Ordinal))
            .ToList();

        var actions = new List<MenuAction>
        {
            new("Listar", ListAllAsync),
            new("Ver detalle", ViewDetailsAsync)
        };

        foreach (var method in methods.Where(IsActionMethod).OrderBy(m => GetActionOrder(m.Name)).ThenBy(m => m.Name))
        {
            actions.Add(new(HumanizeMethod(method.Name), ct => ExecuteActionAsync(method, ct)));
        }

        if (methods.Any(m => m.Name == "DeleteAsync"))
            actions.Add(new("Eliminar", DeleteAsync));

        return actions;
    }

    private static bool IsActionMethod(MethodInfo method)
    {
        if (method.Name is "GetAllAsync" or "GetByIdAsync" or "DeleteAsync")
            return false;
        if (method.Name.StartsWith("GetBy", StringComparison.Ordinal))
            return false;
        return true;
    }

    private static int GetActionOrder(string name)
    {
        if (name == "CreateAsync") return 0;
        if (name.StartsWith("Update", StringComparison.Ordinal) || name.StartsWith("Change", StringComparison.Ordinal)) return 1;
        return 2;
    }

    private async Task ListAllAsync(CancellationToken ct)
    {
        var rows = await GetAllItemsAsync(ct);
        if (rows.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay registros.[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded).Expand();
        var props = GetDisplayProperties(rows[0].GetType());
        foreach (var prop in props)
            table.AddColumn(prop.Name);

        foreach (var row in rows)
            table.AddRow(props.Select(p => Markup.Escape(FormatValue(p.GetValue(row)))).ToArray());

        AnsiConsole.Write(table);
    }

    private async Task ViewDetailsAsync(CancellationToken ct)
    {
        var item = await SelectExistingItemAsync(ct, allowNone: true);
        if (item is null)
            return;

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();

        foreach (var prop in GetDisplayProperties(item.GetType()))
            grid.AddRow($"[aqua]{prop.Name}[/]", Markup.Escape(FormatValue(prop.GetValue(item))));

        AnsiConsole.Write(new Panel(grid).Header($"[green]{Title}[/]"));
    }

    private async Task ExecuteActionAsync(MethodInfo method, CancellationToken ct)
    {
        object? selectedItem = null;
        var parameters = method.GetParameters().Where(p => p.ParameterType != typeof(CancellationToken)).ToList();

        if (parameters.Count > 0 && IsIdentityParameter(parameters[0]))
        {
            selectedItem = await SelectExistingItemAsync(ct, allowNone: true);
            if (selectedItem is null)
                return;
        }

        var args = new List<object?>();
        foreach (var parameter in parameters)
        {
            if (selectedItem is not null && args.Count == 0 && IsIdentityParameter(parameter))
            {
                args.Add(GetIdentityValue(selectedItem));
                continue;
            }

            var defaultValue = selectedItem is null ? null : FindDefaultValue(selectedItem, parameter.Name!, parameter.ParameterType);
            args.Add(await PromptForParameterAsync(parameter, defaultValue, ct));
        }

        if (method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)))
            args.Add(ct);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        var result = await InvokeAsync(method, args.ToArray(), service);

        AnsiConsole.MarkupLine("[green]Operación completada.[/]");
        if (result is not null)
            RenderSingleObject(result);
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        var method = typeof(TService).GetMethod("DeleteAsync");
        if (method is null)
        {
            AnsiConsole.MarkupLine("[yellow]Este módulo no soporta eliminación.[/]");
            return;
        }

        var item = await SelectExistingItemAsync(ct, allowNone: true);
        if (item is null)
            return;

        if (!AnsiConsole.Confirm($"¿Eliminar [red]{Markup.Escape(GetChoiceLabel(item))}[/]?"))
            return;

        var args = new List<object?> { GetIdentityValue(item) };
        if (method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)))
            args.Add(ct);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        await InvokeAsync(method, args.ToArray(), service);

        AnsiConsole.MarkupLine("[green]Registro eliminado.[/]");
    }

    private async Task<IReadOnlyList<object>> GetAllItemsAsync(CancellationToken ct)
    {
        var method = typeof(TService).GetMethod("GetAllAsync");
        if (method is null)
            return Array.Empty<object>();

        await using var scope = _scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TService>();
        var args = method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken))
            ? new object?[] { ct }
            : Array.Empty<object?>();

        var result = await InvokeAsync(method, args, service);
        if (result is IEnumerable enumerable)
            return enumerable.Cast<object>().ToList();

        return Array.Empty<object>();
    }

    private async Task<object?> SelectExistingItemAsync(CancellationToken ct, bool allowNone)
    {
        var items = await GetAllItemsAsync(ct);
        if (items.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay registros disponibles.[/]");
            return null;
        }

        var choices = items.Select(i => new ChoiceItem(GetChoiceLabel(i), i)).ToList();
        if (allowNone)
            choices.Insert(0, new ChoiceItem("Cancelar", null));

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<ChoiceItem>()
                .Title("Seleccione un registro")
                .UseConverter(c => c.Label)
                .PageSize(15)
                .AddChoices(choices));

        return selected.Value;
    }

    private async Task<object?> PromptForParameterAsync(ParameterInfo parameter, object? defaultValue, CancellationToken ct)
    {
        var originalType = parameter.ParameterType;
        var type = Nullable.GetUnderlyingType(originalType) ?? originalType;

        if (!IsIdentityParameter(parameter) &&
            parameter.Name?.EndsWith("Id", StringComparison.OrdinalIgnoreCase) == true &&
            type == typeof(int))
        {
            var related = await TryPromptRelatedSelectionAsync(parameter.Name, Nullable.GetUnderlyingType(parameter.ParameterType) is not null, ct);
            if (!related.Resolved)
                throw new InvalidOperationException(
                    $"No se puede continuar porque la relación '{HumanizeParameter(parameter.Name!)}' no pudo resolverse por lista. Verifica el servicio relacionado, el método GetAllAsync y que existan datos cargados.");

            return related.Value;
        }

        if (type == typeof(string))
        {
            var promptTitle = $"{HumanizeParameter(parameter.Name!)}{RenderDefault(defaultValue)}";
            var raw = IsSensitiveParameter(parameter.Name)
                ? AnsiConsole.Prompt(new TextPrompt<string>(promptTitle).Secret())
                : AnsiConsole.Ask<string>(promptTitle);

            return string.IsNullOrWhiteSpace(raw) && defaultValue is string s
                ? s
                : raw?.Trim() ?? string.Empty;
        }

        if (type == typeof(int))
            return PromptInt(HumanizeParameter(parameter.Name!), ToNullable<int>(defaultValue), parameter.ParameterType != typeof(int));

        if (type == typeof(byte))
            return PromptByte(HumanizeParameter(parameter.Name!), ToNullable<byte>(defaultValue), parameter.ParameterType != typeof(byte));

        if (type == typeof(decimal))
            return PromptDecimal(HumanizeParameter(parameter.Name!), ToNullable<decimal>(defaultValue), parameter.ParameterType != typeof(decimal));

        if (type == typeof(bool))
            return AnsiConsole.Confirm(HumanizeParameter(parameter.Name!), defaultValue is bool b && b);

        if (type == typeof(DateTime))
            return PromptDateTime(HumanizeParameter(parameter.Name!), ToNullable<DateTime>(defaultValue));

        if (type == typeof(DateOnly))
            return PromptDateOnly(HumanizeParameter(parameter.Name!), ToNullableDateOnly(defaultValue), Nullable.GetUnderlyingType(originalType) is not null);

        if (type == typeof(TimeOnly))
            return PromptTimeOnly(HumanizeParameter(parameter.Name!), ToNullableTimeOnly(defaultValue), Nullable.GetUnderlyingType(originalType) is not null);

        if (type.IsClass && type != typeof(string))
            return await BuildComplexObjectAsync(type, defaultValue, ct);

        var text = AnsiConsole.Ask<string>($"{HumanizeParameter(parameter.Name!)}{RenderDefault(defaultValue)}").Trim();
        return string.IsNullOrWhiteSpace(text)
            ? defaultValue
            : Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
    }

    private async Task<object> BuildComplexObjectAsync(Type type, object? defaults, CancellationToken ct)
    {
        var ctor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
        var values = new List<object?>();

        foreach (var parameter in ctor.GetParameters())
        {
            var defaultValue = defaults is null ? null : FindDefaultValue(defaults, parameter.Name!, parameter.ParameterType);
            values.Add(await PromptForParameterAsync(parameter, defaultValue, ct));
        }

        return ctor.Invoke(values.ToArray());
    }

    private async Task<(bool Resolved, int? Value)> TryPromptRelatedSelectionAsync(string parameterName, bool nullable, CancellationToken ct)
    {
        var relatedServiceName = ResolveRelatedServiceName(parameterName);
        if (string.IsNullOrWhiteSpace(relatedServiceName))
            return (false, null);

        var assembly = typeof(TService).Assembly;
        var serviceType = assembly.GetTypes()
            .FirstOrDefault(t => t.IsInterface && t.Name.Equals($"I{relatedServiceName}Service", StringComparison.OrdinalIgnoreCase));

        if (serviceType is null)
            return (false, null);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetService(serviceType);
        if (service is null)
            return (false, null);

        var getAll = serviceType.GetMethod("GetAllAsync");
        if (getAll is null)
            return (false, null);

        var args = getAll.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken))
            ? new object?[] { ct }
            : Array.Empty<object?>();

        var result = await InvokeAsync(getAll, args, service);
        if (result is not IEnumerable enumerable)
            return (false, null);

        var items = enumerable.Cast<object>().ToList();

        if (items.Count == 0)
        {
            var friendlyName = HumanizeParameter(parameterName);
            var moduleTitle = GetFriendlyTitle(relatedServiceName, relatedServiceName);

            AnsiConsole.MarkupLine($"[yellow]No hay datos para {Markup.Escape(friendlyName)}.[/]");
            var createNow = AnsiConsole.Confirm($"¿Deseas crear un registro en [green]{Markup.Escape(moduleTitle)}[/] ahora?");

            if (!createNow)
                return (false, null);

            var module = ResolveModuleUI(relatedServiceName);
            if (module is null)
            {
                AnsiConsole.MarkupLine($"[red]No se encontró el módulo UI para {Markup.Escape(moduleTitle)}.[/]");
                return (false, null);
            }

            await module.RunAsync(ct);

            await using var retryScope = _scopeFactory.CreateAsyncScope();
            var retryService = retryScope.ServiceProvider.GetService(serviceType);
            if (retryService is null)
                return (false, null);

            var retryResult = await InvokeAsync(getAll, args, retryService);
            if (retryResult is not IEnumerable retryEnumerable)
                return (false, null);

            items = retryEnumerable.Cast<object>().ToList();

            if (items.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Aún no hay datos para {Markup.Escape(friendlyName)}.[/]");
                return (false, null);
            }
        }

        var choices = new List<ChoiceItem>();
        if (nullable)
            choices.Add(new ChoiceItem("Ninguno", null));

        choices.AddRange(items.Select(i => new ChoiceItem(GetChoiceLabel(i), GetIdentityValue(i))));

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<ChoiceItem>()
                .Title($"Seleccione {HumanizeParameter(parameterName)}")
                .UseConverter(c => c.Label)
                .PageSize(15)
                .AddChoices(choices));

        return (true, selected.Value is null ? null : Convert.ToInt32(selected.Value, CultureInfo.InvariantCulture));
    }

    private static string ResolveRelatedServiceName(string parameterName)
    {
        return GetRelationEntityName(parameterName);
    }

    private async Task<object?> InvokeAsync(MethodInfo method, object?[] args, object? target)
    {
        var invocation = method.Invoke(target, args);
        if (invocation is Task task)
        {
            await task;
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task);
        }

        return invocation;
    }

    private static PropertyInfo[] GetDisplayProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name is not "CreatedAt" and not "UpdatedAt" and not "CancelledAt" and not "ConfirmedAt")
            .ToArray();

    private static bool IsSensitiveParameter(string? parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return false;

        return parameterName.Contains("password", StringComparison.OrdinalIgnoreCase) ||
               parameterName.Contains("secret", StringComparison.OrdinalIgnoreCase) ||
               parameterName.Contains("token", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsIdentityParameter(ParameterInfo parameter) =>
        string.Equals(parameter.Name, "id", StringComparison.OrdinalIgnoreCase) &&
        (Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType) == typeof(int);

    private static int GetIdentityValue(object item)
    {
        var prop = item.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) || p.Name.EndsWith("Id", StringComparison.Ordinal));

        if (prop is null)
            throw new InvalidOperationException($"No se encontró una propiedad ID en {item.GetType().Name}.");

        return Convert.ToInt32(prop.GetValue(item), CultureInfo.InvariantCulture);
    }

    private static object? FindDefaultValue(object source, string parameterName, Type targetType)
    {
        if (source is DefaultValueBag bag)
            return bag.Get(parameterName);

        var prop = source.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));

        if (prop is not null)
            return prop.GetValue(source);

        if (!targetType.IsClass || targetType == typeof(string))
            return null;

        var targetProps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (targetProps.Length == 0)
            return null;

        var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var targetProp in targetProps)
        {
            var sourceProp = source.GetType().GetProperties()
                .FirstOrDefault(p => p.Name.Equals(targetProp.Name, StringComparison.OrdinalIgnoreCase));

            values[targetProp.Name] = sourceProp?.GetValue(source);
        }

        return new DefaultValueBag(values);
    }

    private static string GetChoiceLabel(object item)
    {
        var props = item.GetType().GetProperties();
        var idProp = props.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) || p.Name.EndsWith("Id", StringComparison.Ordinal));
        var nameProps = props.Where(p => p.Name is "Name" or "Model" or "IataCode" or "FlightCode" or "Code" or "DocumentNumber" or "Email" or "Username")
            .Take(2)
            .ToList();

        var parts = new List<string>();
        if (idProp is not null) parts.Add($"{idProp.GetValue(item)}");
        foreach (var prop in nameProps) parts.Add(FormatValue(prop.GetValue(item)));

        return string.Join(" | ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    private static string GetFriendlyTitle(string key, string fallback) =>
        key.ToLowerInvariant() switch
        {
            "airline" => "Aerolíneas",
            "country" => "Países",
            "city" => "Ciudades y destinos",
            "airport" => "Aeropuertos",
            "terminal" => "Terminales",
            "gate" => "Puertas de embarque",
            "route" => "Rutas",
            "baseflight" => "Vuelos base",
            "routeschedule" => "Horarios de ruta",
            "scheduledflight" => "Vuelos programados",
            "flightstatus" => "Estados de vuelo",
            "customer" => "Clientes",
            "person" => "Personas",
            "passenger" => "Pasajeros",
            "reservation" => "Reservas",
            "reservationdetail" => "Detalles de reserva",
            "reservationstatushistory" => "Trazabilidad de reservas",
            "ticket" => "Tiquetes",
            "ticketstatushistory" => "Trazabilidad de tiquetes",
            "boardingpass" => "Boarding passes",
            "checkin" => "Check-in",
            "payment" => "Pagos",
            "refund" => "Reembolsos",
            _ => fallback
        };

    private static string HumanizeMethod(string methodName)
    {
        var name = methodName.Replace("Async", string.Empty, StringComparison.Ordinal);
        return name switch
        {
            "Create" => "Crear",
            "Update" => "Actualizar",
            "ChangeStatus" => "Cambiar estado",
            "UpdateStatus" => "Actualizar estado",
            "UpdatePrice" => "Actualizar precio",
            "UpdateNotes" => "Actualizar notas",
            "AdjustDelay" => "Ajustar retraso",
            "AdjustAmount" => "Ajustar monto",
            "UpdateQuantityAndFee" => "Actualizar cantidad y tarifa",
            "Assign" => "Asignar",
            "Remove" => "Remover",
            "Record" => "Registrar",
            "Confirm" => "Confirmar",
            "Cancel" => "Cancelar",
            "AddMiles" => "Agregar millas",
            "RedeemMiles" => "Redimir millas",
            "UpgradeTier" => "Subir nivel",
            "Earn" => "Registrar acumulación",
            "Redeem" => "Registrar redención",
            _ => SplitPascal(name)
        };
    }

    private static readonly Dictionary<string, string> FriendlyLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["gateId"] = "Puerta de embarque",
        ["originAirportId"] = "Aeropuerto de origen",
        ["destinationAirportId"] = "Aeropuerto de destino",
        ["documentTypeId"] = "Tipo de documento",
        ["personId"] = "Persona",
        ["customerId"] = "Cliente",
        ["passengerId"] = "Pasajero",
        ["reservationId"] = "Reserva",
        ["reservationDetailId"] = "Detalle de reserva",
        ["reservationStatusId"] = "Estado de reserva",
        ["statusId"] = "Estado de reserva",
        ["confirmedReservationStatusId"] = "Estado de reserva confirmado",
        ["cancelledReservationStatusId"] = "Estado de reserva cancelado",
        ["confirmedStatusId"] = "Estado de reserva confirmado",
        ["cancelledStatusId"] = "Estado de reserva cancelado",
        ["ticketId"] = "Tiquete",
        ["ticketStatusId"] = "Estado de tiquete",
        ["paymentId"] = "Pago",
        ["paymentMethodId"] = "Método de pago",
        ["paymentStatusId"] = "Estado de pago",
        ["refundId"] = "Reembolso",
        ["refundStatusId"] = "Estado de reembolso",
        ["currencyId"] = "Moneda",
        ["countryId"] = "País",
        ["cityId"] = "Ciudad",
        ["nationalityId"] = "Nacionalidad",
        ["genderId"] = "Género",
        ["airlineId"] = "Aerolínea",
        ["airportId"] = "Aeropuerto",
        ["terminalId"] = "Terminal",
        ["routeId"] = "Ruta",
        ["routeScheduleId"] = "Horario de ruta",
        ["scheduledFlightId"] = "Vuelo programado",
        ["flightStatusId"] = "Estado de vuelo",
        ["flightSeatId"] = "Asiento de vuelo",
        ["seatStatusId"] = "Estado del asiento",
        ["aircraftId"] = "Aeronave",
        ["aircraftTypeId"] = "Tipo de aeronave",
        ["manufacturerId"] = "Fabricante",
        ["employeeId"] = "Empleado",
        ["checkInStatusId"] = "Estado de check-in",
    };

    private static readonly Dictionary<string, string> RelationAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["originAirportId"] = "Airport",
        ["destinationAirportId"] = "Airport",
        ["departureGateId"] = "Gate",
        ["arrivalGateId"] = "Gate",
        ["documentTypeId"] = "DocumentType",
        ["reservationStatusId"] = "ReservationStatus",
        ["statusId"] = "ReservationStatus",
        ["confirmedReservationStatusId"] = "ReservationStatus",
        ["cancelledReservationStatusId"] = "ReservationStatus",
        ["confirmedStatusId"] = "ReservationStatus",
        ["cancelledStatusId"] = "ReservationStatus",
        ["ticketStatusId"] = "TicketStatus",
        ["paymentStatusId"] = "PaymentStatus",
        ["paymentMethodId"] = "PaymentMethod",
        ["refundStatusId"] = "RefundStatus",
        ["seatStatusId"] = "SeatStatus",
        ["flightStatusId"] = "FlightStatus",
        ["checkInStatusId"] = "CheckInStatus",
        ["manufacturerId"] = "AircraftManufacturer",
    };

    private static string HumanizeParameter(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return string.Empty;

        if (FriendlyLabels.TryGetValue(parameterName, out var exactLabel))
            return exactLabel;

        if (parameterName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
        {
            var withoutId = parameterName[..^2];
            return ToHumanReadable(withoutId);
        }

        return ToHumanReadable(parameterName);
    }

    private static string GetRelationEntityName(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            return string.Empty;

        if (RelationAliases.TryGetValue(parameterName, out var alias))
            return alias;

        if (parameterName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            return parameterName[..^2];

        return parameterName;
    }

    private static string ToHumanReadable(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var withSpaces = Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");
        withSpaces = withSpaces.Replace("_", " ").Trim();

        return withSpaces.Length == 0
            ? text
            : char.ToUpperInvariant(withSpaces[0]) + withSpaces[1..];
    }

    private static string SplitPascal(string value) =>
        string.Concat(value.Select((ch, i) =>
            i > 0 && char.IsUpper(ch) && value[i - 1] != ' '
                ? " " + ch
                : ch.ToString()));

    private static string RenderDefault(object? defaultValue) =>
        defaultValue is null ? string.Empty : $" [grey](actual: {Markup.Escape(FormatValue(defaultValue))})[/]";

    private static string FormatValue(object? value) =>
        value switch
        {
            null => string.Empty,
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly t => t.ToString("HH:mm", CultureInfo.InvariantCulture),
            bool b => b ? "Sí" : "No",
            _ => value.ToString() ?? string.Empty
        };

    private static T? ToNullable<T>(object? value) where T : struct
    {
        if (value is null) return null;
        if (value is T typed) return typed;
        return (T?)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
    }

    private static DateOnly? ToNullableDateOnly(object? value)
    {
        if (value is null) return null;
        if (value is DateOnly d) return d;
        if (value is DateTime dt) return DateOnly.FromDateTime(dt);
        if (DateOnly.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            return parsed;
        return null;
    }

    private static TimeOnly? ToNullableTimeOnly(object? value)
    {
        if (value is null) return null;
        if (value is TimeOnly t) return t;
        if (value is DateTime dt) return TimeOnly.FromDateTime(dt);
        if (TimeOnly.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            return parsed;
        return null;
    }

    private static int? PromptInt(string title, int? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"{title}{RenderDefault(defaultValue)}").Trim();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Ingrese un número entero válido.[/]");
        }
    }

    private static byte? PromptByte(string title, byte? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"{title}{RenderDefault(defaultValue)}").Trim();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (byte.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Ingrese un número entre 0 y 255.[/]");
        }
    }

    private static decimal? PromptDecimal(string title, decimal? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"{title}{RenderDefault(defaultValue)}").Trim();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Ingrese un número decimal válido.[/]");
        }
    }

    private static DateTime PromptDateTime(string title, DateTime? defaultValue)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"{title}{RenderDefault(defaultValue)}").Trim();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Ingrese una fecha válida.[/] [grey](Ej: 2026-04-22 14:30)[/]");
        }
    }

    private static object? PromptDateOnly(string title, DateOnly? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"{title}{RenderDefault(defaultValue)}").Trim();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (DateOnly.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Ingrese una fecha válida.[/] [grey](Ej: 2026-04-22)[/]");
        }
    }

    private static object? PromptTimeOnly(string title, TimeOnly? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"{title}{RenderDefault(defaultValue)}").Trim();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (TimeOnly.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Ingrese una hora válida.[/] [grey](Ej: 14:30)[/]");
        }
    }

    private static void RenderSingleObject(object result)
    {
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();

        foreach (var prop in GetDisplayProperties(result.GetType()))
            grid.AddRow($"[aqua]{prop.Name}[/]", Markup.Escape(FormatValue(prop.GetValue(result))));

        AnsiConsole.Write(new Panel(grid).Border(BoxBorder.Rounded));
    }

    private sealed record MenuAction(string Label, Func<CancellationToken, Task> Handler);
    private sealed record ChoiceItem(string Label, object? Value);

    private sealed class DefaultValueBag
    {
        private readonly Dictionary<string, object?> _values;

        public DefaultValueBag(Dictionary<string, object?> values)
        {
            _values = values;
        }

        public object? this[string name] => _values.TryGetValue(name, out var value) ? value : null;

        public object? Get(string name) => this[name];
    }
}