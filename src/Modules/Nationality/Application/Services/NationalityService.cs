namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class NationalityService : INationalityService
{
    private readonly INationalityRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public NationalityService(INationalityRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<NationalityDto> CreateAsync(CreateNationalityRequest request, CancellationToken ct = default)
    {
        var entity = NationalityAggregate.Create(request.CountryId, request.Demonym);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task<NationalityDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(NationalityId.New(id), ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<NationalityDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<NationalityDto> UpdateAsync(int id, UpdateNationalityRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(NationalityId.New(id), ct)
            ?? throw new KeyNotFoundException($"Nationality with id {id} not found.");
        entity.Update(request.CountryId, request.Demonym);
        _repository.Update(entity);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(NationalityId.New(id), ct)
            ?? throw new KeyNotFoundException($"Nationality with id {id} not found.");
        _repository.Delete(entity);
        await _unitOfWork.CommitAsync(ct);
    }

    private static NationalityDto Map(NationalityAggregate a) =>
        new(a.Id.Value, a.CountryId, a.Demonym);
}
