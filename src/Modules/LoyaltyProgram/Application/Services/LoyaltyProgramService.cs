namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.UseCases;

public sealed class LoyaltyProgramService : ILoyaltyProgramService
{
    private readonly CreateLoyaltyProgramUseCase         _create;
    private readonly DeleteLoyaltyProgramUseCase         _delete;
    private readonly GetAllLoyaltyProgramsUseCase        _getAll;
    private readonly GetLoyaltyProgramByIdUseCase        _getById;
    private readonly UpdateLoyaltyProgramUseCase         _update;
    private readonly GetLoyaltyProgramByAirlineUseCase   _getByAirline;

    public LoyaltyProgramService(
        CreateLoyaltyProgramUseCase        create,
        DeleteLoyaltyProgramUseCase        delete,
        GetAllLoyaltyProgramsUseCase       getAll,
        GetLoyaltyProgramByIdUseCase       getById,
        UpdateLoyaltyProgramUseCase        update,
        GetLoyaltyProgramByAirlineUseCase  getByAirline)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _update       = update;
        _getByAirline = getByAirline;
    }

    public async Task<LoyaltyProgramDto> CreateAsync(
        int               airlineId,
        string            name,
        decimal           milesPerDollar,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(airlineId, name, milesPerDollar, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<LoyaltyProgramDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<LoyaltyProgramDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        decimal           milesPerDollar,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, milesPerDollar, cancellationToken);

    public async Task<LoyaltyProgramDto?> GetByAirlineAsync(
        int               airlineId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByAirline.ExecuteAsync(airlineId, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    private static LoyaltyProgramDto ToDto(
        Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate.LoyaltyProgramAggregate agg)
        => new(agg.Id.Value, agg.AirlineId, agg.Name, agg.MilesPerDollar);
}
