using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.CreateAccount;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.CreateAccount;

public class CreateAccountHandlerTests
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
    public async Task Handle_WhenSuccess_ShouldReturnId()
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
        var request = new CreateAccountCommand
        {
            Name = "Account_test",
            Currency = "USD"
        };
        var handler = new CreateAccountHandler(dbContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBe(0);
        await dbContext.DisposeAsync();
    }
    
    [Test]
    public async Task Handle_WhenExists_ShouldThrowException()
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
        
        var request = new CreateAccountCommand
        {
            Name = "Account_test",
            Currency = "USD"
        };
        var handler = new CreateAccountHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<Exception>(TestDelegate);
        await dbContext.DisposeAsync();
    }
}