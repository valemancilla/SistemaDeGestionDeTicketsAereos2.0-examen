// El método de pago define cómo el cliente paga su reserva (efectivo, tarjeta, transferencia, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;

// Agregado PaymentMethod: encapsula las reglas de negocio de un método de pago del sistema
public class PaymentMethod
{
    // ID del método de pago (Value Object)
    public PaymentMethodId Id { get; private set; }

    // Nombre del método (ej: "Tarjeta de Crédito", "Efectivo", "PSE")
    public PaymentMethodName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private PaymentMethod(PaymentMethodId id, PaymentMethodName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir un método de pago desde la base de datos
    public static PaymentMethod Create(int id, string name)
    {
        // Regla: el nombre del método de pago es validado por su Value Object (no vacío)
        return new PaymentMethod(
            PaymentMethodId.Create(id),
            PaymentMethodName.Create(name)
        );
    }

    // Método de fábrica para crear un método de pago nuevo (ID = 0, la BD lo asigna después)
    public static PaymentMethod CreateNew(string name) => Create(0, name);
}
