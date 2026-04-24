// Implementación del servicio de zonas horarias de aeropuerto: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airportTimeZone.Application.Services;

// Servicio de aplicación de AirportTimeZone — clave compuesta, sin ID propio
public sealed class AirportTimeZoneService : IAirportTimeZoneService
{
    private readonly IAirportTimeZoneRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public AirportTimeZoneService(IAirportTimeZoneRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Crea la relación verificando que la combinación aeropuerto+zona horaria no exista ya
    public async Task<AirportTimeZone> CreateAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(idAirport, idTimeZone, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"AirportTimeZone for airport '{idAirport}' and timeZone '{idTimeZone}' already exists.");

        var entity = AirportTimeZone.Create(idAirport, idTimeZone);
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca la relación por los dos IDs delegando al repositorio
    public Task<AirportTimeZone?> GetByIdAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(idAirport, idTimeZone, cancellationToken);
    }

    // Retorna todas las relaciones sin filtro
    public async Task<IReadOnlyCollection<AirportTimeZone>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(cancellationToken);
    }

    // Actualiza la relación verificando que exista antes de persistir los cambios
    public async Task<AirportTimeZone> UpdateAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(idAirport, idTimeZone, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"AirportTimeZone for airport '{idAirport}' and timeZone '{idTimeZone}' was not found.");

        var updated = AirportTimeZone.Create(idAirport, idTimeZone);
        await _repository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina la relación por los dos IDs, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int idAirport, int idTimeZone, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(idAirport, idTimeZone, cancellationToken);
        if (existing is null)
            return false;

        await _repository.RemoveAsync(idAirport, idTimeZone, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
