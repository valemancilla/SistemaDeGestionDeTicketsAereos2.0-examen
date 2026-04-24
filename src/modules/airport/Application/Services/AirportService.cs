// Implementación del servicio de aeropuertos: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Application.Services;

// Servicio de aplicación de aeropuertos — orquesta los casos de uso sin lógica de dominio propia
public sealed class AirportService : IAirportService
{
    private readonly IAirportRepository _airportRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public AirportService(IAirportRepository airportRepository, IUnitOfWork unitOfWork)
    {
        _airportRepository = airportRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea un aeropuerto verificando que el código IATA no esté duplicado — es único a nivel mundial
    public async Task<Airport> CreateAsync(string name, string iataCode, int idCity, bool active, CancellationToken cancellationToken = default)
    {
        var existing = await _airportRepository.GetByIataCodeAsync(iataCode, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Airport with IATA code '{iataCode}' already exists.");

        var entity = Airport.CreateNew(name, iataCode, idCity, active);
        await _airportRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un aeropuerto por ID delegando directamente al repositorio
    public Task<Airport?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _airportRepository.GetByIdAsync(AirportId.Create(id), cancellationToken);
    }

    // Retorna todos los aeropuertos sin filtro
    public async Task<IReadOnlyCollection<Airport>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _airportRepository.ListAsync(cancellationToken);
    }

    // Actualiza un aeropuerto verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Airport> UpdateAsync(int id, string name, string iataCode, int idCity, bool active, CancellationToken cancellationToken = default)
    {
        var airportId = AirportId.Create(id);
        var existing = await _airportRepository.GetByIdAsync(airportId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Airport with id '{id}' was not found.");

        var updated = Airport.Create(id, name, iataCode, idCity, active);
        await _airportRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un aeropuerto por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var airportId = AirportId.Create(id);
        var existing = await _airportRepository.GetByIdAsync(airportId, cancellationToken);
        if (existing is null)
            return false;

        await _airportRepository.DeleteAsync(airportId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
