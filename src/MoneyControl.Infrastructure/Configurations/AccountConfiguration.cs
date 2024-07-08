using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyControl.Core.Entities;

namespace MoneyControl.Infrastructure.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.ToTable("Account");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();
    }
}