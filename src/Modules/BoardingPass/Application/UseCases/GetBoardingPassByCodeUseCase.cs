namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories;

public sealed class GetBoardingPassByCodeUseCase
{
    private readonly IBoardingPassRepository _repository;

    public GetBoardingPassByCodeUseCase(IBoardingPassRepository repository)
    {
        _repository = repository;
    }

    public async Task<BoardingPassAggregate?> ExecuteAsync(
        string            boardingPassCode,
        CancellationToken cancellationToken = default)
    {
        var normalized = (boardingPassCode ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalized)) return null;
        return await _repository.GetByCodeAsync(normalized, cancellationToken);
    }
}
