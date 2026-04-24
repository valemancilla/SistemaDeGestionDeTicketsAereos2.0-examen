// Caso de uso: eliminar un check-in por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;

public sealed class DeleteCheckInUseCase
{
    private readonly ICheckInRepository _repo;
    public DeleteCheckInUseCase(ICheckInRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CheckInId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CheckInId.Create(id), ct);
        return true;
    }
}
