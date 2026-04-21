namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.Interfaces;

public sealed class ReservationDetailConsoleUI
{
    private readonly IReservationDetailService _service;

    public ReservationDetailConsoleUI(IReservationDetailService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== RESERVATION DETAIL MODULE ==========");
            Console.WriteLine("1. List all reservation details");
            Console.WriteLine("2. Get detail by ID");
            Console.WriteLine("3. List details by reservation");
            Console.WriteLine("4. Add passenger to reservation");
            Console.WriteLine("5. Change fare type");
            Console.WriteLine("6. Remove detail");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();              break;
                case "2": await GetByIdAsync();              break;
                case "3": await ListByReservationAsync();    break;
                case "4": await AddPassengerAsync();         break;
                case "5": await ChangeFareTypeAsync();       break;
                case "6": await RemoveAsync();               break;
                case "0": running = false;                   break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var details = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Reservation Details ---");
        foreach (var d in details) PrintDetail(d);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter reservation detail ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var detail = await _service.GetByIdAsync(id);
        if (detail is null) Console.WriteLine($"Reservation detail with ID {id} not found.");
        else                PrintDetail(detail);
    }

    private async Task ListByReservationAsync()
    {
        Console.Write("Enter Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        { Console.WriteLine("Invalid ID."); return; }

        var details = await _service.GetByReservationAsync(reservationId);
        Console.WriteLine($"\n--- Details for Reservation {reservationId} ---");
        foreach (var d in details) PrintDetail(d);
    }

    private async Task AddPassengerAsync()
    {
        Console.Write("Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Passenger ID: ");
        if (!int.TryParse(Console.ReadLine(), out int passengerId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Flight Seat ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightSeatId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Fare Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int fareTypeId))
        { Console.WriteLine("Invalid ID."); return; }

        var created = await _service.CreateAsync(
            reservationId, passengerId, flightSeatId, fareTypeId);

        Console.WriteLine(
            $"Detail added: [{created.Id}] Passenger {created.PassengerId} → " +
            $"Seat {created.FlightSeatId} | Fare: {created.FareTypeId}");
    }

    private async Task ChangeFareTypeAsync()
    {
        Console.Write("Reservation detail ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Fare Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int fareTypeId))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.ChangeFareTypeAsync(id, fareTypeId);
        Console.WriteLine("Fare type updated successfully.");
    }

    private async Task RemoveAsync()
    {
        Console.Write("Reservation detail ID to remove: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Reservation detail removed successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintDetail(ReservationDetailDto d)
        => Console.WriteLine(
            $"  [{d.Id}] Reservation: {d.ReservationId} | " +
            $"Passenger: {d.PassengerId} | Seat: {d.FlightSeatId} | " +
            $"FareType: {d.FareTypeId} | Created: {d.CreatedAt:yyyy-MM-dd HH:mm}");
}
