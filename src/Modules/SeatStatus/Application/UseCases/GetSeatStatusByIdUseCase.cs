namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;

public sealed class GetSeatStatusByIdUseCase
{
    private readonly ISeatStatusRepository _repository;

    public GetSeatStatusByIdUseCase(ISeatStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<SeatStatusAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new SeatStatusId(id), cancellationToken);
}
