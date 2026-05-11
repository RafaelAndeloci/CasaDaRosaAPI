using CasaDaRosa.Domain.Entities.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CasaDaRosa.Infrastructure.Persistence.Configurations;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.UnitPrice, unitPriceBuilder =>
        {
            unitPriceBuilder.Property(x => x.Amount)
                .HasColumnName(nameof(CartItem.UnitPrice))
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

        builder.Navigation(x => x.UnitPrice).IsRequired();
    }
}
