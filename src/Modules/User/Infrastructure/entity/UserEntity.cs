namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Infrastructure.Entity;

public sealed class UserEntity
{
    public int       UserId       { get; set; }
    public int       PersonId     { get; set; }
    public int       RoleId       { get; set; }
    public string    Username     { get; set; } = string.Empty;
    public string    PasswordHash { get; set; } = string.Empty;
    public bool      IsActive     { get; set; }
    public DateTime  CreatedAt    { get; set; }
    public DateTime? UpdatedAt    { get; set; }
}
