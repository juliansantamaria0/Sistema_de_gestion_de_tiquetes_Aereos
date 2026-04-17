namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

/// <summary>
/// Contrato que deben implementar todas las UIs de consola de los módulos.
/// Permite al punto de entrada principal (Program.cs) invocar cualquier
/// módulo de forma polimórfica sin conocer su implementación concreta.
/// </summary>
public interface IModuleUI
{
    /// <summary>
    /// Clave única del módulo. Usada para enrutamiento desde el menú principal.
    /// Convenio: nombre de la tabla en minúsculas (ej: "country", "airline").
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Título legible para mostrar en el menú principal de la aplicación.
    /// Ejemplo: "Country Management", "Airline Management".
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Punto de entrada del módulo. Muestra el menú del módulo y
    /// gestiona el ciclo de vida de la interacción con el usuario.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelación limpia.</param>
    Task RunAsync(CancellationToken cancellationToken = default);
}
