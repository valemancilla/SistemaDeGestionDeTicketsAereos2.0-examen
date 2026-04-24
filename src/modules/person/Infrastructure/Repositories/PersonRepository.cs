using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;

public sealed class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _dbContext;

    public PersonRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Person?> GetByIdAsync(PersonId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PersonEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPerson == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Person?> GetByDocumentAsync(int idDocumentType, string documentNumber, CancellationToken ct = default)
    {
        var norm = PersonDocumentNumber.Create(documentNumber).Value;
        var entity = await _dbContext.Set<PersonEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdDocumentType == idDocumentType && x.DocumentNumber == norm, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Person?> GetByDocumentNumberAsync(string normalizedDocumentNumber, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PersonEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.DocumentNumber == normalizedDocumentNumber, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Person>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<PersonEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdPerson).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Person person, CancellationToken ct = default)
    {
        var entity = ToEntity(person);
        await _dbContext.Set<PersonEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Person person, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PersonEntity>().FirstOrDefaultAsync(x => x.IdPerson == person.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Person was not found.");
        }

        var values = ToEntity(person);
        entity.DocumentNumber = values.DocumentNumber;
        entity.FirstName = values.FirstName;
        entity.LastName = values.LastName;
        entity.BirthDate = values.BirthDate;
        entity.IdDocumentType = values.IdDocumentType;
        entity.IdGender = values.IdGender;
        entity.IdCountry = values.IdCountry;
        entity.IdAddress = values.IdAddress;
    }

    public async Task DeleteAsync(PersonId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PersonEntity>().FirstOrDefaultAsync(x => x.IdPerson == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        var idPerson = id.Value;

        // 0) Romper el FK opcional a la "dirección actual" (IdAddress) antes de borrar direcciones (Restrict).
        entity.IdAddress = null;

        // 1) Datos de contacto del cliente
        var emails = await _dbContext.Set<CustomerEmailEntity>()
            .Where(x => x.IdPerson == idPerson)
            .ToListAsync(ct);
        if (emails.Count > 0) _dbContext.Set<CustomerEmailEntity>().RemoveRange(emails);

        var phones = await _dbContext.Set<CustomerPhoneEntity>()
            .Where(x => x.IdPerson == idPerson)
            .ToListAsync(ct);
        if (phones.Count > 0) _dbContext.Set<CustomerPhoneEntity>().RemoveRange(phones);

        // 2) Direcciones (PersonAddress)
        var addresses = await _dbContext.Set<PersonAddressEntity>()
            .Where(x => x.IdPerson == idPerson)
            .ToListAsync(ct);
        if (addresses.Count > 0) _dbContext.Set<PersonAddressEntity>().RemoveRange(addresses);

        // 3) Pasajeros en reservas (BookingCustomer) — depende de Person (Restrict).
        var bookingCustomers = await _dbContext.Set<BookingCustomerEntity>()
            .Where(x => x.IdPerson == idPerson)
            .ToListAsync(ct);
        if (bookingCustomers.Count > 0) _dbContext.Set<BookingCustomerEntity>().RemoveRange(bookingCustomers);

        // 4) Usuarios del sistema vinculados a esta persona (User -> Person Restrict).
        var users = await _dbContext.Set<UserEntity>()
            .Where(x => x.IdPerson == idPerson)
            .ToListAsync(ct);
        if (users.Count > 0) _dbContext.Set<UserEntity>().RemoveRange(users);

        // 5) Empleados vinculados a esta persona (Employee -> Person Restrict),
        // y antes hay que limpiar CrewMember (CrewMember -> Employee Restrict).
        var employeeIds = await _dbContext.Set<EmployeeEntity>()
            .AsNoTracking()
            .Where(e => e.IdPerson == idPerson)
            .Select(e => e.IdEmployee)
            .ToListAsync(ct);
        if (employeeIds.Count > 0)
        {
            var crewMembers = await _dbContext.Set<CrewMemberEntity>()
                .Where(cm => employeeIds.Contains(cm.IdEmployee))
                .ToListAsync(ct);
            if (crewMembers.Count > 0) _dbContext.Set<CrewMemberEntity>().RemoveRange(crewMembers);

            var employees = await _dbContext.Set<EmployeeEntity>()
                .Where(e => employeeIds.Contains(e.IdEmployee))
                .ToListAsync(ct);
            if (employees.Count > 0) _dbContext.Set<EmployeeEntity>().RemoveRange(employees);
        }

        // 6) Si existe Customer (1 a 1), eliminarlo antes de la persona (Restrict).
        var customer = await _dbContext.Set<CustomerEntity>()
            .FirstOrDefaultAsync(c => c.IdPerson == idPerson, ct);
        if (customer is not null) _dbContext.Set<CustomerEntity>().Remove(customer);

        _dbContext.Set<PersonEntity>().Remove(entity);
    }

    private static Person ToDomain(PersonEntity entity)
    {
        return Person.Create(entity.IdPerson, entity.FirstName, entity.LastName, entity.BirthDate, entity.DocumentNumber, entity.IdDocumentType, entity.IdGender, entity.IdCountry, entity.IdAddress);
    }

    private static PersonEntity ToEntity(Person aggregate)
    {
        return new PersonEntity
        {
            IdPerson = aggregate.Id.Value,
            IdDocumentType = aggregate.IdDocumentType,
            DocumentNumber = aggregate.DocumentNumber.Value,
            FirstName = aggregate.FirstName.Value,
            LastName = aggregate.LastName.Value,
            BirthDate = aggregate.BirthDate.Value,
            IdGender = aggregate.IdGender,
            IdCountry = aggregate.IdCountry,
            IdAddress = aggregate.IdAddress
        };
    }
}
