using SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.Services;

public sealed class FlightService : IFlightService
{
    private readonly IFlightRepository _flightRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FlightService(IFlightRepository flightRepository, IUnitOfWork unitOfWork)
    {
        _flightRepository = flightRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Flight> CreateAsync(string number, DateOnly date, TimeOnly departureTime, TimeOnly arrivalTime, int totalCapacity, int availableSeats, int idRoute, int idAircraft, int idStatus, int idCrew, int idFare, CancellationToken cancellationToken = default)
    {
        var existing = await _flightRepository.GetByFlightNumberAsync(number, date, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Flight '{number}' on '{date}' already exists.");

        var entity = Flight.CreateNew(number, date, departureTime, arrivalTime, totalCapacity, availableSeats, idRoute, idAircraft, idStatus, idCrew, idFare);
        await _flightRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _flightRepository.GetByIdAsync(FlightId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Flight>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _flightRepository.ListAsync(cancellationToken);
    }

    public async Task<Flight> UpdateAsync(int id, string number, DateOnly date, TimeOnly departureTime, TimeOnly arrivalTime, int totalCapacity, int availableSeats, int idRoute, int idAircraft, int idStatus, int idCrew, int idFare, CancellationToken cancellationToken = default)
    {
        var flightId = FlightId.Create(id);
        var existing = await _flightRepository.GetByIdAsync(flightId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Flight with id '{id}' was not found.");

        var updated = Flight.Create(id, number, date, departureTime, arrivalTime, totalCapacity, availableSeats, idRoute, idAircraft, idStatus, idCrew, idFare);
        await _flightRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var flightId = FlightId.Create(id);
        var existing = await _flightRepository.GetByIdAsync(flightId, cancellationToken);
        if (existing is null)
            return false;

        await _flightRepository.DeleteAsync(flightId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
