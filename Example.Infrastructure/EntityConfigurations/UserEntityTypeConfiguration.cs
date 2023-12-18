using Example.Domain.Entities;
using Example.Domain.ValueObjects;
using Example.Infrastructure.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Infrastructure.EntityConfigurations;

internal sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion<UserIdConverter>()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();
        
        builder.OwnsOne(x => x.Name, navigation =>
        {
            navigation.Property(x => x.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(Name.MaxFirstNameLen)
                .IsRequired();

            navigation.Property(x => x.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(Name.MaxLastNameLen)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Address, navigation =>
        {
            navigation.Property(x => x.CountryCode)
                .HasColumnName("country_code")
                .HasMaxLength(2)
                .IsRequired();

            navigation.Property(x => x.City)
                .HasColumnName("city")
                .HasMaxLength(128)
                .IsRequired();

            navigation.Property(x => x.Street)
                .HasColumnName("street")
                .HasMaxLength(128)
                .IsRequired();

            navigation.Property(x => x.Building)
                .HasColumnName("building")
                .HasMaxLength(64)
                .IsRequired();
        });

        
        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.Version)
            .IsRowVersion();
        
        builder.Metadata
            .FindNavigation(nameof(User.Cards))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Cards)
            .WithOne()
            .HasForeignKey("userId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}