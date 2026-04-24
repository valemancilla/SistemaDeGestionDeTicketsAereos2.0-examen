using SistemaDeGestionDeTicketsAereos.src.modules.route.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Application.Services;

public sealed class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RouteService(IRouteRepository routeRepository, IUnitOfWork unitOfWork)
    {
        _routeRepository = routeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Route> CreateAsync(decimal distanceKm, TimeOnly estDuration, int originAirport, int destinationAirport, bool active, CancellationToken cancellationToken = default)
    {
        var entity = Route.CreateNew(distanceKm, estDuration, originAirport, destinationAirport, active);
        await _routeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Route?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _routeRepository.GetByIdAsync(RouteId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Route>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _routeRepository.ListAsync(cancellationToken);
    }

    public async Task<Route> UpdateAsync(int id, decimal distanceKm, TimeOnly estDuration, int originAirport, int destinationAirport, bool active, CancellationToken cancellationToken = default)
    {
        var routeId = RouteId.Create(id);
        var existing = await _routeRepository.GetByIdAsync(routeId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Route with id '{id}' was not found.");

        var updated = Route.Create(id, distanceKm, estDuration, originAirport, destinationAirport, active);
        await _routeRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var routeId = RouteId.Create(id);
        var existing = await _routeRepository.GetByIdAsync(routeId, cancellationToken);
        if (existing is null)
            return false;

        await _routeRepository.DeleteAsync(routeId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
