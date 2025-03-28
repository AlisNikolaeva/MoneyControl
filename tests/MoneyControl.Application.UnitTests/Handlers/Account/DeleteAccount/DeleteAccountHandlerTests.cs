using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Account.DeleteAccount;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Account.DeleteAccount;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Account.DeleteAccount;

public class DeleteAccountHandlerTests
{
    private MsSqlContainer _msSqlContainer;
    private Guid _userId = new("94B0D67A-77AB-49F8-B4DD-9009358CEB7A");

    [SetUp]
    public async Task SetUpAsync()
    {
        _msSqlContainer = new MsSqlBuilder().Build();
        await _msSqlContainer.StartAsync();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenNotExists_ShouldThrowException()
    {
        // Arrange
        var applicationOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString(),
                b =>
                {
                    b.EnableRetryOnFailure(3);
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "dbo");
                    b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })
            .Options;
        var dbContext = new ApplicationDbContext(applicationOptions);
        await dbContext.Database.EnsureCreatedAsync();

        UserContext.SetUserContext(_userId);
        var request = new DeleteAccountCommand
        {
            Id = 1
        };
        var handler = new DeleteAccountHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<ValidationException>(TestDelegate);
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenSuccess_ShouldRemoveAccountAndTransactions()
    {
        // Arrange
        var applicationOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString(),
                b =>
                {
                    b.EnableRetryOnFailure(3);
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "dbo");
                    b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })
            .Options;

        var account = new AccountEntity
        {
            UserId = _userId,
            Name = "Account_test",
            Balance = 0,
            Currency = "USD"
        };

        var dbContext = new ApplicationDbContext(applicationOptions);
        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.Accounts.AddAsync(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var category = new CategoryEntity
        {
            UserId = _userId,
            Name = "Category_test"
        };

        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account,
            Category = category,
            Sum = 10,
            DateUtc = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new DeleteAccountCommand
        {
            Id = 1
        };
        var handler = new DeleteAccountHandler(dbContext);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var accountsCount = await dbContext.Accounts.CountAsync();
        accountsCount.Should().Be(0);

        var transactionsCount = await dbContext.Transactions.CountAsync();
        transactionsCount.Should().Be(0);

        await dbContext.DisposeAsync();
    }
}