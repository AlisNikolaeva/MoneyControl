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
        await dbContext.Accounts.AddAsync(new AccountEntity
        {
            Name = "Account_test",
            Balance = 0,
            Currency = "USD"
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var request = new DeleteAccountCommand
        {
            Id = 1
        };
        var handler = new DeleteAccountHandler(dbContext);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var count = await dbContext.Accounts.CountAsync();
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
}