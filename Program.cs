using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddSingleton<MainMenu>();

using var host = builder.Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var menu = scope.ServiceProvider.GetRequiredService<MainMenu>();
    await menu.RunAsync();
}

internal sealed class MainMenu(IEnumerable<IModuleUI> modules)
{
    private readonly List<IModuleUI> _modules = modules
        .OrderBy(m => m.Title)
        .ToList();

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Air Tickets")
                .Centered()
                .Color(Color.CornflowerBlue));

        AnsiConsole.MarkupLine("[grey]Sistema de gestion de tiquetes aereos[/]");

        if (_modules.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No hay modulos registrados en DI.[/]");
            return;
        }

        var exit = false;
        while (!exit && !cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.WriteLine();
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Selecciona un modulo[/]")
                    .PageSize(18)
                    .AddChoices(_modules.Select(m => m.Title).Append("Salir")));

            if (selected == "Salir")
            {
                exit = true;
                continue;
            }

            var module = _modules.First(m => m.Title == selected);

            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[green]{module.Title}[/]"));

            try
            {
                await module.RunAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Presiona una tecla para volver al menu principal...[/]");
            Console.ReadKey(intercept: true);
            AnsiConsole.Clear();
        }

        AnsiConsole.MarkupLine("[green]Hasta luego.[/]");
    }
}
