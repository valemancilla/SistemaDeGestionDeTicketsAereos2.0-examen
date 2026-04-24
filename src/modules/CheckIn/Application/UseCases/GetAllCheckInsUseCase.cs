// Caso de uso: obtener todos los check-ins registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;
using CheckInClass = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;

public sealed class GetAllCheckInsUseCase
{
    private readonly ICheckInRepository _repo;
    public GetAllCheckInsUseCase(ICheckInRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<CheckInClass>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
