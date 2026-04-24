// Caso de uso: registrar una nueva aerolínea verificando que el código IATA no esté duplicado
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;

public sealed class CreateAerolineUseCase
{
    private readonly IAirlineRepository _repo;

    public CreateAerolineUseCase(IAirlineRepository repo) => _repo = repo;

    // El código IATA identifica a la aerolínea mundialmente — debe ser único en el sistema
    public async Task<Aeroline> ExecuteAsync(string name, string iataCode, int idCountry, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIataCodeAsync(iataCode, ct);
        if (existing is not null) throw new InvalidOperationException($"Aeroline with IATA code '{iataCode}' already exists.");
        var entity = Aeroline.CreateNew(name, iataCode, idCountry, active);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
