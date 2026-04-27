namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Domain.Repositories;

public sealed record TopMilesAccumulatedRow(
    int    PassengerId,
    string PassengerName,
    string ProgramName,
    string TierName,
    int    TotalMiles,
    int    AvailableMiles,
    int    RedeemedMiles);

public sealed record TopMilesRedeemedRow(
    int    PassengerId,
    string PassengerName,
    int    TotalRedeemed,
    int    TotalEarned,
    double RedemptionRate);

public sealed record AirlineLoyaltyVolumeRow(
    string  AirlineName,
    string  ProgramName,
    decimal MilesPerDollar,
    int     TotalAccountsEnrolled,
    long    TotalMilesEarned,
    long    TotalMilesRedeemed);

public sealed record FrequentTravelerRow(
    int    PassengerId,
    string PassengerName,
    string FrequentFlyerNumber,
    int    TotalTransactions,
    int    TotalMilesEarned,
    int    AvailableMiles,
    string TierName);

public sealed record LoyaltySummaryRow(
    int    TotalAccounts,
    long   TotalMilesEarned,
    long   TotalMilesRedeemed,
    long   TotalMilesInCirculation,
    double RedemptionRate,
    int    TotalPrograms,
    string TopPassengerName,
    int    TopPassengerMiles);

public interface ILoyaltyAnalyticsRepository
{
    Task<IEnumerable<TopMilesAccumulatedRow>> GetTopMilesAccumulatedAsync(
        int top, CancellationToken cancellationToken = default);

    Task<IEnumerable<TopMilesRedeemedRow>> GetTopMilesRedeemedAsync(
        int top, CancellationToken cancellationToken = default);

    Task<IEnumerable<AirlineLoyaltyVolumeRow>> GetAirlineLoyaltyVolumeAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<FrequentTravelerRow>> GetFrequentTravelersAsync(
        int top, CancellationToken cancellationToken = default);

    Task<LoyaltySummaryRow> GetSummaryAsync(CancellationToken cancellationToken = default);
}
