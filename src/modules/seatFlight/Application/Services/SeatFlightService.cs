using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.Services;

public sealed class SeatFlightService : ISeatFlightService
{
    private readonly ISeatFlightRepository _seatFlightRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SeatFlightService(ISeatFlightRepository seatFlightRepository, IUnitOfWork unitOfWork)
    {
        _seatFlightRepository = seatFlightRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SeatFlight> CreateAsync(int idSeat, int idFlight, bool available, CancellationToken cancellationToken = default)
    {
        var existing = await _seatFlightRepository.GetBySeatAndFlightAsync(idSeat, idFlight, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"SeatFlight for seat '{idSeat}' and flight '{idFlight}' already exists.");

        var entity = SeatFlight.CreateNew(idSeat, idFlight, available);
        await _seatFlightRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<SeatFlight?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _seatFlightRepository.GetByIdAsync(SeatFlightId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<SeatFlight>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _seatFlightRepository.ListAsync(cancellationToken);
    }

    public async Task<SeatFlight> UpdateAsync(int id, int idSeat, int idFlight, bool available, CancellationToken cancellationToken = default)
    {
        var seatFlightId = SeatFlightId.Create(id);
        var existing = await _seatFlightRepository.GetByIdAsync(seatFlightId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"SeatFlight with id '{id}' was not found.");

        var updated = SeatFlight.Create(id, idSeat, idFlight, available);
        await _seatFlightRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var seatFlightId = SeatFlightId.Create(id);
        var existing = await _seatFlightRepository.GetByIdAsync(seatFlightId, cancellationToken);
        if (existing is null)
            return false;

        await _seatFlightRepository.DeleteAsync(seatFlightId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
