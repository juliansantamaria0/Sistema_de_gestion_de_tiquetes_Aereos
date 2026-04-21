namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.Interfaces;

public interface IDiscountTypeService
{
    Task<DiscountTypeDto?>             GetByIdAsync(int id,                                                          CancellationToken cancellationToken = default);
    Task<IEnumerable<DiscountTypeDto>> GetAllAsync(                                                                  CancellationToken cancellationToken = default);
    Task<DiscountTypeDto>              CreateAsync(string name, decimal percentage, int? ageMin, int? ageMax,       CancellationToken cancellationToken = default);
    Task                               UpdateAsync(int id, string name, decimal percentage, int? ageMin, int? ageMax, CancellationToken cancellationToken = default);
    Task                               DeleteAsync(int id,                                                          CancellationToken cancellationToken = default);
}

public sealed record DiscountTypeDto(
    int     Id,
    string  Name,
    decimal Percentage,
    int?    AgeMin,
    int?    AgeMax);
