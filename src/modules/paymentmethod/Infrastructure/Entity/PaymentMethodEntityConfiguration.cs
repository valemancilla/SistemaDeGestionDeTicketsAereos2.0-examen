using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Entity;

// Configuración de EF Core para la tabla PaymentMethod
public sealed class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethodEntity>
{
    public void Configure(EntityTypeBuilder<PaymentMethodEntity> builder)
    {
        builder.ToTable("PaymentMethod");

        // Clave primaria
        builder.HasKey(x => x.IdPaymentMethod);

        // Se genera automáticamente
        builder.Property(x => x.IdPaymentMethod)
            .HasColumnName("IdPaymentMethod")
            .ValueGeneratedOnAdd();

        // Nombre del método de pago, obligatorio, máximo 50 caracteres
        builder.Property(x => x.MethodName)
            .HasColumnName("MethodName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.HasData(
            new PaymentMethodEntity { IdPaymentMethod = 1, MethodName = "Tarjeta de crédito" },
            new PaymentMethodEntity { IdPaymentMethod = 2, MethodName = "Tarjeta de débito" },
            new PaymentMethodEntity { IdPaymentMethod = 3, MethodName = "Transferencia bancaria" },
            new PaymentMethodEntity { IdPaymentMethod = 4, MethodName = "Efectivo" },
            new PaymentMethodEntity { IdPaymentMethod = 5, MethodName = "PSE" }
        );
    }
}
