namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;

public sealed class GetGenderByIdUseCase
{
    private readonly IGenderRepository _repository;

    public GetGenderByIdUseCase(IGenderRepository repository)
    {
        _repository = repository;
    }

    public async Task<GenderAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new GenderId(id), cancellationToken);
}
