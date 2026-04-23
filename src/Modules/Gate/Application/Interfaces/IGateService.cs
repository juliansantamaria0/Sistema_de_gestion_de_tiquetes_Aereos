namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;


public interface IGateService
{
    Task<GateDto> CreateAsync(CreateGateRequest request, CancellationToken cancellationToken = default);
    Task<GateDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GateDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GateDto> UpdateAsync(int id, UpdateGateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record GateDto(int GateId, int TerminalId, string Code, bool IsActive);
public sealed record CreateGateRequest(int TerminalId, string Code, bool IsActive = true);
public sealed record UpdateGateRequest(int TerminalId, string Code, bool IsActive);
