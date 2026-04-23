namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories;






public sealed class GetBoardingPassByCheckInUseCase
{
    private readonly IBoardingPassRepository _repository;

    public GetBoardingPassByCheckInUseCase(IBoardingPassRepository repository)
    {
        _repository = repository;
    }

    public async Task<BoardingPassAggregate?> ExecuteAsync(
        int               checkInId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByCheckInAsync(checkInId, cancellationToken);
}
