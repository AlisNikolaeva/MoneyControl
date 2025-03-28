using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Category.UpdateCategory;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Category.UpdateCategory;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Category.UpdateCategory;

public class UpdateCategoryHandlerTests
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
        await dbContext.Categories.AddAsync(new()
        {
            UserId = _userId,
            Name = "Category_test1"
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new UpdateCategoryCommand
        {
            Id = 1,
            Name = "Category_test2"
        };
        var handler = new UpdateCategoryHandler(dbContext);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var expected = new CategoryModel
        {
            Id = 1,
            Name = "Category_test2"
        };
        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == 1);
        category.Should().BeEquivalentTo(expected);
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
        var request = new UpdateCategoryCommand
        {
            Id = 2,
            Name = "Category_test2"
        };
        var handler = new UpdateCategoryHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<ValidationException>(TestDelegate);
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenNameExists_ShouldThrowException()
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
        await dbContext.Categories.AddAsync(new()
        {
            UserId = _userId,
            Name = "Category_test"
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);
        var request = new UpdateCategoryCommand
        {
            Id = 1,
            Name = "Category_test"
        };
        var handler = new UpdateCategoryHandler(dbContext);

        // Act
        async Task TestDelegate() => await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.ThrowsAsync<ValidationException>(TestDelegate);
        await dbContext.DisposeAsync();
    }
}