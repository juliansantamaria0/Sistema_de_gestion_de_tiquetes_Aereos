using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;

public static class QASeeder
{
    public static async Task SeedTestData(IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<AppDbContext>();

        // 1. Register User profe_test/1234
        var authService = new AuthService(db);
        var registerResult = await authService.RegisterAsync("profe_test", "1234", "Profe", "Test", 1, "12345678", null, null);
        if (!registerResult.IsSuccess)
        {
            Console.WriteLine("QASeeder: User already exists or failed to register: " + registerResult.ErrorMessage);
        }
        else
        {
            Console.WriteLine("QASeeder: Registered user profe_test");
        }

        var authLogin = await authService.LoginAsync("profe_test", "1234");
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == "profe_test");
        var person = await db.Persons.FirstOrDefaultAsync(p => p.Id == user.PersonId);

        // Get customer & passenger
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.PersonId == person.Id);
        if (customer == null)
        {
            customer = new CustomerEntity { PersonId = person.Id, CreatedAt = DateTime.UtcNow };
            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        }

        var passenger = await db.Passengers.FirstOrDefaultAsync(p => p.PersonId == person.Id);
        if (passenger == null)
        {
            passenger = new PassengerEntity { PersonId = person.Id, NationalityId = 1, CreatedAt = DateTime.UtcNow };
            db.Passengers.Add(passenger);
            await db.SaveChangesAsync();
        }

        // 2. Book an available flight
        var flight = await db.ScheduledFlights
            .Where(f => f.DepartureDate > DateOnly.FromDateTime(DateTime.UtcNow))
            .FirstOrDefaultAsync();

        if (flight == null)
        {
            Console.WriteLine("QASeeder: No scheduled flights found.");
            return;
        }

        var reservationStatus = await db.ReservationStatuses.FirstOrDefaultAsync(s => s.Name == "CONFIRMED" || s.Name == "CONFIRMADA");
        if (reservationStatus == null)
        {
             Console.WriteLine("QASeeder: No reservation status CONFIRMED.");
             return;
        }

        var reservation = await db.Reservations.FirstOrDefaultAsync(r => r.CustomerId == customer.Id && r.ScheduledFlightId == flight.Id);
        if (reservation == null)
        {
            reservation = new ReservationEntity
            {
                CustomerId = customer.Id,
                ScheduledFlightId = flight.Id,
                ReservationDate = DateTime.UtcNow,
                ReservationStatusId = reservationStatus.Id,
                CreatedAt = DateTime.UtcNow
            };
            db.Reservations.Add(reservation);
            await db.SaveChangesAsync();
        }

        var reservationDetail = await db.ReservationDetails.FirstOrDefaultAsync(d => d.ReservationId == reservation.Id);
        if (reservationDetail == null)
        {
            reservationDetail = new ReservationDetailEntity
            {
                ReservationId = reservation.Id,
                PassengerId = passenger.Id,
                FareTypeId = 1,
                FlightSeatId = 1,
                CreatedAt = DateTime.UtcNow
            };
            db.ReservationDetails.Add(reservationDetail);
            await db.SaveChangesAsync();
        }

        // 3. Make ticket payment & set state to Pendiente por Check-in
        var ticketStatus = await db.TicketStatuses.FirstOrDefaultAsync(s => s.Name == "ISSUED" || s.Name == "PENDING_CHECKIN" || s.Name == "PENDIENTE");
        if (ticketStatus == null)
        {
             Console.WriteLine("QASeeder: No ticket status found.");
             return;
        }

        var checkInStatus = await db.CheckInStatuses.FirstOrDefaultAsync(s => s.Name == "PENDING" || s.Name == "PENDIENTE");
        
        var paymentStatus = await db.PaymentStatuses.FirstOrDefaultAsync(s => s.Name == "COMPLETED" || s.Name == "PAGADO" || s.Name == "CONFIRMED");
        if (paymentStatus == null)
        {
             Console.WriteLine("QASeeder: No payment status found.");
             return;
        }

        var paymentMethod = await db.PaymentMethods.FirstOrDefaultAsync();
        var currency = await db.Currencies.FirstOrDefaultAsync();

        var payment = await db.Payments.FirstOrDefaultAsync(p => p.ReservationId == reservation.Id);
        if (payment == null)
        {
            payment = new PaymentEntity
            {
                ReservationId = reservation.Id,
                Amount = 100,
                PaymentDate = DateTime.UtcNow,
                PaymentMethodId = paymentMethod.Id,
                PaymentStatusId = paymentStatus.Id,
                CurrencyId = currency.Id,
                CreatedAt = DateTime.UtcNow
            };
            db.Payments.Add(payment);
            await db.SaveChangesAsync();
        }

        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.ReservationDetailId == reservationDetail.Id);
        if (ticket == null)
        {
            ticket = new TicketEntity
            {
                ReservationDetailId = reservationDetail.Id,
                TicketCode = "TKT-" + new Random().Next(1000, 9999).ToString(),
                IssueDate = DateTime.UtcNow,
                TicketStatusId = ticketStatus.Id,
                CreatedAt = DateTime.UtcNow
            };
            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();
        }
        else
        {
            ticket.TicketStatusId = ticketStatus.Id;
            await db.SaveChangesAsync();
        }
        
        Console.WriteLine($"QASeeder: Setup complete. Profe_test registered, reservation id: {reservation.Id}, ticket id: {ticket.Id}");
    }
}
