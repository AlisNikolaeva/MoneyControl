using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Transaction.GetTransactions;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Transaction.GetTransactions;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Transaction.GetTransactions;

public class GetTransactionsHandlerTests
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
    public async Task Handle_WhenHasTransactions_ShouldReturnTransactions()
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

        var account1 = new AccountEntity
        {
            UserId = _userId,
            Name = "Account_test1",
            Balance = 10,
            Currency = "USD"
        };

        var account2 = new AccountEntity
        {
            UserId = _userId,
            Name = "Account_test2",
            Balance = 20,
            Currency = "CAD"
        };

        var account3 = new AccountEntity
        {
            UserId = _userId,
            Name = "Account_test3",
            Balance = 30,
            Currency = "EUR"
        };
        await dbContext.Accounts.AddAsync(account1);
        await dbContext.Accounts.AddAsync(account2);
        await dbContext.Accounts.AddAsync(account3);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var category1 = new CategoryEntity
        {
            UserId = _userId,
            Name = "Category_test1"
        };

        var category2 = new CategoryEntity
        {
            UserId = _userId,
            Name = "Category_test2"
        };

        var category3 = new CategoryEntity
        {
            UserId = _userId,
            Name = "Category_test3"
        };
        await dbContext.Categories.AddAsync(category1);
        await dbContext.Categories.AddAsync(category2);
        await dbContext.Categories.AddAsync(category3);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var date1 = new DateTime(2001, 01, 01);
        var date2 = new DateTime(2002, 02, 02);
        var date3 = new DateTime(2003, 03, 03);
        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account1,
            Category = category1,
            Sum = 10,
            DateUtc = date1
        });
        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account2,
            Category = category2,
            Sum = 20,
            DateUtc = date2
        });
        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account3,
            Category = category3,
            Sum = 30,
            DateUtc = date3
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new GetTransactionsCommand();
        var handler = new GetTransactionsHandler(dbContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var expected = new List<TransactionModel>
        {
            new()
            {
                Id = 1,
                AccountId = 1,
                AccountName = "Account_test1",
                Sum = 10,
                CategoryId = 1,
                CategoryName = "Category_test1",
                DateUtc = date1
            },
            new()
            {
                Id = 2,
                AccountId = 2,
                AccountName = "Account_test2",
                Sum = 20,
                CategoryId = 2,
                CategoryName = "Category_test2",
                DateUtc = date2
            },
            new()
            {
                Id = 3,
                AccountId = 3,
                AccountName = "Account_test3",
                Sum = 30,
                CategoryId = 3,
                CategoryName = "Category_test3",
                DateUtc = date3
            }
        };
        result.Should().BeEquivalentTo(expected);
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenNoTransactions_ShouldReturnEmptyCollection()
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
        var request = new GetTransactionsCommand();
        var handler = new GetTransactionsHandler(dbContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        await dbContext.DisposeAsync();
    }
}