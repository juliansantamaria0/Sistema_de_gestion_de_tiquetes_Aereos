using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public abstract class ReflectiveModuleUI<TService> : IModuleUI where TService : class
{
    private const int VarcharInputMaxLength = 60;
    private const string BackLabel   = "« Volver";
    private const string CancelLabel = "« Cancelar";

    private static readonly object CancelActionSentinel = new();

    private static string TruncateForVarchar60(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value ?? string.Empty;
        return value.Length <= VarcharInputMaxLength ? value : value[..VarcharInputMaxLength];
    }

    public string Key   { get; }
    public string Title { get; }

    private readonly IServiceProvider    _serviceProvider;
    private readonly IServiceScopeFactory _scopeFactory;

    protected ReflectiveModuleUI(string key, string title, TService service, IServiceProvider serviceProvider)
    {
        Key   = key;
        Title = GetFriendlyTitle(key, title);
        _serviceProvider = serviceProvider;
        _scopeFactory    = serviceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    private IModuleUI? ResolveModuleUI(string relatedServiceName)
    {
        using var scope = _scopeFactory.CreateScope();
        var modules = scope.ServiceProvider.GetServices<IModuleUI>();
        var target  = ModuleKeyNormalizer.Normalize(relatedServiceName);
        return modules.FirstOrDefault(m =>
            string.Equals(ModuleKeyNormalizer.Normalize(m.Key), target, StringComparison.Ordinal) ||
            string.Equals(ModuleKeyNormalizer.Normalize(m.Title), target, StringComparison.Ordinal) ||
            string.Equals(
                ModuleKeyNormalizer.Normalize(m.Title.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)),
                target,
                StringComparison.Ordinal));
    }

    // ── Entry point ────────────────────────────────────────────────────────────
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            RenderModuleHeader(Title, "[grey]Use las flechas para navegar, Enter para seleccionar. En campos de texto escriba [bold]cancelar[/] para anular.[/]");

            var actions = BuildMenuActions();
            var choice  = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Qué desea hacer?[/]")
                    .PageSize(15)
                    .MoreChoicesText("[grey](Desplace para ver más opciones)[/]")
                    .AddChoices(actions.Select(a => a.Label).Append(BackLabel)));

            if (choice == BackLabel)
                return;

            var selected = actions.First(a => a.Label == choice);

            AnsiConsole.Clear();
            RenderModuleHeader($"{Title}  ›  {choice}", null);

            await selected.Handler(cancellationToken);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar...[/]");
            Console.ReadKey(intercept: true);
        }
    }

    // ── Menu construction ──────────────────────────────────────────────────────
    private List<MenuAction> BuildMenuActions()
    {
        var methods = typeof(TService).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name.EndsWith("Async", StringComparison.Ordinal))
            .ToList();

        var actions = new List<MenuAction>
        {
            new("Ver todos los registros", ListAllAsync),
            new("Ver detalle de un registro", ViewDetailsAsync)
        };

        foreach (var method in methods.Where(IsActionMethod).OrderBy(m => GetActionOrder(m.Name)).ThenBy(m => m.Name))
            actions.Add(new(HumanizeMethod(method.Name), ct => ExecuteActionAsync(method, ct)));

        if (methods.Any(m => m.Name == "DeleteAsync"))
            actions.Add(new("Eliminar registro", DeleteAsync));

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

    // ── List / Detail ──────────────────────────────────────────────────────────
    private async Task ListAllAsync(CancellationToken ct)
    {
        var rows = await GetAllItemsAsync(ct);
        if (rows.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay registros para mostrar.[/]");
            AnsiConsole.MarkupLine("[grey]Use la opción [bold]Crear[/] para agregar el primer registro.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[grey]{rows.Count} registro(s) encontrado(s).[/]");
        AnsiConsole.WriteLine();

        var table = new Table().Border(TableBorder.Rounded).Expand();
        var props = GetDisplayProperties(rows[0].GetType());
        foreach (var prop in props)
            table.AddColumn(new TableColumn($"[bold]{Markup.Escape(GetColumnLabel(prop.Name))}[/]"));

        foreach (var row in rows)
            table.AddRow(props.Select(p => Markup.Escape(FormatValue(p.GetValue(row)))).ToArray());

        AnsiConsole.Write(table);
    }

    private async Task ViewDetailsAsync(CancellationToken ct)
    {
        var item = await SelectExistingItemAsync("Ver detalle de", ct, allowNone: true);
        if (item is null)
        {
            AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]");
            return;
        }

        var grid = new Grid().AddColumn().AddColumn();
        foreach (var prop in GetDisplayProperties(item.GetType()))
            grid.AddRow(
                $"[aqua]{Markup.Escape(GetColumnLabel(prop.Name))}[/]",
                Markup.Escape(FormatValue(prop.GetValue(item))));

        AnsiConsole.Write(new Panel(grid)
            .Header($"[green]  {Markup.Escape(Title)} — Detalle  [/]")
            .Border(BoxBorder.Rounded));
    }

    // ── Actions ────────────────────────────────────────────────────────────────
    private async Task ExecuteActionAsync(MethodInfo method, CancellationToken ct)
    {
        try
        {
            object? selectedItem = null;
            var parameters = method.GetParameters().Where(p => p.ParameterType != typeof(CancellationToken)).ToList();

            if (parameters.Count > 0 && IsIdentityParameter(parameters[0]))
            {
                selectedItem = await SelectExistingItemAsync(HumanizeMethod(method.Name), ct, allowNone: true);
                if (selectedItem is null)
                {
                    AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]");
                    return;
                }
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
            var result  = await InvokeAsync(method, args.ToArray(), service);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]  Operación realizada con éxito.[/]");
            if (result is not null)
            {
                AnsiConsole.WriteLine();
                RenderSingleObject(result);
            }
        }
        catch (FlowAbortException)
        {
            AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]");
        }
        catch (DbUpdateConcurrencyException)
        {
            AnsiConsole.MarkupLine("[red]El registro fue modificado por otro proceso. Intente de nuevo.[/]");
        }
        catch (DbUpdateException ex)
        {
            ConsoleErrorHandler.WriteFriendlyDbUpdateException(ex);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(ex.Message)}[/]");
        }
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        var method = typeof(TService).GetMethod("DeleteAsync");
        if (method is null)
        {
            AnsiConsole.MarkupLine("[yellow]Este módulo no soporta eliminación.[/]");
            return;
        }

        var item = await SelectExistingItemAsync("Eliminar", ct, allowNone: true);
        if (item is null)
        {
            AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]");
            return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[yellow]Registro seleccionado:[/] [bold]{Markup.Escape(GetChoiceLabel(item))}[/]");
        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm("[red]¿Confirma que desea eliminar este registro? Esta acción no se puede deshacer.[/]"))
        {
            AnsiConsole.MarkupLine("[grey]Eliminación cancelada.[/]");
            return;
        }

        var args = new List<object?> { GetIdentityValue(item) };
        if (method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken)))
            args.Add(ct);

        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<TService>();
            await InvokeAsync(method, args.ToArray(), service);
            AnsiConsole.MarkupLine("[green]  Registro eliminado correctamente.[/]");
        }
        catch (DbUpdateConcurrencyException)
        {
            AnsiConsole.MarkupLine("[red]El registro fue modificado por otro proceso. Intente de nuevo.[/]");
        }
        catch (DbUpdateException ex)
        {
            ConsoleErrorHandler.WriteFriendlyDbUpdateException(ex);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(ex.Message)}[/]");
        }
    }

    // ── Data access ────────────────────────────────────────────────────────────
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

    private async Task<object?> SelectExistingItemAsync(string actionLabel, CancellationToken ct, bool allowNone)
    {
        var items = await GetAllItemsAsync(ct);
        if (items.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No hay registros de {Markup.Escape(Title)} disponibles.[/]");
            AnsiConsole.MarkupLine("[grey]Cree al menos un registro antes de realizar esta acción.[/]");
            return null;
        }

        var choices = items.Select(i => new ChoiceItem(GetChoiceLabel(i), i)).ToList();
        if (allowNone)
            choices.Insert(0, new ChoiceItem(CancelLabel, null));

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<ChoiceItem>()
                .Title($"[yellow]{Markup.Escape(actionLabel)} — seleccione un registro:[/]")
                .UseConverter(c => c.Label)
                .PageSize(15)
                .MoreChoicesText("[grey](Desplace para ver más)[/]")
                .AddChoices(choices));

        return selected.Value;
    }

    // ── Parameter prompting ────────────────────────────────────────────────────
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
                    $"No se puede continuar: la relación '{HumanizeParameter(parameter.Name!)}' no pudo resolverse. Verifique que existan datos en el módulo relacionado.");
            return related.Value;
        }

        if (type == typeof(string))
        {
            var promptTitle = $"[yellow]{HumanizeParameter(parameter.Name!)}:[/]{RenderDefault(defaultValue)} [grey](o escriba cancelar)[/]";
            var raw = IsSensitiveParameter(parameter.Name)
                ? AnsiConsole.Prompt(new TextPrompt<string>(promptTitle).Secret())
                : AnsiConsole.Ask<string>(promptTitle);

            var trimmed = raw?.Trim() ?? string.Empty;
            if (!IsSensitiveParameter(parameter.Name) && IsUserCancelInput(trimmed))
                throw new FlowAbortException();

            var resolved = string.IsNullOrWhiteSpace(trimmed) && defaultValue is string s
                ? TruncateForVarchar60(s)
                : TruncateForVarchar60(trimmed);
            return resolved;
        }

        if (type == typeof(int))
            return PromptInt(HumanizeParameter(parameter.Name!), ToNullable<int>(defaultValue), parameter.ParameterType != typeof(int));

        if (type == typeof(byte))
            return PromptByte(HumanizeParameter(parameter.Name!), ToNullable<byte>(defaultValue), parameter.ParameterType != typeof(byte));

        if (type == typeof(decimal))
            return PromptDecimal(HumanizeParameter(parameter.Name!), ToNullable<decimal>(defaultValue), parameter.ParameterType != typeof(decimal));

        if (type == typeof(bool))
            return AnsiConsole.Confirm($"[yellow]{HumanizeParameter(parameter.Name!)}:[/]", defaultValue is bool b && b);

        if (type == typeof(DateTime))
            return PromptDateTime(HumanizeParameter(parameter.Name!), ToNullable<DateTime>(defaultValue));

        if (type == typeof(DateOnly))
            return PromptDateOnly(HumanizeParameter(parameter.Name!), ToNullableDateOnly(defaultValue), Nullable.GetUnderlyingType(originalType) is not null);

        if (type == typeof(TimeOnly))
            return PromptTimeOnly(HumanizeParameter(parameter.Name!), ToNullableTimeOnly(defaultValue), Nullable.GetUnderlyingType(originalType) is not null);

        if (type.IsClass && type != typeof(string))
            return await BuildComplexObjectAsync(type, defaultValue, ct);

        var text = TruncateForVarchar60(
            AnsiConsole.Ask<string>($"[yellow]{HumanizeParameter(parameter.Name!)}:[/]{RenderDefault(defaultValue)} [grey](o escriba cancelar)[/]").Trim());
        if (IsUserCancelInput(text))
            throw new FlowAbortException();

        return string.IsNullOrWhiteSpace(text)
            ? defaultValue
            : Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
    }

    private async Task<object> BuildComplexObjectAsync(Type type, object? defaults, CancellationToken ct)
    {
        var ctor   = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
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

        var assembly    = typeof(TService).Assembly;
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

        var args   = getAll.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken))
            ? new object?[] { ct }
            : Array.Empty<object?>();

        var result = await InvokeAsync(getAll, args, service);
        if (result is not IEnumerable enumerable)
            return (false, null);

        var items = enumerable.Cast<object>().ToList();

        if (items.Count == 0)
        {
            var friendlyName = HumanizeParameter(parameterName);
            var moduleTitle  = GetFriendlyTitle(relatedServiceName, relatedServiceName);

            AnsiConsole.MarkupLine($"[yellow]No hay datos disponibles para {Markup.Escape(friendlyName)}.[/]");
            var createNow = AnsiConsole.Confirm($"¿Desea crear un registro en [green]{Markup.Escape(moduleTitle)}[/] ahora?");

            if (!createNow)
                throw new FlowAbortException();

            var module = ResolveModuleUI(relatedServiceName);
            if (module is null)
            {
                AnsiConsole.MarkupLine($"[red]No se encontró el módulo para {Markup.Escape(moduleTitle)}.[/]");
                throw new FlowAbortException();
            }

            await module.RunAsync(ct);

            await using var retryScope   = _scopeFactory.CreateAsyncScope();
            var retryService = retryScope.ServiceProvider.GetService(serviceType);
            if (retryService is null)
                return (false, null);

            var retryResult = await InvokeAsync(getAll, args, retryService);
            if (retryResult is not IEnumerable retryEnumerable)
                return (false, null);

            items = retryEnumerable.Cast<object>().ToList();
            if (items.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Aún no hay registros para {Markup.Escape(friendlyName)}. Operación cancelada.[/]");
                throw new FlowAbortException();
            }
        }

        var choices = new List<ChoiceItem> { new(CancelLabel, CancelActionSentinel) };
        if (nullable)
            choices.Add(new ChoiceItem("(Ninguno / dejar vacío)", null));

        choices.AddRange(items.Select(i => new ChoiceItem(GetChoiceLabel(i), GetIdentityValue(i))));

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<ChoiceItem>()
                .Title($"[yellow]Seleccione {Markup.Escape(HumanizeParameter(parameterName))}:[/]")
                .UseConverter(c => c.Label)
                .PageSize(15)
                .MoreChoicesText("[grey](Desplace para ver más)[/]")
                .AddChoices(choices));

        if (ReferenceEquals(selected.Value, CancelActionSentinel))
            throw new FlowAbortException();

        return (true, selected.Value is null ? null : Convert.ToInt32(selected.Value, CultureInfo.InvariantCulture));
    }

    private static string ResolveRelatedServiceName(string parameterName)
        => GetRelationEntityName(parameterName);

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

    // ── Display helpers ────────────────────────────────────────────────────────
    private static PropertyInfo[] GetDisplayProperties(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name is not "CreatedAt" and not "UpdatedAt" and not "CancelledAt" and not "ConfirmedAt")
            .ToArray();

    private static void RenderModuleHeader(string title, string? hint)
    {
        AnsiConsole.Write(new Rule($"[green] {Markup.Escape(title)} [/]").RuleStyle(Style.Parse("grey")));
        if (!string.IsNullOrEmpty(hint))
            AnsiConsole.MarkupLine(hint);
        AnsiConsole.WriteLine();
    }

    private static void RenderSingleObject(object result)
    {
        var grid = new Grid().AddColumn().AddColumn();
        foreach (var prop in GetDisplayProperties(result.GetType()))
            grid.AddRow(
                $"[aqua]{Markup.Escape(GetColumnLabel(prop.Name))}[/]",
                Markup.Escape(FormatValue(prop.GetValue(result))));

        AnsiConsole.Write(new Panel(grid).Border(BoxBorder.Rounded));
    }

    private static string GetColumnLabel(string propName)
    {
        if (PropLabels.TryGetValue(propName, out var label))
            return label;
        return ToHumanReadable(propName);
    }

    private static readonly Dictionary<string, string> PropLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Id"]                    = "ID",
        ["Name"]                  = "Nombre",
        ["Code"]                  = "Código",
        ["FlightCode"]            = "Cód. vuelo",
        ["ReservationCode"]       = "Cód. reserva",
        ["IataCode"]              = "IATA",
        ["Description"]           = "Descripción",
        ["Email"]                 = "Correo",
        ["Phone"]                 = "Teléfono",
        ["Username"]              = "Usuario",
        ["FirstName"]             = "Nombre",
        ["LastName"]              = "Apellido",
        ["DocumentNumber"]        = "Nº documento",
        ["DepartureDate"]         = "Fecha salida",
        ["DepartureTime"]         = "Hora salida",
        ["ArrivalDate"]           = "Fecha llegada",
        ["ArrivalTime"]           = "Hora llegada",
        ["EstimatedArrivalDatetime"] = "Llegada estimada",
        ["ReservationDate"]       = "Fecha reserva",
        ["Amount"]                = "Monto",
        ["Price"]                 = "Precio",
        ["TotalSeats"]            = "Total asientos",
        ["AvailableSeats"]        = "Asientos libres",
        ["Model"]                 = "Modelo",
        ["Capacity"]              = "Capacidad",
        ["Origin"]                = "Origen",
        ["Destination"]           = "Destino",
        ["CustomerId"]            = "ID Cliente",
        ["ScheduledFlightId"]     = "ID Vuelo",
        ["ReservationStatusId"]   = "Estado",
        ["ReservationId"]         = "ID Reserva",
        ["TicketStatusId"]        = "Estado",
        ["PaymentStatusId"]       = "Estado pago",
        ["SeatNumber"]            = "Asiento",
        ["SeatClass"]             = "Clase",
        ["RouteId"]               = "Ruta",
        ["BaseFlightId"]          = "Vuelo base",
        ["AircraftId"]            = "Aeronave",
        ["TotalAmount"]           = "Total",
        ["Discount"]              = "Descuento",
    };

    private static string GetChoiceLabel(object item)
    {
        var props    = item.GetType().GetProperties();
        var idProp   = props.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) || p.Name.EndsWith("Id", StringComparison.Ordinal));
        var nameProps = props.Where(p => p.Name is "Name" or "Model" or "IataCode" or "FlightCode" or "Code" or "DocumentNumber" or "Email" or "Username" or "ReservationCode")
            .Take(2).ToList();

        var parts = new List<string>();
        if (idProp is not null)  parts.Add($"#{idProp.GetValue(item)}");
        foreach (var prop in nameProps) parts.Add(FormatValue(prop.GetValue(item)));

        return string.Join("  ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    private static bool IsSensitiveParameter(string? parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName)) return false;
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
        if (targetProps.Length == 0) return null;

        var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var targetProp in targetProps)
        {
            var sourceProp = source.GetType().GetProperties()
                .FirstOrDefault(p => p.Name.Equals(targetProp.Name, StringComparison.OrdinalIgnoreCase));
            values[targetProp.Name] = sourceProp?.GetValue(source);
        }
        return new DefaultValueBag(values);
    }

    // ── Label helpers ──────────────────────────────────────────────────────────
    private static string GetFriendlyTitle(string key, string fallback) =>
        key.ToLowerInvariant() switch
        {
            "airline"                    => "Aerolíneas",
            "country"                    => "Países",
            "city"                       => "Ciudades y destinos",
            "airport"                    => "Aeropuertos",
            "terminal"                   => "Terminales",
            "gate"                       => "Puertas de embarque",
            "route"                      => "Rutas",
            "baseflight"                 => "Vuelos base",
            "routeschedule"              => "Horarios de ruta",
            "scheduledflight"            => "Vuelos programados",
            "flightstatus"               => "Estados de vuelo",
            "customer"                   => "Mis datos de cliente",
            "person"                     => "Datos personales",
            "passenger"                  => "Pasajeros",
            "reservation"                => "Reservas",
            "reservationdetail"          => "Detalles de reserva",
            "reservationstatushistory"   => "Historial de reservas",
            "ticket"                     => "Tiquetes",
            "ticketstatushistory"        => "Historial de tiquetes",
            "boardingpass"               => "Pases de abordar",
            "checkin"                    => "Check-in",
            "payment"                    => "Pagos",
            "refund"                     => "Reembolsos",
            "faretype"                   => "Tipos de tarifa",
            _                            => fallback
        };

    private static string HumanizeMethod(string methodName)
    {
        var name = methodName.Replace("Async", string.Empty, StringComparison.Ordinal);
        return name switch
        {
            "Create"                 => "Crear nuevo registro",
            "Update"                 => "Actualizar registro",
            "ChangeStatus"           => "Cambiar estado",
            "UpdateStatus"           => "Actualizar estado",
            "UpdatePrice"            => "Actualizar precio",
            "UpdateNotes"            => "Actualizar notas",
            "AdjustDelay"            => "Ajustar retraso",
            "AdjustAmount"           => "Ajustar monto",
            "UpdateQuantityAndFee"   => "Actualizar cantidad y tarifa",
            "Assign"                 => "Asignar",
            "Remove"                 => "Remover asignación",
            "Record"                 => "Registrar",
            "Confirm"                => "Confirmar",
            "Cancel"                 => "Cancelar",
            "AddMiles"               => "Agregar millas",
            "RedeemMiles"            => "Redimir millas",
            "UpgradeTier"            => "Subir nivel de lealtad",
            "Earn"                   => "Registrar acumulación",
            "Redeem"                 => "Registrar redención",
            _                        => SplitPascal(name)
        };
    }

    private static readonly Dictionary<string, string> FriendlyLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["gateId"]                         = "Puerta de embarque",
        ["originAirportId"]                = "Aeropuerto de origen",
        ["destinationAirportId"]           = "Aeropuerto de destino",
        ["documentTypeId"]                 = "Tipo de documento",
        ["personId"]                       = "Persona",
        ["customerId"]                     = "Cliente",
        ["passengerId"]                    = "Pasajero",
        ["reservationId"]                  = "Reserva",
        ["reservationDetailId"]            = "Detalle de reserva",
        ["reservationStatusId"]            = "Estado de reserva",
        ["statusId"]                       = "Estado de reserva",
        ["confirmedReservationStatusId"]   = "Estado (confirmado)",
        ["cancelledReservationStatusId"]   = "Estado (cancelado)",
        ["confirmedStatusId"]              = "Estado (confirmado)",
        ["cancelledStatusId"]              = "Estado (cancelado)",
        ["ticketId"]                       = "Tiquete",
        ["ticketStatusId"]                 = "Estado de tiquete",
        ["paymentId"]                      = "Pago",
        ["paymentMethodId"]                = "Método de pago",
        ["paymentStatusId"]                = "Estado de pago",
        ["refundId"]                       = "Reembolso",
        ["refundStatusId"]                 = "Estado de reembolso",
        ["currencyId"]                     = "Moneda",
        ["countryId"]                      = "País",
        ["cityId"]                         = "Ciudad",
        ["nationalityId"]                  = "Nacionalidad",
        ["genderId"]                       = "Género",
        ["airlineId"]                      = "Aerolínea",
        ["airportId"]                      = "Aeropuerto",
        ["terminalId"]                     = "Terminal",
        ["routeId"]                        = "Ruta",
        ["routeScheduleId"]                = "Horario de ruta",
        ["scheduledFlightId"]              = "Vuelo programado",
        ["flightStatusId"]                 = "Estado de vuelo",
        ["flightSeatId"]                   = "Asiento de vuelo",
        ["seatStatusId"]                   = "Estado del asiento",
        ["aircraftId"]                     = "Aeronave",
        ["aircraftTypeId"]                 = "Tipo de aeronave",
        ["manufacturerId"]                 = "Fabricante",
        ["employeeId"]                     = "Empleado",
        ["checkInStatusId"]                = "Estado de check-in",
    };

    private static readonly Dictionary<string, string> RelationAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["originAirportId"]        = "Airport",
        ["destinationAirportId"]   = "Airport",
        ["departureGateId"]        = "Gate",
        ["arrivalGateId"]          = "Gate",
        ["documentTypeId"]         = "DocumentType",
        ["reservationStatusId"]    = "ReservationStatus",
        ["statusId"]               = "ReservationStatus",
        ["confirmedReservationStatusId"] = "ReservationStatus",
        ["cancelledReservationStatusId"] = "ReservationStatus",
        ["confirmedStatusId"]      = "ReservationStatus",
        ["cancelledStatusId"]      = "ReservationStatus",
        ["ticketStatusId"]         = "TicketStatus",
        ["paymentStatusId"]        = "PaymentStatus",
        ["paymentMethodId"]        = "PaymentMethod",
        ["refundStatusId"]         = "RefundStatus",
        ["seatStatusId"]           = "SeatStatus",
        ["flightStatusId"]         = "FlightStatus",
        ["checkInStatusId"]        = "CheckInStatus",
        ["manufacturerId"]         = "AircraftManufacturer",
    };

    private static string HumanizeParameter(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName)) return string.Empty;
        if (FriendlyLabels.TryGetValue(parameterName, out var exactLabel)) return exactLabel;
        if (parameterName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            return ToHumanReadable(parameterName[..^2]);
        return ToHumanReadable(parameterName);
    }

    private static string GetRelationEntityName(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName)) return string.Empty;
        if (RelationAliases.TryGetValue(parameterName, out var alias)) return alias;
        if (parameterName.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            return parameterName[..^2];
        return parameterName;
    }

    private static string ToHumanReadable(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var withSpaces = Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");
        withSpaces = withSpaces.Replace("_", " ").Trim();
        return withSpaces.Length == 0
            ? text
            : char.ToUpperInvariant(withSpaces[0]) + withSpaces[1..];
    }

    private static string SplitPascal(string value) =>
        string.Concat(value.Select((ch, i) =>
            i > 0 && char.IsUpper(ch) && value[i - 1] != ' ' ? " " + ch : ch.ToString()));

    private static string RenderDefault(object? defaultValue) =>
        defaultValue is null ? string.Empty : $" [grey](actual: {Markup.Escape(FormatValue(defaultValue))})[/]";

    private static string FormatValue(object? value) =>
        value switch
        {
            null      => string.Empty,
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            DateOnly d  => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly t  => t.ToString("HH:mm", CultureInfo.InvariantCulture),
            bool b      => b ? "Sí" : "No",
            _           => value.ToString() ?? string.Empty
        };

    // ── Type helpers ───────────────────────────────────────────────────────────
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

    private static bool IsUserCancelInput(string raw) =>
        string.Equals(raw, "cancelar", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(raw, "volver",   StringComparison.OrdinalIgnoreCase) ||
        string.Equals(raw, "salir",    StringComparison.OrdinalIgnoreCase) ||
        string.Equals(raw, "q",        StringComparison.OrdinalIgnoreCase);

    // ── Typed input prompts ────────────────────────────────────────────────────
    private static int? PromptInt(string title, int? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"[yellow]{title}:[/]{RenderDefault(defaultValue)} [grey](número entero, o cancelar)[/]").Trim();
            if (IsUserCancelInput(raw)) throw new FlowAbortException();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Por favor ingrese un número entero válido (ej: 42).[/]");
        }
    }

    private static byte? PromptByte(string title, byte? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"[yellow]{title}:[/]{RenderDefault(defaultValue)} [grey](0–255, o cancelar)[/]").Trim();
            if (IsUserCancelInput(raw)) throw new FlowAbortException();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (byte.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Por favor ingrese un número entre 0 y 255.[/]");
        }
    }

    private static decimal? PromptDecimal(string title, decimal? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"[yellow]{title}:[/]{RenderDefault(defaultValue)} [grey](número decimal, ej: 1234.50, o cancelar)[/]").Trim();
            if (IsUserCancelInput(raw)) throw new FlowAbortException();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Por favor ingrese un número decimal válido (ej: 1234.50).[/]");
        }
    }

    private static DateTime PromptDateTime(string title, DateTime? defaultValue)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"[yellow]{title}:[/]{RenderDefault(defaultValue)} [grey](ej: 2026-12-31 14:30, o cancelar)[/]").Trim();
            if (IsUserCancelInput(raw)) throw new FlowAbortException();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Formato inválido. Use el formato: 2026-12-31 14:30[/]");
        }
    }

    private static object? PromptDateOnly(string title, DateOnly? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"[yellow]{title}:[/]{RenderDefault(defaultValue)} [grey](ej: 2026-12-31, o cancelar)[/]").Trim();
            if (IsUserCancelInput(raw)) throw new FlowAbortException();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (DateOnly.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Formato inválido. Use el formato: 2026-12-31[/]");
        }
    }

    private static object? PromptTimeOnly(string title, TimeOnly? defaultValue, bool nullable)
    {
        while (true)
        {
            var raw = AnsiConsole.Ask<string>($"[yellow]{title}:[/]{RenderDefault(defaultValue)} [grey](ej: 14:30, o cancelar)[/]").Trim();
            if (IsUserCancelInput(raw)) throw new FlowAbortException();
            if (string.IsNullOrWhiteSpace(raw) && defaultValue.HasValue) return defaultValue.Value;
            if (string.IsNullOrWhiteSpace(raw) && nullable) return null;
            if (TimeOnly.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value)) return value;
            AnsiConsole.MarkupLine("[red]Formato inválido. Use el formato: 14:30[/]");
        }
    }

    // ── Inner types ────────────────────────────────────────────────────────────
    private sealed record MenuAction(string Label, Func<CancellationToken, Task> Handler);
    private sealed record ChoiceItem(string Label, object? Value);

    private sealed class DefaultValueBag
    {
        private readonly Dictionary<string, object?> _values;
        public DefaultValueBag(Dictionary<string, object?> values) => _values = values;
        public object? this[string name] => _values.TryGetValue(name, out var value) ? value : null;
        public object? Get(string name)  => this[name];
    }
}
