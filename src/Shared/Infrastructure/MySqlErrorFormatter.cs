using MySqlConnector;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public static class MySqlErrorFormatter
{
    /// <summary>Mensaje en español, sin detalles técnicos, para mostrar al usuario final.</summary>
    public static string ToUserMessage(MySqlException ex)
    {
        if (ex.Number is 2002 or 2003)
            return "No se pudo conectar al servidor de base de datos. Compruebe su red o que el servicio (Aiven) esté en línea.";

        return ex.ErrorCode switch
        {
            MySqlErrorCode.LockDeadlock
                => "Otro proceso estaba actualizando los mismos datos. Intente de nuevo en unos segundos.",
            MySqlErrorCode.LockWaitTimeout
                => "Tiempo de espera agotado por bloqueo en la base de datos. Intente de nuevo.",
            MySqlErrorCode.DuplicateKeyEntry
                => "No se pudo guardar: el registro ya existe o se duplicó una clave única.",
            MySqlErrorCode.UnableToConnectToHost
                => "No se pudo conectar al servidor de base de datos. Compruebe su red o que el servicio (Aiven) esté en línea.",
            _ => MapByNumber(ex.Number) ?? "Error al acceder a la base de datos. Intente de nuevo o contacte al administrador."
        };
    }

    private static string? MapByNumber(int number) =>
        number switch
        {
            1205 => "Tiempo de espera agotado por bloqueo. Intente de nuevo.",
            1213 => "Interbloqueo detectado. Intente de nuevo en unos segundos.",
            1040 => "El servidor acepta demasiadas conexiones. Intente más tarde.",
            2006 => "Se perdió la conexión con el servidor. Verifique su red e intente otra vez.",
            _ => null
        };
}
