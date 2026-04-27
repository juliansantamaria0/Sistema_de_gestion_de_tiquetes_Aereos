namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateBoardingPassUseCase
{
    private readonly IBoardingPassRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public CreateBoardingPassUseCase(IBoardingPassRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BoardingPassAggregate> ExecuteAsync(
        string            boardingPassCode,
        int               checkInId,
        int?              gateId,
        string?           boardingGroup,
        int               flightSeatId,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = (boardingPassCode ?? string.Empty).Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new InvalidOperationException("El código del pase de abordar es obligatorio.");

        if (await _repository.BoardingPassCodeExistsAsync(normalizedCode, cancellationToken))
            throw new InvalidOperationException(
                $"Ya existe un pase de abordar con el código {normalizedCode}.");

        var boardingPass = new BoardingPassAggregate(
            new BoardingPassId(0),
            normalizedCode,
            checkInId,
            gateId,
            boardingGroup,
            flightSeatId,
            DateTime.UtcNow);

        await _repository.AddAsync(boardingPass, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return boardingPass;
    }
}
