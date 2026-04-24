// El empleado es la persona que trabaja para una aerolínea con un rol específico dentro de ella
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;

// Agregado Employee: encapsula las reglas de negocio de un empleado de aerolínea
public class Employee
{
    // ID del empleado (Value Object)
    public EmployeeId Id { get; private set; }

    // FK a la persona asociada — el empleado es una extensión de Person
    public int IdPerson { get; private set; }

    // FK a la aerolínea para la que trabaja
    public int IdAirline { get; private set; }

    // FK al rol que desempeña dentro de la aerolínea (piloto, técnico, agente, etc.)
    public int IdRole { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Employee(EmployeeId id, int idPerson, int idAirline, int idRole)
    {
        Id = id;
        IdPerson = idPerson;
        IdAirline = idAirline;
        IdRole = idRole;
    }

    // Método de fábrica para crear o reconstruir un empleado desde la base de datos
    public static Employee Create(int id, int idPerson, int idAirline, int idRole)
    {
        // Regla: todo empleado debe estar vinculado a una persona válida
        if (idPerson <= 0)
            throw new ArgumentException("IdPerson must be greater than 0.", nameof(idPerson));

        // Regla: todo empleado debe pertenecer a una aerolínea válida
        if (idAirline <= 0)
            throw new ArgumentException("IdAirline must be greater than 0.", nameof(idAirline));

        // Regla: todo empleado debe tener un rol asignado válido
        if (idRole <= 0)
            throw new ArgumentException("IdRole must be greater than 0.", nameof(idRole));

        return new Employee(
            EmployeeId.Create(id),
            idPerson,
            idAirline,
            idRole
        );
    }

    // Método de fábrica para crear un empleado nuevo (ID = 0, la BD lo asigna después)
    public static Employee CreateNew(int idPerson, int idAirline, int idRole) => Create(0, idPerson, idAirline, idRole);
}
