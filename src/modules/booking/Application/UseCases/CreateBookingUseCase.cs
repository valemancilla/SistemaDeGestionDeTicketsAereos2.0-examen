// Caso de uso: crear una reserva nueva verificando que el código no exista previamente
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;

public sealed class CreateBookingUseCase
{
    private readonly IBookingRepository _repo;

    public CreateBookingUseCase(IBookingRepository repo) => _repo = repo;

    // Verifica unicidad del código antes de persistir — si ya existe lanza excepción
    public async Task<Booking> ExecuteAsync(string code, DateTime flightDate, DateOnly creationDate, int seatCount, string? observations, int idFlight, int idStatus, CancellationToken ct = default)
    {
        // Misma normalización que BookingCode (mayúsculas) para unicidad y búsqueda por código tras guardar
        var normalizedCode = code.Trim().ToUpperInvariant();
        var existing = await _repo.GetByCodeAsync(normalizedCode, ct);
        if (existing is not null) throw new InvalidOperationException($"Booking with code '{normalizedCode}' already exists.");
        var entity = Booking.CreateNew(normalizedCode, flightDate, creationDate, seatCount, observations, idFlight, idStatus);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
