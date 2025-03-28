using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Transaction.UpdateTransaction;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Queries.Transaction.UpdateTransaction;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Transaction.UpdateTransaction;

public class UpdateTransactionHandlerTests
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
    public async Task Handle_WhenSuccess_ShouldUpdate()
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

        await dbContext.Accounts.AddAsync(account1);
        await dbContext.Accounts.AddAsync(account2);
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

        await dbContext.Categories.AddAsync(category1);
        await dbContext.Categories.AddAsync(category2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await dbContext.Transactions.AddAsync(new()
        {
            Account = account1,
            Category = category1,
            Sum = 10,
            DateUtc = new DateTime(2001, 1, 1)
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new UpdateTransactionCommand
        {
            Id = 1,
            AccountId = 2,
            Sum = 20,
            CategoryId = 2,
            DateUtc = new DateTime(2002, 2, 2)
        };
        var handler = new UpdateTransactionHandler(dbContext);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var expected = new TransactionEntity
        {
            Id = 1,
            Account = account2,
            Category = category2,
            Sum = 20,
            DateUtc = new DateTime(2002, 2, 2)
        };
        var transaction = await dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == 1);
        transaction.Should().BeEquivalentTo(expected);
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenSuccessAndNoChangingAccount_ShouldUpdate()
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

        await dbContext.Categories.AddAsync(category1);
        await dbContext.Categories.AddAsync(category2);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await dbContext.Transactions.AddAsync(new()
        {
            Account = account,
            Category = category1,
            Sum = 10,
            DateUtc = new DateTime(2001, 1, 1)
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new UpdateTransactionCommand
        {
            Id = 1,
            AccountId = 1,
            Sum = 20,
            CategoryId = 2,
            DateUtc = new DateTime(2002, 2, 2)
        };
        var handler = new UpdateTransactionHandler(dbContext);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var expected = new TransactionEntity
        {
            Id = 1,
            Account = account,
            Category = category2,
            Sum = 20,
            DateUtc = new DateTime(2002, 2, 2)
        };
        var transaction = await dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == 1);
        transaction.Should().BeEquivalentTo(expected);
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenNoAccount_ShouldThrowException()
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

        await dbContext.Transactions.AddAsync(new()
        {
            Account = account,
            Sum = 10,
            DateUtc = new DateTime(2001, 1, 1)
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new UpdateTransactionCommand
        {
            Id = 1,
            AccountId = 2,
            Sum = 20,
            DateUtc = new DateTime(2002, 2, 2)
        };
        var handler = new UpdateTransactionHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<ValidationException>(TestDelegate);
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenNoTransaction_ShouldThrowException()
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
        await dbContext.Accounts.AddAsync(new()
        {
            Name = "Account_test",
            Balance = 0,
            Currency = "USD"
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new UpdateTransactionCommand
        {
            Id = 1,
            AccountId = 1,
            Sum = 20,
            DateUtc = new DateTime(2002, 2, 2)
        };
        var handler = new UpdateTransactionHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<ValidationException>(TestDelegate);
        await dbContext.DisposeAsync();
    }
}