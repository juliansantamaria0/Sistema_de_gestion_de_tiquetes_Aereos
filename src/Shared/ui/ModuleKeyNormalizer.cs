using System.Text;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;





public static class ModuleKeyNormalizer
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var sb = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            if (ch is '_' or '-' or ' ')
                continue;
            sb.Append(char.ToLowerInvariant(ch));
        }

        return sb.ToString();
    }
}
