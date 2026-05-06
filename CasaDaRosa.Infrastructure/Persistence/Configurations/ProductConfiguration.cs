using CasaDaRosa.Domain.ValueObjects;
using CasaDaRosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CasaDaRosa.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Name, nameBuilder =>
        {
            nameBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Product.Name))
                .HasMaxLength(150)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Description, descriptionBuilder =>
        {
            descriptionBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Product.Description))
                .HasMaxLength(1000)
                .IsRequired(false);
        });

        builder.OwnsOne(x => x.Price, priceBuilder =>
        {
            priceBuilder.Property(x => x.Amount)
                .HasColumnName(nameof(Product.Price))
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.OwnsOne(x => x.StockQuantity, stockBuilder =>
        {
            stockBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Product.StockQuantity))
                .IsRequired();
        });

        builder.Navigation(x => x.Name).IsRequired();
        builder.Navigation(x => x.Price).IsRequired();
        builder.Navigation(x => x.StockQuantity).IsRequired();

        builder.HasIndex("Name");

        builder.HasMany(x => x.Reviews)
            .WithOne()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
