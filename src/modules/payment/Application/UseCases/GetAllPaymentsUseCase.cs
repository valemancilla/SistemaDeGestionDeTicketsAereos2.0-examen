using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;

public sealed class GetAllPaymentsUseCase
{
    private readonly IPaymentRepository _repo;
    public GetAllPaymentsUseCase(IPaymentRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Payment>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
