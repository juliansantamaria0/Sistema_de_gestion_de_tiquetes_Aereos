namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;


public sealed class GateService : IGateService
{
    private readonly IGateRepository _repository;
    private readonly IUnitOfWork     _unitOfWork;

    public GateService(IGateRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GateDto> CreateAsync(CreateGateRequest request, CancellationToken cancellationToken = default)
    {
        var gate = GateAggregate.Create(request.TerminalId, request.Code, request.IsActive);
        await _repository.AddAsync(gate, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return MapToDto(gate);
    }

    public async Task<GateDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var gate = await _repository.GetByIdAsync(GateId.New(id), cancellationToken);
        return gate is null ? null : MapToDto(gate);
    }

    public async Task<IReadOnlyList<GateDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _repository.GetAllAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<GateDto> UpdateAsync(int id, UpdateGateRequest request, CancellationToken cancellationToken = default)
    {
        var gate = await _repository.GetByIdAsync(GateId.New(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Gate with id {id} not found.");

        gate.Update(request.TerminalId, request.Code, request.IsActive);
        _repository.Update(gate);
        await _unitOfWork.CommitAsync(cancellationToken);
        return MapToDto(gate);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var gate = await _repository.GetByIdAsync(GateId.New(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Gate with id {id} not found.");

        _repository.Delete(gate);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private static GateDto MapToDto(GateAggregate g) =>
        new(g.Id.Value, g.TerminalId, g.Code, g.IsActive);
}
