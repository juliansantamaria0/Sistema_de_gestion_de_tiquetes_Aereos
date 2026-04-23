namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;





public sealed class UpdateFlightCancellationUseCase
{
    private readonly IFlightCancellationRepository _repository;
    private readonly IUnitOfWork                   _unitOfWork;

    public UpdateFlightCancellationUseCase(IFlightCancellationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var flightCancellation = await _repository.GetByIdAsync(new FlightCancellationId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"FlightCancellation with id {id} was not found.");

        flightCancellation.UpdateNotes(notes);
        await _repository.UpdateAsync(flightCancellation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
