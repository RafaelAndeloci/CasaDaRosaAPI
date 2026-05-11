using CasaDaRosa.Domain.Entities.Orders;
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

            totalBuilder.OwnsOne(x => x.Currency, currencyBuilder =>
            {
                currencyBuilder.Property(x => x.Code)
                    .HasColumnName("TotalAmountCurrency")
                    .HasMaxLength(3)
                    .IsRequired(false);
            });
        });

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.TotalAmount).IsRequired();
    }
}
