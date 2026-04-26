namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class CurrentUser
{
    private static int? _userId;
    private static string? _username;
    private static int? _personId;
    private static int? _customerId;

    public static int? UserId => _userId;
    public static string? Username => _username;
    public static int? PersonId => _personId;
    public static int? CustomerId => _customerId;

    public static bool IsAuthenticated => _userId.HasValue;

    public static void Set(int userId, string username, int personId, int customerId)
    {
        _userId = userId;
        _username = username;
        _personId = personId;
        _customerId = customerId;
    }

    public static void Clear()
    {
        _userId = null;
        _username = null;
        _personId = null;
        _customerId = null;
    }
}