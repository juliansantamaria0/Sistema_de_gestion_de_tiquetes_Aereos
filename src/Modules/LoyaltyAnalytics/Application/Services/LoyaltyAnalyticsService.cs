namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Domain.Repositories;

public sealed class LoyaltyAnalyticsService : ILoyaltyAnalyticsService
{
    private readonly ILoyaltyAnalyticsRepository _repository;

    public LoyaltyAnalyticsService(ILoyaltyAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<TopMilesAccumulatedRow>> GetTopMilesAccumulatedAsync(
        int top = 10, CancellationToken cancellationToken = default)
        => _repository.GetTopMilesAccumulatedAsync(top, cancellationToken);

    public Task<IEnumerable<TopMilesRedeemedRow>> GetTopMilesRedeemedAsync(
        int top = 10, CancellationToken cancellationToken = default)
        => _repository.GetTopMilesRedeemedAsync(top, cancellationToken);

    public Task<IEnumerable<AirlineLoyaltyVolumeRow>> GetAirlineLoyaltyVolumeAsync(
        CancellationToken cancellationToken = default)
        => _repository.GetAirlineLoyaltyVolumeAsync(cancellationToken);

    public Task<IEnumerable<FrequentTravelerRow>> GetFrequentTravelersAsync(
        int top = 10, CancellationToken cancellationToken = default)
        => _repository.GetFrequentTravelersAsync(top, cancellationToken);

    public Task<LoyaltySummaryRow> GetSummaryAsync(CancellationToken cancellationToken = default)
        => _repository.GetSummaryAsync(cancellationToken);
}
