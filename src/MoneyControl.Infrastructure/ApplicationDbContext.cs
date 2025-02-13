using Microsoft.EntityFrameworkCore;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure.Configurations;

namespace MoneyControl.Infrastructure;


public class ApplicationDbContext : DbContext
{
    public DbSet<AccountEntity> Accounts { get; set; } = null!;
    public DbSet<TransactionEntity> Transactions { get; set; } = null!;
    public DbSet<CategoryEntity> Categories { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
    { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("dbo");
        builder.ApplyConfiguration(new AccountConfiguration());
        builder.ApplyConfiguration(new TransactionConfiguration());
        builder.ApplyConfiguration(new CategoryConfiguration());
        base.OnModelCreating(builder);
    }
}