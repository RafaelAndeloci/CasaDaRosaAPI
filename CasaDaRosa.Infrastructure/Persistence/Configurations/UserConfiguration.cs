using CasaDaRosa.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CasaDaRosa.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Name, nameBuilder =>
        {
            nameBuilder.Property(x => x.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired();

            nameBuilder.Property(x => x.Surname)
                .HasColumnName("Surname")
                .HasMaxLength(150)
                .IsRequired();

            nameBuilder.Property(x => x.FullName)
                .HasColumnName(nameof(User.Name))
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Email, emailBuilder =>
        {
            emailBuilder.Property(x => x.Value)
                .HasColumnName(nameof(User.Email))
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.OwnsOne(x => x.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(x => x.FormattedValue)
                .HasColumnName(nameof(User.PhoneNumber))
                .HasMaxLength(20)
                .IsRequired(false);

            phoneBuilder.Property(x => x.RawValue)
                .HasColumnName("PhoneNumberRawValue")
                .IsRequired(false);

            phoneBuilder.Property(x => x.AreaCode)
                .HasColumnName("PhoneNumberAreaCode")
                .IsRequired(false);

            phoneBuilder.Property(x => x.CountryCode)
                .HasColumnName("PhoneNumberCountryCode")
                .IsRequired(false);
        });

        builder.Navigation(x => x.Name).IsRequired();
        builder.Navigation(x => x.Email).IsRequired();

        builder.HasIndex("Email")
            .IsUnique();

        builder.HasMany(x => x.Addresses)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
