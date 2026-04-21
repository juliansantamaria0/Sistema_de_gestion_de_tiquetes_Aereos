namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class JobPositionService : IJobPositionService
{
    private readonly IJobPositionRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public JobPositionService(IJobPositionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<JobPositionDto> CreateAsync(CreateJobPositionRequest request, CancellationToken ct = default)
    {
        var entity = JobPositionAggregate.Create(request.Name, request.Department);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task<JobPositionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(JobPositionId.New(id), ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<JobPositionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<JobPositionDto> UpdateAsync(int id, UpdateJobPositionRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(JobPositionId.New(id), ct)
            ?? throw new KeyNotFoundException($"JobPosition with id {id} not found.");
        entity.Update(request.Name, request.Department);
        _repository.Update(entity);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(JobPositionId.New(id), ct)
            ?? throw new KeyNotFoundException($"JobPosition with id {id} not found.");
        _repository.Delete(entity);
        await _unitOfWork.CommitAsync(ct);
    }

    private static JobPositionDto Map(JobPositionAggregate a) =>
        new(a.Id.Value, a.Name, a.Department);
}
