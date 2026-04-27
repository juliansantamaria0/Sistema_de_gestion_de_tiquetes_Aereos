namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class LoyaltyAnalyticsRepository : ILoyaltyAnalyticsRepository
{
    private readonly AppDbContext _db;

    public LoyaltyAnalyticsRepository(AppDbContext db)
    {
        _db = db;
    }

    // 1. TOP CLIENTES CON MAS MILLAS ACUMULADAS

    public async Task<IEnumerable<TopMilesAccumulatedRow>> GetTopMilesAccumulatedAsync(
        int top, CancellationToken cancellationToken = default)
    {
        var rows = await (
            from account   in _db.LoyaltyAccounts.AsNoTracking()
            join passenger in _db.Passengers.AsNoTracking()
                on account.PassengerId equals passenger.Id
            join person    in _db.Persons.AsNoTracking()
                on passenger.PersonId equals person.Id
            join program   in _db.LoyaltyPrograms.AsNoTracking()
                on account.LoyaltyProgramId equals program.Id
            join tier      in _db.LoyaltyTiers.AsNoTracking()
                on account.LoyaltyTierId equals tier.Id
            orderby account.TotalMiles descending
            select new TopMilesAccumulatedRow(
                account.PassengerId,
                person.FirstName + " " + person.LastName,
                program.Name,
                tier.Name,
                account.TotalMiles,
                account.AvailableMiles,
                account.TotalMiles - account.AvailableMiles)
        ).Take(top).ToListAsync(cancellationToken);

        return rows;
    }

    // 2. CLIENTES QUE MAS MILLAS HAN REDIMIDO
    // EF Core no traduce GroupBy con subqueries, se agrupa en memoria.

    public async Task<IEnumerable<TopMilesRedeemedRow>> GetTopMilesRedeemedAsync(
        int top, CancellationToken cancellationToken = default)
    {
        var raw = await (
            from tx        in _db.LoyaltyTransactions.AsNoTracking()
            join account   in _db.LoyaltyAccounts.AsNoTracking()
                on tx.LoyaltyAccountId equals account.Id
            join passenger in _db.Passengers.AsNoTracking()
                on account.PassengerId equals passenger.Id
            join person    in _db.Persons.AsNoTracking()
                on passenger.PersonId equals person.Id
            select new
            {
                account.PassengerId,
                FullName       = person.FirstName + " " + person.LastName,
                tx.TransactionType,
                tx.Miles
            }
        ).ToListAsync(cancellationToken);

        return raw
            .GroupBy(x => new { x.PassengerId, x.FullName })
            .Select(g =>
            {
                var earned   = g.Where(t => t.TransactionType == "EARN")  .Sum(t => t.Miles);
                var redeemed = g.Where(t => t.TransactionType == "REDEEM").Sum(t => t.Miles);
                return new TopMilesRedeemedRow(
                    g.Key.PassengerId,
                    g.Key.FullName,
                    redeemed,
                    earned,
                    earned > 0 ? Math.Round((double)redeemed / earned * 100, 1) : 0.0);
            })
            .Where(r => r.TotalRedeemed > 0)
            .OrderByDescending(r => r.TotalRedeemed)
            .Take(top)
            .ToList();
    }

    // 3. AEROLINEAS CON MAYOR VOLUMEN DE FIDELIZACION
    // EF Core no traduce IQueryable dentro de let, se hace en pasos.

    public async Task<IEnumerable<AirlineLoyaltyVolumeRow>> GetAirlineLoyaltyVolumeAsync(
        CancellationToken cancellationToken = default)
    {
        var programs = await (
            from program in _db.LoyaltyPrograms.AsNoTracking()
            join airline in _db.Airlines.AsNoTracking()
                on program.AirlineId equals airline.AirlineId
            select new
            {
                program.Id,
                AirlineName    = airline.Name,
                ProgramName    = program.Name,
                program.MilesPerDollar
            }
        ).ToListAsync(cancellationToken);

        var rows = new List<AirlineLoyaltyVolumeRow>();

        foreach (var prog in programs)
        {
            var accountIds = await _db.LoyaltyAccounts.AsNoTracking()
                .Where(a => a.LoyaltyProgramId == prog.Id)
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            var enrolled = accountIds.Count;

            var earned = accountIds.Count == 0 ? 0L :
                await _db.LoyaltyTransactions.AsNoTracking()
                    .Where(tx => accountIds.Contains(tx.LoyaltyAccountId)
                                 && tx.TransactionType == "EARN")
                    .SumAsync(tx => (long?)tx.Miles, cancellationToken) ?? 0L;

            var redeemed = accountIds.Count == 0 ? 0L :
                await _db.LoyaltyTransactions.AsNoTracking()
                    .Where(tx => accountIds.Contains(tx.LoyaltyAccountId)
                                 && tx.TransactionType == "REDEEM")
                    .SumAsync(tx => (long?)tx.Miles, cancellationToken) ?? 0L;

            rows.Add(new AirlineLoyaltyVolumeRow(
                prog.AirlineName,
                prog.ProgramName,
                prog.MilesPerDollar,
                enrolled,
                earned,
                redeemed));
        }

        return rows.OrderByDescending(r => r.TotalMilesEarned).ToList();
    }

    // 4. RANKING DE VIAJEROS FRECUENTES

    public async Task<IEnumerable<FrequentTravelerRow>> GetFrequentTravelersAsync(
        int top, CancellationToken cancellationToken = default)
    {
        var rows = await (
            from account   in _db.LoyaltyAccounts.AsNoTracking()
            join passenger in _db.Passengers.AsNoTracking()
                on account.PassengerId equals passenger.Id
            join person    in _db.Persons.AsNoTracking()
                on passenger.PersonId equals person.Id
            join tier      in _db.LoyaltyTiers.AsNoTracking()
                on account.LoyaltyTierId equals tier.Id
            let txCount = _db.LoyaltyTransactions
                .Count(tx => tx.LoyaltyAccountId == account.Id && tx.TransactionType == "EARN")
            let txMiles = _db.LoyaltyTransactions
                .Where(tx => tx.LoyaltyAccountId == account.Id && tx.TransactionType == "EARN")
                .Sum(tx => (int?)tx.Miles) ?? 0
            where txCount > 0
            orderby txCount descending, account.TotalMiles descending
            select new FrequentTravelerRow(
                account.PassengerId,
                person.FirstName + " " + person.LastName,
                passenger.FrequentFlyerNumber ?? "—",
                txCount,
                txMiles,
                account.AvailableMiles,
                tier.Name)
        ).Take(top).ToListAsync(cancellationToken);

        return rows;
    }

    // 5. RESUMEN EJECUTIVO

    public async Task<LoyaltySummaryRow> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totalAccounts = await _db.LoyaltyAccounts.AsNoTracking()
            .CountAsync(cancellationToken);

        var totalPrograms = await _db.LoyaltyPrograms.AsNoTracking()
            .CountAsync(cancellationToken);

        var totalEarned = await _db.LoyaltyTransactions.AsNoTracking()
            .Where(tx => tx.TransactionType == "EARN")
            .SumAsync(tx => (long?)tx.Miles, cancellationToken) ?? 0L;

        var totalRedeemed = await _db.LoyaltyTransactions.AsNoTracking()
            .Where(tx => tx.TransactionType == "REDEEM")
            .SumAsync(tx => (long?)tx.Miles, cancellationToken) ?? 0L;

        var redemptionRate = totalEarned > 0
            ? Math.Round((double)totalRedeemed / totalEarned * 100, 1)
            : 0.0;

        var topAccount = await (
            from account   in _db.LoyaltyAccounts.AsNoTracking()
            join passenger in _db.Passengers.AsNoTracking()
                on account.PassengerId equals passenger.Id
            join person    in _db.Persons.AsNoTracking()
                on passenger.PersonId equals person.Id
            orderby account.TotalMiles descending
            select new
            {
                Name  = person.FirstName + " " + person.LastName,
                account.TotalMiles
            }
        ).FirstOrDefaultAsync(cancellationToken);

        return new LoyaltySummaryRow(
            TotalAccounts:           totalAccounts,
            TotalMilesEarned:        totalEarned,
            TotalMilesRedeemed:      totalRedeemed,
            TotalMilesInCirculation: totalEarned - totalRedeemed,
            RedemptionRate:          redemptionRate,
            TotalPrograms:           totalPrograms,
            TopPassengerName:        topAccount?.Name ?? "—",
            TopPassengerMiles:       topAccount?.TotalMiles ?? 0);
    }
}