using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Transaction.DeleteTransaction;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Transaction.DeleteTransaction;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Transaction.DeleteTransaction;

public class DeleteTransactionHandlerTests
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
    public async Task Handle_WhenSuccess_ShouldRemove()
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

        var account = new AccountEntity
        {
            UserId = _userId,
            Name = "Account_test",
            Balance = 0,
            Currency = "USD"
        };
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
            DateUtc = DateTime.Now
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new DeleteTransactionQuery
        {
            Id = 1
        };
        var handler = new DeleteTransactionHandler(dbContext);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var count = await dbContext.Transactions.CountAsync();
        count.Should().Be(0);
        await dbContext.DisposeAsync();
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
        var request = new DeleteTransactionQuery
        {
            Id = 1
        };
        var handler = new DeleteTransactionHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<ValidationException>(TestDelegate);
        await dbContext.DisposeAsync();
    }
}