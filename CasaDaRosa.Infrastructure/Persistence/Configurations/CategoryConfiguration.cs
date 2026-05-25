using CasaDaRosa.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CasaDaRosa.Domain.Entities.Categories;

namespace CasaDaRosa.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Name, nameBuilder =>
        {
            nameBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Category.Name))
                .HasMaxLength(120)
                .IsRequired();

            nameBuilder.HasIndex(x => x.Value)
                .IsUnique();
        });

        builder.OwnsOne(x => x.Description, descriptionBuilder =>
        {
            descriptionBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Category.Description))
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.Navigation(x => x.Name).IsRequired();
    }
}
