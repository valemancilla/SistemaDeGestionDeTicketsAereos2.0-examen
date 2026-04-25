using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Repositories;

public sealed class BoardingPassRepository : IBoardingPassRepository
{
    private readonly AppDbContext _dbContext;
    public BoardingPassRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<BoardingPass?> GetByIdAsync(BoardingPassId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BoardingPassEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdBoardingPass == id.Value, ct);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<BoardingPass?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var key = (code ?? string.Empty).Trim().ToUpperInvariant();
        if (key.Length == 0) return null;
        var entity = await _dbContext.Set<BoardingPassEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == key, ct);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<BoardingPass?> GetByTicketIdAsync(int idTicket, CancellationToken ct = default)
    {
        if (idTicket <= 0) return null;
        var entity = await _dbContext.Set<BoardingPassEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTicket == idTicket, ct);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<BoardingPass>> ListAsync(CancellationToken ct = default)
    {
        var entities = await _dbContext.Set<BoardingPassEntity>()
            .AsNoTracking()
            .OrderBy(x => x.IdBoardingPass)
            .ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(BoardingPass boardingPass, CancellationToken ct = default)
    {
        await _dbContext.Set<BoardingPassEntity>().AddAsync(ToEntity(boardingPass), ct);
    }

    public async Task UpdateAsync(BoardingPass boardingPass, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BoardingPassEntity>()
            .FirstOrDefaultAsync(x => x.IdBoardingPass == boardingPass.Id.Value, ct);
        if (entity is null)
            throw new KeyNotFoundException($"BoardingPass with id '{boardingPass.Id.Value}' was not found.");
        var values = ToEntity(boardingPass);
        entity.Code = values.Code;
        entity.IdTicket = values.IdTicket;
        entity.IdSeat = values.IdSeat;
        entity.Gate = values.Gate;
        entity.BoardingTime = values.BoardingTime;
        entity.CreatedAt = values.CreatedAt;
        entity.IdStatus = values.IdStatus;
    }

    public async Task DeleteAsync(BoardingPassId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<BoardingPassEntity>().FirstOrDefaultAsync(x => x.IdBoardingPass == id.Value, ct);
        if (entity is null) return;
        _dbContext.Set<BoardingPassEntity>().Remove(entity);
    }

    private static BoardingPass ToDomain(BoardingPassEntity e) =>
        BoardingPass.Create(
            e.IdBoardingPass,
            e.Code,
            e.IdTicket,
            e.IdSeat,
            e.Gate,
            e.BoardingTime,
            e.CreatedAt,
            e.IdStatus,
            e.PassengerFullName);

    private static BoardingPassEntity ToEntity(BoardingPass a) => new()
    {
        IdBoardingPass = a.Id.Value,
        Code = a.Code.Value,
        IdTicket = a.IdTicket,
        IdSeat = a.IdSeat,
        Gate = a.Gate.Value,
        BoardingTime = a.BoardingTime,
        CreatedAt = a.CreatedAt,
        IdStatus = a.IdStatus,
        PassengerFullName = a.PassengerFullName
    };
}

