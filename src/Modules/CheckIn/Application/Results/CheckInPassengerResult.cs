namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.Results;

public sealed record CheckInPassengerResult(
    int      CheckInId,
    int      TicketId,
    int      FlightSeatId,
    int      BoardingPassId,
    string   BoardingPassCode,
    int?     GateId,
    string?  BoardingGroup,
    DateTime IssuedAt);
