namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteFlightCabinPriceUseCase
{
    private readonly IFlightCabinPriceRepository _repository;
    private readonly IUnitOfWork                 _unitOfWork;

    public DeleteFlightCabinPriceUseCase(
        IFlightCabinPriceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new FlightCabinPriceId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
