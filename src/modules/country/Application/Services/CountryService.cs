// Implementación del servicio de países: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Application.Services;

// Servicio de aplicación de países — orquesta sin lógica de dominio propia
public sealed class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CountryService(ICountryRepository countryRepository, IUnitOfWork unitOfWork)
    {
        _countryRepository = countryRepository;
        _unitOfWork = unitOfWork;
    }

    // El código ISO identifica al país internacionalmente — debe ser único antes de persistir
    public async Task<Country> CreateAsync(string name, string isoCode, CancellationToken cancellationToken = default)
    {
        var existing = await _countryRepository.GetByIsoCodeAsync(isoCode, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Country with ISO code '{isoCode}' already exists.");

        var entity = Country.CreateNew(name, isoCode);
        await _countryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un país por ID delegando directamente al repositorio
    public Task<Country?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _countryRepository.GetByIdAsync(CountryId.Create(id), cancellationToken);
    }

    // Retorna todos los países sin filtro
    public async Task<IReadOnlyCollection<Country>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _countryRepository.ListAsync(cancellationToken);
    }

    // Actualiza un país verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Country> UpdateAsync(int id, string name, string isoCode, CancellationToken cancellationToken = default)
    {
        var countryId = CountryId.Create(id);
        var existing = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Country with id '{id}' was not found.");

        var updated = Country.Create(id, name, isoCode);
        await _countryRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un país por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var countryId = CountryId.Create(id);
        var existing = await _countryRepository.GetByIdAsync(countryId, cancellationToken);
        if (existing is null)
            return false;

        await _countryRepository.DeleteAsync(countryId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
