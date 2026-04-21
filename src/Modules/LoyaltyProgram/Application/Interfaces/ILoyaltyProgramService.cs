namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.Interfaces;

public interface ILoyaltyProgramService
{
    Task<LoyaltyProgramDto?>             GetByIdAsync(int id,                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyProgramDto>> GetAllAsync(                                                      CancellationToken cancellationToken = default);
    Task<LoyaltyProgramDto?>             GetByAirlineAsync(int airlineId,                                  CancellationToken cancellationToken = default);
    Task<LoyaltyProgramDto>              CreateAsync(int airlineId, string name, decimal milesPerDollar,   CancellationToken cancellationToken = default);
    Task                                 UpdateAsync(int id, string name, decimal milesPerDollar,          CancellationToken cancellationToken = default);
    Task                                 DeleteAsync(int id,                                               CancellationToken cancellationToken = default);
}

public sealed record LoyaltyProgramDto(int Id, int AirlineId, string Name, decimal MilesPerDollar);
