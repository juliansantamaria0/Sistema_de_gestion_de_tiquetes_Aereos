namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;


public sealed class TerminalService : ITerminalService
{
    private readonly ITerminalRepository _repository;
    private readonly IUnitOfWork         _unitOfWork;

    public TerminalService(ITerminalRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TerminalDto> CreateAsync(CreateTerminalRequest request, CancellationToken cancellationToken = default)
    {
        var terminal = TerminalAggregate.Create(request.AirportId, request.Name, request.IsInternational);
        await _repository.AddAsync(terminal, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return MapToDto(terminal);
    }

    public async Task<TerminalDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var t = await _repository.GetByIdAsync(TerminalId.New(id), cancellationToken);
        return t is null ? null : MapToDto(t);
    }

    public async Task<IReadOnlyList<TerminalDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _repository.GetAllAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<TerminalDto> UpdateAsync(int id, UpdateTerminalRequest request, CancellationToken cancellationToken = default)
    {
        var terminal = await _repository.GetByIdAsync(TerminalId.New(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Terminal with id {id} not found.");

        terminal.Update(request.AirportId, request.Name, request.IsInternational);
        _repository.Update(terminal);
        await _unitOfWork.CommitAsync(cancellationToken);
        return MapToDto(terminal);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var terminal = await _repository.GetByIdAsync(TerminalId.New(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Terminal with id {id} not found.");

        _repository.Delete(terminal);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private static TerminalDto MapToDto(TerminalAggregate t) =>
        new(t.Id.Value, t.AirportId, t.Name, t.IsInternational, t.CreatedAt);
}
