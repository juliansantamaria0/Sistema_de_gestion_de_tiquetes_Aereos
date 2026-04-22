public sealed class EmployeeEntity
{
    public int       EmployeeId    { get; set; }
    public int       PersonId      { get; set; }
    public int       AirlineId     { get; set; }
    public int?      JobPositionId { get; set; } 
    public DateOnly  HireDate      { get; set; }
    public bool      IsActive      { get; set; }
    public DateTime  CreatedAt     { get; set; }
    public DateTime? UpdatedAt     { get; set; }
}