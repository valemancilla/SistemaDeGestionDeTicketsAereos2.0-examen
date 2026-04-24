// Contrato del repositorio de fabricantes de aeronaves: define las operaciones de persistencia
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de fabricantes
public interface IManufacturerRepository
{
    // Busca un fabricante por su ID
    Task<Manufacturer?> GetByIdAsync(ManufacturerId id, CancellationToken ct = default);

    // Busca un fabricante por su nombre — útil para validar duplicados
    Task<Manufacturer?> GetByNameAsync(string name, CancellationToken ct = default);

    // Retorna todos los fabricantes registrados en el sistema
    Task<IReadOnlyList<Manufacturer>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo fabricante al sistema
    Task AddAsync(Manufacturer manufacturer, CancellationToken ct = default);

    // Actualiza los datos de un fabricante existente
    Task UpdateAsync(Manufacturer manufacturer, CancellationToken ct = default);

    // Elimina un fabricante del sistema por su ID
    Task DeleteAsync(ManufacturerId id, CancellationToken ct = default);
}
