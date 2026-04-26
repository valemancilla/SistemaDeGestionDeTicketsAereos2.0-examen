// Implementación del servicio de aerolíneas: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.Services;

// Servicio de aplicación de aerolíneas — orquesta los casos de uso sin lógica de dominio propia
public sealed class AerolineService : IAerolineService
{
    private readonly IAirlineRepository _airlineRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public AerolineService(IAirlineRepository airlineRepository, IUnitOfWork unitOfWork)
    {
        _airlineRepository = airlineRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea una aerolínea verificando que el código IATA no esté duplicado — es único a nivel mundial
    public async Task<Aeroline> CreateAsync(string name, string iataCode, int idCountry, bool active, CancellationToken cancellationToken = default)
    {
        var existingByName = await _airlineRepository.GetByNameAsync(name, cancellationToken);
        if (existingByName is not null)
            throw new InvalidOperationException($"Aeroline with name '{name}' already exists.");

        var existingByIata = await _airlineRepository.GetByIataCodeAsync(iataCode, cancellationToken);
        if (existingByIata is not null)
            throw new InvalidOperationException($"Aeroline with IATA code '{iataCode}' already exists.");

        var entity = Aeroline.CreateNew(name, iataCode, idCountry, active);
        await _airlineRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca una aerolínea por ID delegando directamente al repositorio
    public Task<Aeroline?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _airlineRepository.GetByIdAsync(AirlineId.Create(id), cancellationToken);
    }

    // Retorna todas las aerolíneas sin filtro
    public async Task<IReadOnlyCollection<Aeroline>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _airlineRepository.ListAsync(cancellationToken);
    }

    // Actualiza una aerolínea verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Aeroline> UpdateAsync(int id, string name, string iataCode, int idCountry, bool active, CancellationToken cancellationToken = default)
    {
        var airlineId = AirlineId.Create(id);
        var existing = await _airlineRepository.GetByIdAsync(airlineId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Aeroline with id '{id}' was not found.");

        var existingByName = await _airlineRepository.GetByNameAsync(name, cancellationToken);
        if (existingByName is not null && existingByName.Id.Value != id)
            throw new InvalidOperationException($"Aeroline with name '{name}' already exists.");

        var existingByIata = await _airlineRepository.GetByIataCodeAsync(iataCode, cancellationToken);
        if (existingByIata is not null && existingByIata.Id.Value != id)
            throw new InvalidOperationException($"Aeroline with IATA code '{iataCode}' already exists.");

        var updated = Aeroline.Create(id, name, iataCode, idCountry, active);
        await _airlineRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina una aerolínea por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var airlineId = AirlineId.Create(id);
        var existing = await _airlineRepository.GetByIdAsync(airlineId, cancellationToken);
        if (existing is null)
            return false;

        await _airlineRepository.DeleteAsync(airlineId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
