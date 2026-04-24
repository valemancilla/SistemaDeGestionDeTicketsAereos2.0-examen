using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;

// Configuración de EF Core para mapear PaymentEntity a la tabla Payment
public sealed class PaymentEntityConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
    {
        builder.ToTable("Payment");

        // Clave primaria
        builder.HasKey(x => x.IdPayment);

        // El ID se genera automáticamente
        builder.Property(x => x.IdPayment)
            .HasColumnName("IdPayment")
            .ValueGeneratedOnAdd();

        // FK a la reserva, obligatoria
        builder.Property(x => x.IdBooking)
            .HasColumnName("IdBooking")
            .IsRequired();

        // FK opcional al tiquete asociado al pago
        builder.Property(x => x.IdTicket)
            .HasColumnName("IdTicket");

        // FK al método de pago, obligatoria
        builder.Property(x => x.IdPaymentMethod)
            .HasColumnName("IdPaymentMethod")
            .IsRequired();

        // Monto del pago, hasta 12 dígitos con 2 decimales
        builder.Property(x => x.Amount)
            .HasColumnName("Amount")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        // Fecha y hora del pago, se registra automáticamente al momento de crear el registro
        builder.Property(x => x.PaymentDate)
            .HasColumnName("PaymentDate")
            .HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        // FK al estado del pago, obligatoria
        builder.Property(x => x.IdStatus)
            .HasColumnName("IdStatus")
            .IsRequired();

        // Relación: un pago pertenece a una reserva, una reserva puede tener varios pagos
        builder.HasOne(x => x.Booking)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.IdBooking)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Ticket)
            .WithMany()
            .HasForeignKey(x => x.IdTicket)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación: un pago se realizó con un método específico
        builder.HasOne(x => x.PaymentMethod)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.IdPaymentMethod)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un pago tiene un estado que puede cambiar (pendiente → aprobado)
        builder.HasOne(x => x.Status)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
