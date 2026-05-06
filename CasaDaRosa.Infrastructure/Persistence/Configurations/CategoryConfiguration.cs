using CasaDaRosa.Domain.ValueObjects;
using CasaDaRosa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        });

        builder.OwnsOne(x => x.Description, descriptionBuilder =>
        {
            descriptionBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Category.Description))
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.Navigation(x => x.Name).IsRequired();

        builder.HasIndex("Name")
            .IsUnique();
    }
}
