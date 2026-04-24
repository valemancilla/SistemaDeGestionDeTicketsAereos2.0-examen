using SistemaDeGestionDeTicketsAereos.src.modules.role.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Application.Services;

public sealed class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Role> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = Role.CreateNew(name);
        await _roleRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _roleRepository.GetByIdAsync(RoleId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _roleRepository.ListAsync(cancellationToken);
    }

    public async Task<Role> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var roleId = RoleId.Create(id);
        var existing = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Role with id '{id}' was not found.");

        var updated = Role.Create(id, name);
        await _roleRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var roleId = RoleId.Create(id);
        var existing = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (existing is null)
            return false;

        await _roleRepository.DeleteAsync(roleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
