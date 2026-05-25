using CasaDaRosa.Domain.Entities.Orders;
using CasaDaRosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CasaDaRosa.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductNameSnapshot)
            .HasMaxLength(150)
            .IsRequired();

        builder.OwnsOne(x => x.UnitPrice, unitPriceBuilder =>
        {
            unitPriceBuilder.Property(x => x.Amount)
                .HasColumnName(nameof(OrderItem.UnitPrice))
                .HasPrecision(18, 2)
                .IsRequired();

            unitPriceBuilder.Property(x => x.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency == null ? string.Empty : currency.Code,
                    code => Currency.FromCodeOrNone(code));
        });

        builder.OwnsOne(x => x.Total, totalBuilder =>
        {
            totalBuilder.Property(x => x.Amount)
                .HasColumnName(nameof(OrderItem.Total))
                .HasPrecision(18, 2)
                .IsRequired();

            totalBuilder.Property(x => x.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency == null ? string.Empty : currency.Code,
                    code => Currency.FromCodeOrNone(code));
        });

        builder.Navigation(x => x.UnitPrice).IsRequired();
        builder.Navigation(x => x.Total).IsRequired();
    }
}
