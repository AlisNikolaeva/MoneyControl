using Microsoft.EntityFrameworkCore;
using MoneyControl.Core;
using MoneyControl.Infrastructure.Configurations;
using Transaction = MoneyControl.Core.Transaction;

namespace MoneyControl.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
    { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("dbo");
        builder.ApplyConfiguration(new AccountConfiguration());
        builder.ApplyConfiguration(new TransactionConfiguration());
        base.OnModelCreating(builder);
    }
}