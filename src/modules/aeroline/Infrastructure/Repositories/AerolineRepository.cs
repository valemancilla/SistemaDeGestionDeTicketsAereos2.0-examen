using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Repositories;

public sealed class AerolineRepository : IAirlineRepository
{
    private readonly AppDbContext _dbContext;

    public AerolineRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Aeroline aeroline, CancellationToken ct = default)
    {
        var entity = ToEntity(aeroline);
        await _dbContext.Set<AerolineEntity>().AddAsync(entity, ct);
    }

    public async Task<Aeroline?> GetByIdAsync(AirlineId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AerolineEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdAirline == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Aeroline?> GetByIataCodeAsync(string iataCode, CancellationToken ct = default)
    {
        var normalizedIataCode = iataCode.Trim().ToUpperInvariant();

        var entity = await _dbContext.Set<AerolineEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IATACode == normalizedIataCode, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Aeroline>> ListAsync(CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<AerolineEntity>()
            .AsNoTracking()
            .OrderBy(x => x.IdAirline)
            .ToListAsync(ct);

        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Aeroline>> ListActiveAsync(CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<AerolineEntity>()
            .AsNoTracking()
            .Where(x => x.Active)
            .OrderBy(x => x.IdAirline)
            .ToListAsync(ct);

        return entities.Select(ToDomain).ToList();
    }

    public async Task UpdateAsync(Aeroline aeroline, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AerolineEntity>()
            .FirstOrDefaultAsync(x => x.IdAirline == aeroline.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException($"Aeroline with id '{aeroline.Id.Value}' was not found.");
        }

        entity.Name = aeroline.Name.Value;
        entity.IATACode = aeroline.IATACode.Value;
        entity.IdCountry = aeroline.IdCountry;
        entity.Active = aeroline.Active;
    }

    public async Task DeleteAsync(AirlineId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AerolineEntity>()
            .FirstOrDefaultAsync(x => x.IdAirline == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        // Si existen empleados de esta aerolínea, hay que eliminarlos primero (FK RESTRICT).
        // Antes, se deben eliminar sus registros en CrewMember (también FK RESTRICT).
        var employeeIds = await _dbContext.Set<EmployeeEntity>()
            .AsNoTracking()
            .Where(e => e.IdAirline == id.Value)
            .Select(e => e.IdEmployee)
            .ToListAsync(ct);

        if (employeeIds.Count > 0)
        {
            var crewMembers = await _dbContext.Set<CrewMemberEntity>()
                .Where(cm => employeeIds.Contains(cm.IdEmployee))
                .ToListAsync(ct);
            if (crewMembers.Count > 0)
                _dbContext.Set<CrewMemberEntity>().RemoveRange(crewMembers);

            var employees = await _dbContext.Set<EmployeeEntity>()
                .Where(e => employeeIds.Contains(e.IdEmployee))
                .ToListAsync(ct);
            if (employees.Count > 0)
                _dbContext.Set<EmployeeEntity>().RemoveRange(employees);
        }

        // Si existen aeronaves de esta aerolínea, hay que eliminarlas primero (FK RESTRICT).
        // AircraftRepository elimina además vuelos, reservas, asientos, etc.
        var aircraftIds = await _dbContext.Set<AircraftEntity>()
            .AsNoTracking()
            .Where(a => a.IdAirline == id.Value)
            .Select(a => a.IdAircraft)
            .ToListAsync(ct);

        if (aircraftIds.Count > 0)
        {
            var aircraftRepo = new AircraftRepository(_dbContext);
            foreach (var aircraftId in aircraftIds)
            {
                await aircraftRepo.DeleteAsync(AircraftId.Create(aircraftId), ct);
            }
        }

        // Si existen tarifas de esta aerolínea, hay que eliminarlas primero (FK RESTRICT).
        // Y si hubiera tickets que aún apunten a esas tarifas, se deben eliminar antes (Ticket -> Fare también es RESTRICT).
        var fareIds = await _dbContext.Set<FareEntity>()
            .AsNoTracking()
            .Where(f => f.IdAirline == id.Value)
            .Select(f => f.IdFare)
            .ToListAsync(ct);

        if (fareIds.Count > 0)
        {
            var ticketIds = await _dbContext.Set<TicketEntity>()
                .AsNoTracking()
                .Where(t => fareIds.Contains(t.IdFare))
                .Select(t => t.IdTicket)
                .ToListAsync(ct);

            if (ticketIds.Count > 0)
            {
                var baggages = await _dbContext.Set<BaggageEntity>()
                    .Where(b => ticketIds.Contains(b.IdTicket))
                    .ToListAsync(ct);
                if (baggages.Count > 0) _dbContext.Set<BaggageEntity>().RemoveRange(baggages);

                var checkIns = await _dbContext.Set<CheckInEntity>()
                    .Where(c => ticketIds.Contains(c.IdTicket))
                    .ToListAsync(ct);
                if (checkIns.Count > 0) _dbContext.Set<CheckInEntity>().RemoveRange(checkIns);

                var ticketHist = await _dbContext.Set<TicketStatusHistoryEntity>()
                    .Where(h => ticketIds.Contains(h.IdTicket))
                    .ToListAsync(ct);
                if (ticketHist.Count > 0) _dbContext.Set<TicketStatusHistoryEntity>().RemoveRange(ticketHist);

                var tickets = await _dbContext.Set<TicketEntity>()
                    .Where(t => ticketIds.Contains(t.IdTicket))
                    .ToListAsync(ct);
                if (tickets.Count > 0) _dbContext.Set<TicketEntity>().RemoveRange(tickets);
            }

            var seatClassPrices = await _dbContext.Set<FareSeatClassPriceEntity>()
                .Where(p => fareIds.Contains(p.IdFare))
                .ToListAsync(ct);
            if (seatClassPrices.Count > 0) _dbContext.Set<FareSeatClassPriceEntity>().RemoveRange(seatClassPrices);

            var fares = await _dbContext.Set<FareEntity>()
                .Where(f => f.IdAirline == id.Value)
                .ToListAsync(ct);
            if (fares.Count > 0) _dbContext.Set<FareEntity>().RemoveRange(fares);
        }

        _dbContext.Set<AerolineEntity>().Remove(entity);
    }

    private static Aeroline ToDomain(AerolineEntity entity)
    {
        return Aeroline.Create(entity.IdAirline, entity.Name, entity.IATACode, entity.IdCountry, entity.Active);
    }

    private static AerolineEntity ToEntity(Aeroline aeroline)
    {
        return new AerolineEntity
        {
            IdAirline = aeroline.Id.Value,
            Name = aeroline.Name.Value,
            IATACode = aeroline.IATACode.Value,
            IdCountry = aeroline.IdCountry,
            Active = aeroline.Active
        };
    }
}
