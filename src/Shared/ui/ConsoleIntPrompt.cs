using System.Globalization;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

/// <summary>Validación de enteros reutilizable (mismo criterio que <see cref="ReflectiveModuleUI{TService}"/>.</summary>
public static class ConsoleIntPrompt
{
    public static bool TryParseFirstSegmentAsInt32(string listChoiceLine, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(listChoiceLine)) return false;
        var first = listChoiceLine.Split(" - ", StringSplitOptions.None)[0].Trim();
        return int.TryParse(first, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value > 0;
    }
}
