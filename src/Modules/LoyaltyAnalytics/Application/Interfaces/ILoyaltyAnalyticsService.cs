namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Application.Interfaces;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Domain.Repositories;

public interface ILoyaltyAnalyticsService
{
    Task<IEnumerable<TopMilesAccumulatedRow>> GetTopMilesAccumulatedAsync(
        int top = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<TopMilesRedeemedRow>> GetTopMilesRedeemedAsync(
        int top = 10, CancellationToken cancellationToken = default);

    Task<IEnumerable<AirlineLoyaltyVolumeRow>> GetAirlineLoyaltyVolumeAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<FrequentTravelerRow>> GetFrequentTravelersAsync(
        int top = 10, CancellationToken cancellationToken = default);

    Task<LoyaltySummaryRow> GetSummaryAsync(CancellationToken cancellationToken = default);
}
