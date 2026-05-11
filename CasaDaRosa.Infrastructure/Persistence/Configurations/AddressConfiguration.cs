using CasaDaRosa.Domain.Entities.Addresses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CasaDaRosa.Infrastructure.Persistence.Configurations;

public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Street, streetBuilder =>
        {
            streetBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Address.Street))
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Number, numberBuilder =>
        {
            numberBuilder.Property(x => x.Value)
                .HasColumnName(nameof(Address.Number))
                .IsRequired();
        });

        builder.Property(x => x.Neighborhood).HasMaxLength(100).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();

        builder.OwnsOne(x => x.State, stateBuilder =>
        {
            stateBuilder.OwnsOne(x => x.Abbreviation, abbreviationBuilder =>
            {
                abbreviationBuilder.Property(x => x.Code)
                    .HasColumnName("StateCode")
                    .HasMaxLength(2)
                    .IsRequired();

                abbreviationBuilder.Property(x => x.FullName)
                    .HasColumnName("StateName")
                    .HasMaxLength(50)
                    .IsRequired();
            });
        });

        builder.OwnsOne(x => x.ZipCode, zipCodeBuilder =>
        {
            zipCodeBuilder.Property(x => x.FormattedValue)
                .HasColumnName(nameof(Address.ZipCode))
                .HasMaxLength(10)
                .IsRequired();

            zipCodeBuilder.Property(x => x.RawValue)
                .HasColumnName("ZipCodeRawValue")
                .IsRequired();
        });

        builder.Property(x => x.Complement).HasMaxLength(200);
        builder.Property(x => x.Reference).HasMaxLength(200);

        builder.Navigation(x => x.Street).IsRequired();
        builder.Navigation(x => x.Number).IsRequired();
        builder.Navigation(x => x.State).IsRequired();
        builder.Navigation(x => x.ZipCode).IsRequired();
    }
}
