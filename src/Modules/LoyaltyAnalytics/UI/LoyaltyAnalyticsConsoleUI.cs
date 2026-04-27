namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.UI;

using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class LoyaltyAnalyticsConsoleUI : IModuleUI
{
    private readonly ILoyaltyAnalyticsService _svc;

    public string Key   => "loyalty_analytics";
    public string Title => " Reportes de Fidelización (Millas)";

    public LoyaltyAnalyticsConsoleUI(ILoyaltyAnalyticsService svc)
    {
        _svc = svc;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            ConsoleDashboard.ModuleScreenHeader(
                "Analítica Avanzada – Programa de Millas",
                "Reportes LINQ sobre acumulación, redención y fidelización");

            var opcion = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Seleccione el reporte:[/]")
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(
                        "1. Top clientes – millas acumuladas",
                        "2. Top clientes – millas redimidas",
                        "3. Aerolíneas – volumen de fidelización",
                        "4. Ranking de viajeros frecuentes",
                        "5. Resumen ejecutivo del programa",
                        "« Volver"));

            if (opcion.StartsWith("«")) break;

            try
            {
                if      (opcion.StartsWith("1")) await ReporteTopAcumuladasAsync(cancellationToken);
                else if (opcion.StartsWith("2")) await ReporteTopRedimidasAsync(cancellationToken);
                else if (opcion.StartsWith("3")) await ReporteAerolineasAsync(cancellationToken);
                else if (opcion.StartsWith("4")) await ReporteViajerosFrecuentesAsync(cancellationToken);
                else if (opcion.StartsWith("5")) await ReporteResumenAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                ConsoleDashboard.Error(ex.Message);
            }

            ConsoleDashboard.FooterPressKey();
            Console.ReadKey(intercept: true);
        }
    }

    private async Task ReporteTopAcumuladasAsync(CancellationToken ct)
    {
        ConsoleDashboard.SubScreenTitle("Top clientes – millas acumuladas");
        var datos = (await _svc.GetTopMilesAccumulatedAsync(10, ct)).ToList();
        if (!datos.Any()) { ConsoleDashboard.Warning("No hay cuentas registradas."); return; }

        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn(new TableColumn("[bold]#[/]").RightAligned());
        table.AddColumn("[bold]Pasajero[/]");
        table.AddColumn("[bold]Programa[/]");
        table.AddColumn("[bold]Tier[/]");
        table.AddColumn(new TableColumn("[bold]Total millas[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Disponibles[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Redimidas[/]").RightAligned());

        int pos = 1;
        foreach (var d in datos)
        {
            var medal = pos switch { 1 => "🥇", 2 => "🥈", 3 => "🥉", _ => $"{pos}." };
            table.AddRow(medal, Markup.Escape(d.PassengerName), Markup.Escape(d.ProgramName),
                Markup.Escape(d.TierName), $"[bold green]{d.TotalMiles:N0}[/]",
                $"[aqua]{d.AvailableMiles:N0}[/]", $"[yellow]{d.RedeemedMiles:N0}[/]");
            pos++;
        }
        ConsoleDashboard.ShowTablePanel("TOP MILLAS ACUMULADAS", table);
    }

    private async Task ReporteTopRedimidasAsync(CancellationToken ct)
    {
        ConsoleDashboard.SubScreenTitle("Top clientes – millas redimidas");
        var datos = (await _svc.GetTopMilesRedeemedAsync(10, ct)).ToList();
        if (!datos.Any()) { ConsoleDashboard.Warning("Ningún pasajero ha redimido millas."); return; }

        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn(new TableColumn("[bold]#[/]").RightAligned());
        table.AddColumn("[bold]Pasajero[/]");
        table.AddColumn(new TableColumn("[bold]Redimidas[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Ganadas[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]% Redención[/]").RightAligned());

        int pos = 1;
        foreach (var d in datos)
        {
            var color = d.RedemptionRate >= 50 ? "red" : "yellow";
            table.AddRow($"{pos++}.", Markup.Escape(d.PassengerName),
                $"[bold yellow]{d.TotalRedeemed:N0}[/]", $"[green]{d.TotalEarned:N0}[/]",
                $"[{color}]{d.RedemptionRate:F1}%[/]");
        }
        ConsoleDashboard.ShowTablePanel("CLIENTES QUE MÁS REDIMEN MILLAS", table);
    }

    private async Task ReporteAerolineasAsync(CancellationToken ct)
    {
        ConsoleDashboard.SubScreenTitle("Aerolíneas – volumen de fidelización");
        var datos = (await _svc.GetAirlineLoyaltyVolumeAsync(ct)).ToList();
        if (!datos.Any()) { ConsoleDashboard.Warning("No hay programas registrados."); return; }

        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn("[bold]Aerolínea[/]");
        table.AddColumn("[bold]Programa[/]");
        table.AddColumn(new TableColumn("[bold]Millas/$[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Cuentas[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Millas generadas[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Millas redimidas[/]").RightAligned());

        foreach (var d in datos)
            table.AddRow(Markup.Escape(d.AirlineName), Markup.Escape(d.ProgramName),
                $"[aqua]{d.MilesPerDollar:F2}[/]", $"{d.TotalAccountsEnrolled:N0}",
                $"[bold green]{d.TotalMilesEarned:N0}[/]", $"[yellow]{d.TotalMilesRedeemed:N0}[/]");

        ConsoleDashboard.ShowTablePanel("AEROLÍNEAS – VOLUMEN FIDELIZACIÓN", table);
    }

    private async Task ReporteViajerosFrecuentesAsync(CancellationToken ct)
    {
        ConsoleDashboard.SubScreenTitle("Ranking de viajeros frecuentes");
        var datos = (await _svc.GetFrequentTravelersAsync(10, ct)).ToList();
        if (!datos.Any()) { ConsoleDashboard.Warning("No hay transacciones de millas."); return; }

        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn(new TableColumn("[bold]#[/]").RightAligned());
        table.AddColumn("[bold]Pasajero[/]");
        table.AddColumn("[bold]Nro. viajero[/]");
        table.AddColumn(new TableColumn("[bold]Vuelos[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Millas ganadas[/]").RightAligned());
        table.AddColumn(new TableColumn("[bold]Disponibles[/]").RightAligned());
        table.AddColumn("[bold]Tier[/]");

        int pos = 1;
        foreach (var d in datos)
        {
            var tierColor = d.TierName.ToUpperInvariant() switch
            {
                var t when t.Contains("PLAT")                        => "bold magenta",
                var t when t.Contains("ORO") || t.Contains("GOLD")  => "bold gold3",
                var t when t.Contains("PLATA") || t.Contains("SIL") => "silver",
                _                                                     => "grey"
            };
            var medal = pos switch { 1 => "🥇", 2 => "🥈", 3 => "🥉", _ => $"{pos}." };
            table.AddRow(medal, Markup.Escape(d.PassengerName), Markup.Escape(d.FrequentFlyerNumber),
                $"[bold]{d.TotalTransactions}[/]", $"[green]{d.TotalMilesEarned:N0}[/]",
                $"[aqua]{d.AvailableMiles:N0}[/]", $"[{tierColor}]{Markup.Escape(d.TierName)}[/]");
            pos++;
        }
        ConsoleDashboard.ShowTablePanel("RANKING VIAJEROS FRECUENTES", table);
    }

    private async Task ReporteResumenAsync(CancellationToken ct)
    {
        ConsoleDashboard.SubScreenTitle("Resumen ejecutivo del programa de millas");
        var s = await _svc.GetSummaryAsync(ct);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderStyle(Style.Parse("grey"))
            .AddColumn(new TableColumn("[bold steelblue1]Indicador[/]"))
            .AddColumn(new TableColumn("[bold]Valor[/]").RightAligned());

        table.AddRow("Total cuentas activas",      $"[bold]{s.TotalAccounts:N0}[/]");
        table.AddRow("Programas de fidelización",  $"[bold]{s.TotalPrograms}[/]");
        table.AddRow("Millas totales acumuladas",  $"[bold green]{s.TotalMilesEarned:N0}[/]");
        table.AddRow("Millas totales redimidas",   $"[bold yellow]{s.TotalMilesRedeemed:N0}[/]");
        table.AddRow("Millas en circulación",      $"[bold aqua]{s.TotalMilesInCirculation:N0}[/]");
        table.AddRow("Tasa global de redención",   $"[bold]{s.RedemptionRate:F1}%[/]");
        table.AddRow("Pasajero con más millas",    $"[bold gold3]{Markup.Escape(s.TopPassengerName)}[/]");
        table.AddRow("Millas del pasajero top",    $"[bold gold3]{s.TopPassengerMiles:N0}[/]");

        AnsiConsole.Write(
            new Panel(table)
                .Header("[seagreen1 bold]  RESUMEN EJECUTIVO – FIDELIZACIÓN  [/]")
                .Border(BoxBorder.Double)
                .BorderStyle(Style.Parse("seagreen1"))
                .Expand());
    }
}
