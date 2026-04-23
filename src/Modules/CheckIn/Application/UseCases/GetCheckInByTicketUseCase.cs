namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.Repositories;







public sealed class GetCheckInByTicketUseCase
{
    private readonly ICheckInRepository _repository;

    public GetCheckInByTicketUseCase(ICheckInRepository repository)
    {
        _repository = repository;
    }

    public async Task<CheckInAggregate?> ExecuteAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByTicketAsync(ticketId, cancellationToken);
}
