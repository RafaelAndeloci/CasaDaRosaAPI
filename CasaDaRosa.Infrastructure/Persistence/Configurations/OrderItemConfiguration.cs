using CasaDaRosa.Domain.Entities.Orders;
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

            unitPriceBuilder.OwnsOne(x => x.Currency, currencyBuilder =>
            {
                currencyBuilder.Property(x => x.Code)
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired(false);
            });
        });

        builder.OwnsOne(x => x.Total, totalBuilder =>
        {
            totalBuilder.Property(x => x.Amount)
                .HasColumnName(nameof(OrderItem.Total))
                .HasPrecision(18, 2)
                .IsRequired();

            totalBuilder.OwnsOne(x => x.Currency, currencyBuilder =>
            {
                currencyBuilder.Property(x => x.Code)
                    .HasColumnName("TotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired(false);
            });
        });

        builder.Navigation(x => x.UnitPrice).IsRequired();
        builder.Navigation(x => x.Total).IsRequired();
    }
}
