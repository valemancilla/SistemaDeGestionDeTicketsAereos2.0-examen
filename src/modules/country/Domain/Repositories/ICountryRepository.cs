// Contrato del repositorio de países: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de países
public interface ICountryRepository
{
    // Busca un país por su ID
    Task<Country?> GetByIdAsync(CountryId id, CancellationToken ct = default);

    // Busca un país por su código ISO (ej: "CO", "US") — útil para validar duplicados
    Task<Country?> GetByIsoCodeAsync(string isoCode, CancellationToken ct = default);

    // Retorna todos los países registrados en el sistema
    Task<IReadOnlyList<Country>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo país al sistema
    Task AddAsync(Country country, CancellationToken ct = default);

    // Actualiza los datos de un país existente
    Task UpdateAsync(Country country, CancellationToken ct = default);

    // Elimina un país del sistema por su ID
    Task DeleteAsync(CountryId id, CancellationToken ct = default);
}
