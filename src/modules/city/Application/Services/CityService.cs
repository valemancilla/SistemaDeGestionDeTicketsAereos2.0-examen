// Implementación del servicio de ciudades: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.city.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Application.Services;

// Servicio de aplicación de ciudades — orquesta sin lógica de dominio propia
public sealed class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CityService(ICityRepository cityRepository, IUnitOfWork unitOfWork)
    {
        _cityRepository = cityRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea la ciudad y la persiste inmediatamente
    public async Task<City> CreateAsync(string name, int idCountry, CancellationToken cancellationToken = default)
    {
        var entity = City.CreateNew(name, idCountry);
        await _cityRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca una ciudad por ID delegando directamente al repositorio
    public Task<City?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _cityRepository.GetByIdAsync(CityId.Create(id), cancellationToken);
    }

    // Retorna todas las ciudades sin filtro
    public async Task<IReadOnlyCollection<City>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _cityRepository.ListAsync(cancellationToken);
    }

    // Actualiza una ciudad verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<City> UpdateAsync(int id, string name, int idCountry, CancellationToken cancellationToken = default)
    {
        var cityId = CityId.Create(id);
        var existing = await _cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"City with id '{id}' was not found.");

        var updated = City.Create(id, name, idCountry);
        await _cityRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina una ciudad por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cityId = CityId.Create(id);
        var existing = await _cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (existing is null)
            return false;

        await _cityRepository.DeleteAsync(cityId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
