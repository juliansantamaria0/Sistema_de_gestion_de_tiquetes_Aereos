namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.Interfaces;

public interface IPersonService
{
    Task<PersonDto?>             GetByIdAsync(int id,                                                                                   CancellationToken cancellationToken = default);
    Task<IEnumerable<PersonDto>> GetAllAsync(                                                                                           CancellationToken cancellationToken = default);
    Task<PersonDto?>             GetByDocumentAsync(int documentTypeId, string documentNumber,                                          CancellationToken cancellationToken = default);
    Task<PersonDto>              CreateAsync(int documentTypeId, string documentNumber, string firstName, string lastName, DateOnly? birthDate, int? genderId, CancellationToken cancellationToken = default);
    Task                         UpdateAsync(int id, string firstName, string lastName, DateOnly? birthDate, int? genderId,             CancellationToken cancellationToken = default);
    Task                         DeleteAsync(int id,                                                                                   CancellationToken cancellationToken = default);
}

public sealed record PersonDto(
    int      Id,
    int      DocumentTypeId,
    string   DocumentNumber,
    string   FirstName,
    string   LastName,
    DateOnly? BirthDate,
    int?     GenderId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
