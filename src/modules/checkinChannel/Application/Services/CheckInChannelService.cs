// Implementación del servicio de canales de check-in: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.Services;

// Servicio de aplicación de canales — orquesta sin lógica de dominio propia
public sealed class CheckInChannelService : ICheckInChannelService
{
    private readonly ICheckInChannelRepository _checkInChannelRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CheckInChannelService(ICheckInChannelRepository checkInChannelRepository, IUnitOfWork unitOfWork)
    {
        _checkInChannelRepository = checkInChannelRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea el canal y lo persiste inmediatamente
    public async Task<CheckInChannel> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = CheckInChannel.CreateNew(name);
        await _checkInChannelRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un canal por ID delegando directamente al repositorio
    public Task<CheckInChannel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _checkInChannelRepository.GetByIdAsync(CheckInChannelId.Create(id), cancellationToken);
    }

    // Retorna todos los canales sin filtro
    public async Task<IReadOnlyCollection<CheckInChannel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _checkInChannelRepository.ListAsync(cancellationToken);
    }

    // Actualiza un canal verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<CheckInChannel> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var channelId = CheckInChannelId.Create(id);
        var existing = await _checkInChannelRepository.GetByIdAsync(channelId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"CheckInChannel with id '{id}' was not found.");

        var updated = CheckInChannel.Create(id, name);
        await _checkInChannelRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un canal por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var channelId = CheckInChannelId.Create(id);
        var existing = await _checkInChannelRepository.GetByIdAsync(channelId, cancellationToken);
        if (existing is null)
            return false;

        await _checkInChannelRepository.DeleteAsync(channelId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
