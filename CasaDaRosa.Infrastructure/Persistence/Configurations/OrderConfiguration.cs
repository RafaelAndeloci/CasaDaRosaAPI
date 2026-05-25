using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CasaDaRosa.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.TotalAmount, totalBuilder =>
        {
            totalBuilder.Property(x => x.Amount)
                .HasColumnName(nameof(Order.TotalAmount))
                .HasPrecision(18, 2)
                .IsRequired();

            totalBuilder.Property(x => x.Currency)
                .HasColumnName("TotalAmountCurrency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency == null ? string.Empty : currency.Code,
                    code => Currency.FromCodeOrNone(code));
        });

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TotalAmount).IsRequired();
    }
}
