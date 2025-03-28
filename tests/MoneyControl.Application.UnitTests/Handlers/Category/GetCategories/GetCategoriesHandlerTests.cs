using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Category.GetCategories;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Category.GetCategories;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Category.GetCategories;

public class GetCategoriesHandlerTests
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
    public async Task Handle_WhenHasCategories_ShouldReturnCategories()
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
        await dbContext.Categories.AddAsync(new CategoryEntity
        {
            UserId = _userId,
            Name = "Category_test1"
        });

        await dbContext.Categories.AddAsync(new CategoryEntity
        {
            UserId = _userId,
            Name = "Category_test2"
        });

        await dbContext.SaveChangesAsync(CancellationToken.None);
        UserContext.SetUserContext(_userId);
        var request = new GetCategoriesCommand();
        var handler = new GetCategoriesHandler(dbContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var expected = new List<CategoryModel>
        {
            new()
            {
                Id = 1,
                Name = "Category_test1"
            },
            new()
            {
                Id = 2,
                Name = "Category_test2"
            }
        };
        result.Should().BeEquivalentTo(expected);
        await dbContext.DisposeAsync();
    }

    [Test]
    public async Task Handle_WhenNoCategories_ShouldReturnEmptyCollection()
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
        var request = new GetCategoriesCommand();
        var handler = new GetCategoriesHandler(dbContext);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        await dbContext.DisposeAsync();
    }
}