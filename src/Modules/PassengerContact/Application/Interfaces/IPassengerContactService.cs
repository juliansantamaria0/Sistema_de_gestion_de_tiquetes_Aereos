namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.Interfaces;

public interface IPassengerContactService
{
    Task<PassengerContactDto?>             GetByIdAsync(int id,                                                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerContactDto>> GetAllAsync(                                                                                      CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerContactDto>> GetByPassengerAsync(int passengerId,                                                              CancellationToken cancellationToken = default);
    Task<PassengerContactDto>              CreateAsync(int passengerId, int contactTypeId, string fullName, string phone, string? relationship, CancellationToken cancellationToken = default);
    Task                                   UpdateAsync(int id, string fullName, string phone, string? relationship,                          CancellationToken cancellationToken = default);
    Task                                   DeleteAsync(int id,                                                                               CancellationToken cancellationToken = default);
}

public sealed record PassengerContactDto(
    int     Id,
    int     PassengerId,
    int     ContactTypeId,
    string  FullName,
    string  Phone,
    string? Relationship);
