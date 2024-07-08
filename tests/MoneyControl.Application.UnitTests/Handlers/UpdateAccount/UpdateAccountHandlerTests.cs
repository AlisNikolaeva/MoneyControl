using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.UpdateAccount;
using MoneyControl.Infrastructure;
using MoneyControl.Shared;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.UpdateAccount;

public class UpdateAccountHandlerTests
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
        await dbContext.Accounts.AddAsync(new()
        {
            Balance = 0,
            Currency = "USD",
            Name = "Account_test"
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        var request = new UpdateAccountCommand
        {
            Id = 1,
            Name = "Account_test2",
            Currency = "CAD"
        };
        var handler = new UpdateAccountHandler(dbContext);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var expected = new AccountModel
        {
            Id = 1,
            Name = "Account_test2",
            Balance = 0,
            Currency = "CAD"
        };
        var account = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == 1);
        account.Should().BeEquivalentTo(expected);
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
        var request = new UpdateAccountCommand
        {
            Id = 2,
            Name = "Account_test2",
            Currency = "CAD"
        };
        var handler = new UpdateAccountHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<Exception>(TestDelegate);
        await dbContext.DisposeAsync();
    }
}