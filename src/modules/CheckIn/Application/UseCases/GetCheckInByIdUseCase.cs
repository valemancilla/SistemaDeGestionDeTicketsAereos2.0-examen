// Caso de uso: buscar un check-in por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;
using CheckInClass = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;

public sealed class GetCheckInByIdUseCase
{
    private readonly ICheckInRepository _repo;
    public GetCheckInByIdUseCase(ICheckInRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<CheckInClass> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CheckInId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"CheckIn with id '{id}' was not found.");
        return entity;
    }
}
