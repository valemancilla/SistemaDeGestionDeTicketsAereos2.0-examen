// Contrato del repositorio de ciudades: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de ciudades
public interface ICityRepository
{
    // Busca una ciudad por su ID
    Task<City?> GetByIdAsync(CityId id, CancellationToken ct = default);

    // Retorna todas las ciudades del sistema
    Task<IReadOnlyList<City>> ListAsync(CancellationToken ct = default);

    // Retorna todas las ciudades que pertenecen a un país específico
    Task<IReadOnlyList<City>> ListByCountryAsync(int idCountry, CancellationToken ct = default);

    // Agrega una nueva ciudad al sistema
    Task AddAsync(City city, CancellationToken ct = default);

    // Actualiza los datos de una ciudad existente
    Task UpdateAsync(City city, CancellationToken ct = default);

    // Elimina una ciudad del sistema por su ID
    Task DeleteAsync(CityId id, CancellationToken ct = default);
}
