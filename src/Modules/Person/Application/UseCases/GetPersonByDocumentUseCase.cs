namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;

/// <summary>
/// Busca una persona por su tipo + número de documento.
/// La UNIQUE sobre (document_type_id, document_number) garantiza un único resultado.
/// </summary>
public sealed class GetPersonByDocumentUseCase
{
    private readonly IPersonRepository _repository;

    public GetPersonByDocumentUseCase(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<PersonAggregate?> ExecuteAsync(
        int               documentTypeId,
        string            documentNumber,
        CancellationToken cancellationToken = default)
        => await _repository.GetByDocumentAsync(documentTypeId, documentNumber, cancellationToken);
}
