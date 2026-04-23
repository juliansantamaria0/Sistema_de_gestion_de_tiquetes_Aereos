namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;


public interface ITerminalService
{
    Task<TerminalDto> CreateAsync(CreateTerminalRequest request, CancellationToken cancellationToken = default);
    Task<TerminalDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TerminalDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TerminalDto> UpdateAsync(int id, UpdateTerminalRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record TerminalDto(int TerminalId, int AirportId, string Name, bool IsInternational, DateTime CreatedAt);
public sealed record CreateTerminalRequest(int AirportId, string Name, bool IsInternational);
public sealed record UpdateTerminalRequest(int AirportId, string Name, bool IsInternational);
