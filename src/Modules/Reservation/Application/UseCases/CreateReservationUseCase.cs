namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class CreateReservationUseCase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly AppDbContext           _context;

    public CreateReservationUseCase(IReservationRepository reservationRepository, AppDbContext context)
    {
        _reservationRepository = reservationRepository;
        _context               = context;
    }

    public async Task<ReservationAggregate> ExecuteAsync(
        int               customerId,
        int               scheduledFlightId,
        int               reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var code = await GenerateUniqueCodeAsync(cancellationToken);

        return await _reservationRepository.CreateReservationWithInitialHistoryAsync(
            code,
            customerId,
            scheduledFlightId,
            reservationStatusId,
            cancellationToken);
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        while (true)
        {
            var code = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray());

            if (!await _context.Reservations.AsNoTracking()
                    .AnyAsync(r => r.ReservationCode == code, ct))
                return code;
        }
    }
}
