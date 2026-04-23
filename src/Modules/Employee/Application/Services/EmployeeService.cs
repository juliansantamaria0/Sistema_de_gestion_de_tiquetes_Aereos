namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Services;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly AppDbContext        _context;

    public EmployeeService(IEmployeeRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        if (!await _context.Persons.AsNoTracking().AnyAsync(x => x.Id == request.PersonId, ct))
            throw new InvalidOperationException($"No existe la persona con id {request.PersonId}.");

        if (await _repository.ExistsByPersonIdAsync(request.PersonId, ct))
            throw new InvalidOperationException("Ya existe un empleado asociado a esta persona.");

        var entity = EmployeeAggregate.Create(
            request.PersonId, request.AirlineId, request.HireDate,
            request.JobPositionId, request.IsActive);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(EmployeeId.New(id), ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repository.GetAllAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(EmployeeId.New(id), ct)
            ?? throw new KeyNotFoundException($"Employee with id {id} not found.");
        entity.Update(request.AirlineId, request.JobPositionId, request.HireDate, request.IsActive);
        _repository.Update(entity);
        await _unitOfWork.CommitAsync(ct);
        return Map(entity);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(EmployeeId.New(id), ct)
            ?? throw new KeyNotFoundException($"Employee with id {id} not found.");
        _repository.Delete(entity);
        await _unitOfWork.CommitAsync(ct);
    }

    private static EmployeeDto Map(EmployeeAggregate a) =>
        new(a.Id.Value, a.PersonId, a.AirlineId, a.JobPositionId,
            a.HireDate, a.IsActive, a.CreatedAt, a.UpdatedAt);
}
