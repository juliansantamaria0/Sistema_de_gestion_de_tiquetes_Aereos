namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Country.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateCountryUseCase
{
    private readonly ICountryRepository _repository;
    private readonly IUnitOfWork        _unitOfWork;

    public CreateCountryUseCase(ICountryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CountryAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var country = new CountryAggregate(new CountryId(await GetNextIdAsync(cancellationToken)), name);
        await _repository.AddAsync(country, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return country;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
