using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.Services;

public sealed class BoardingPassService : IBoardingPassService
{
    private readonly IBoardingPassRepository _repo;
    private readonly IUnitOfWork _uow;

    public BoardingPassService(IBoardingPassRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<BoardingPass> CreateAsync(
        string code,
        int idTicket,
        int idSeat,
        string gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName,
        CancellationToken cancellationToken = default)
    {
        var pass = BoardingPass.CreateNew(code, idTicket, idSeat, gate, boardingTime, createdAt, idStatus, passengerFullName);
        await _repo.AddAsync(pass, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return pass;
    }

    public Task<BoardingPass?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _repo.GetByIdAsync(BoardingPassId.Create(id), cancellationToken);

    public Task<BoardingPass?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        _repo.GetByCodeAsync(code, cancellationToken);

    public Task<BoardingPass?> GetByTicketIdAsync(int idTicket, CancellationToken cancellationToken = default) =>
        _repo.GetByTicketIdAsync(idTicket, cancellationToken);

    public async Task<IReadOnlyCollection<BoardingPass>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _repo.ListAsync(cancellationToken);

    public async Task<BoardingPass> UpdateAsync(
        int id,
        string code,
        int idTicket,
        int idSeat,
        string gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName,
        CancellationToken cancellationToken = default)
    {
        var idVo = BoardingPassId.Create(id);
        var existing = await _repo.GetByIdAsync(idVo, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"No existe pase de abordar con id '{id}'.");

        var updated = BoardingPass.Create(id, code, idTicket, idSeat, gate, boardingTime, createdAt, idStatus, passengerFullName);
        await _repo.UpdateAsync(updated, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var idVo = BoardingPassId.Create(id);
        var existing = await _repo.GetByIdAsync(idVo, cancellationToken);
        if (existing is null) return false;
        await _repo.DeleteAsync(idVo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
