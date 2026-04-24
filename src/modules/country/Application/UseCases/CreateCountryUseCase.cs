// Caso de uso: registrar un nuevo país verificando que el código ISO no esté duplicado
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;

public sealed class CreateCountryUseCase
{
    private readonly ICountryRepository _repo;
    public CreateCountryUseCase(ICountryRepository repo) => _repo = repo;

    // El código ISO (ej. "AR", "US") debe ser único — dos países no pueden compartirlo
    public async Task<Country> ExecuteAsync(string name, string isoCode, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIsoCodeAsync(isoCode, ct);
        if (existing is not null) throw new InvalidOperationException($"Country with ISO code '{isoCode}' already exists.");
        var entity = Country.CreateNew(name, isoCode);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
