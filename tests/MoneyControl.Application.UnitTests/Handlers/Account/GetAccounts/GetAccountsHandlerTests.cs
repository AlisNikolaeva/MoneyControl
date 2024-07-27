using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Account.GetAccounts;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Account.GetAccounts;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Account.GetAccounts;

public class GetAccountsHandlerTests
{
    private MsSqlContainer _msSqlContainer;
    
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
    public async Task Handle_WhenHasAccounts_ShouldReturnAccounts()
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
        await dbContext.Accounts.AddAsync(new AccountEntity
        {
            Name = "Account_test",
            Balance = 0,
            Currency = "USD"
        });
        await dbContext.Accounts.AddAsync(new AccountEntity
        {
            Name = "Account_test2",
            Balance = 0,
            Currency = "USD"
        });
        await dbContext.Accounts.AddAsync(new AccountEntity
        {
            Name = "Account_test3",
            Balance = 0,
            Currency = "USD"
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var request = new GetAccountsCommand();
        var handler = new GetAccountsHandler(dbContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var expected = new List<AccountModel>
        {
            new()
            {
                Id = 1,
                Name = "Account_test",
                Balance = 0,
                Currency = "USD"
            },
            new()
            {
                Id = 2,
                Name = "Account_test2",
                Balance = 0,
                Currency = "USD"
            },
            new()
            {
                Id = 3,
                Name = "Account_test3",
                Balance = 0,
                Currency = "USD"
            }
        };
        result.Should().BeEquivalentTo(expected);
        await dbContext.DisposeAsync();
    }
    
    [Test]
    public async Task Handle_WhenNoAccounts_ShouldReturnEmptyCollection()
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
        var request = new GetAccountsCommand();
        var handler = new GetAccountsHandler(dbContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        await dbContext.DisposeAsync();
    }
}