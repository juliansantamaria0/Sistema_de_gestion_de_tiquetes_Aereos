namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.Interfaces;

public interface IFareTypeService
{
    Task<FareTypeDto?>             GetByIdAsync(int id,                                                                                    CancellationToken cancellationToken = default);
    Task<IEnumerable<FareTypeDto>> GetAllAsync(                                                                                            CancellationToken cancellationToken = default);
    Task<FareTypeDto>              CreateAsync(string name, bool isRefundable, bool isChangeable, int advancePurchaseDays, bool baggageIncluded, CancellationToken cancellationToken = default);
    Task                           UpdateAsync(int id, string name, bool isRefundable, bool isChangeable, int advancePurchaseDays, bool baggageIncluded, CancellationToken cancellationToken = default);
    Task                           DeleteAsync(int id,                                                                                    CancellationToken cancellationToken = default);
}

public sealed record FareTypeDto(
    int    Id,
    string Name,
    bool   IsRefundable,
    bool   IsChangeable,
    int    AdvancePurchaseDays,
    bool   BaggageIncluded);
