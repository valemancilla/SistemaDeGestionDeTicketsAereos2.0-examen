using SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.Services;

public sealed class SeatService : ISeatService
{
    private readonly ISeatRepository _seatRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SeatService(ISeatRepository seatRepository, IUnitOfWork unitOfWork)
    {
        _seatRepository = seatRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Seat> CreateAsync(string number, int idAircraft, int idClase, CancellationToken cancellationToken = default)
    {
        var entity = Seat.CreateNew(number, idAircraft, idClase);
        await _seatRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Seat?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _seatRepository.GetByIdAsync(SeatId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Seat>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _seatRepository.ListAsync(cancellationToken);
    }

    public async Task<Seat> UpdateAsync(int id, string number, int idAircraft, int idClase, CancellationToken cancellationToken = default)
    {
        var seatId = SeatId.Create(id);
        var existing = await _seatRepository.GetByIdAsync(seatId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Seat with id '{id}' was not found.");

        var updated = Seat.Create(id, number, idAircraft, idClase);
        await _seatRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var seatId = SeatId.Create(id);
        var existing = await _seatRepository.GetByIdAsync(seatId, cancellationToken);
        if (existing is null)
            return false;

        await _seatRepository.DeleteAsync(seatId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
