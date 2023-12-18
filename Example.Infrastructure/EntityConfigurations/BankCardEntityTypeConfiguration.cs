using Example.Domain;
using Example.Domain.Entities;
using Example.Infrastructure.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.Infrastructure.EntityConfigurations;

internal sealed class BankCardEntityTypeConfiguration : IEntityTypeConfiguration<BankCard>
{
    public void Configure(EntityTypeBuilder<BankCard> builder)
    {
        builder.ToTable("bank_cards");

        builder.Property(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");
        
        builder.Property<UserId>("userId")
            .HasConversion<UserIdConverter>()
            .HasColumnName("user_id");
        
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Number)
            .HasColumnName("number")
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.Expiry)
            .HasColumnName("expiry")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
