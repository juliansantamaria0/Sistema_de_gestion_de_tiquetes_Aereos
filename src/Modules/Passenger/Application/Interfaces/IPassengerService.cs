namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.Interfaces;

public interface IPassengerService
{
    Task<PassengerDto?>             GetByIdAsync(int id,                                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerDto>> GetAllAsync(                                                                      CancellationToken cancellationToken = default);
    Task<PassengerDto?>             GetByPersonAsync(int personId,                                                    CancellationToken cancellationToken = default);
    Task<PassengerDto>              CreateAsync(int personId, string? frequentFlyerNumber, int? nationalityId,        CancellationToken cancellationToken = default);
    Task                            UpdateAsync(int id, string? frequentFlyerNumber, int? nationalityId,             CancellationToken cancellationToken = default);
    Task                            DeleteAsync(int id,                                                              CancellationToken cancellationToken = default);
}

public sealed record PassengerDto(
    int      Id,
    int      PersonId,
    string?  FrequentFlyerNumber,
    int?     NationalityId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
