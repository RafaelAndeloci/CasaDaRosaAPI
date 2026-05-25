using CasaDaRosa.Domain.Entities.Carts;
using CasaDaRosa.Domain.ValueObjects;
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

            unitPriceBuilder.Property(x => x.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency == null ? string.Empty : currency.Code,
                    code => Currency.FromCodeOrNone(code));
        });

        builder.Navigation(x => x.UnitPrice).IsRequired();
    }
}
