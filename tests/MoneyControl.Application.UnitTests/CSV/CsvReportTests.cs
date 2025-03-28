using System.Collections;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.CSV;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.CSV;

public class CsvReportTests
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

    private static IEnumerable GetPeriodTransactionTestCases
    {
        get
        {
            yield return new TestCaseData(
                "When StartUtc and EndUtc are null",
                null,
                null,
                new List<CsvData>
                {
                    new()
                    {
                        Id = 1,
                        AccountId = 1,
                        AccountName = "Account_test1",
                        Sum = 10,
                        Currency = "USD",
                        Category = "Category_test1",
                        DateUtc = new DateTime(2001, 1, 1).ToShortDateString()
                    },
                    new()
                    {
                        Id = 2,
                        AccountId = 2,
                        AccountName = "Account_test2",
                        Sum = 20,
                        Currency = "CAD",
                        Category = "Category_test2",
                        DateUtc = new DateTime(2002, 2, 2).ToShortDateString()
                    },
                    new()
                    {
                        Id = 3,
                        AccountId = 3,
                        AccountName = "Account_test3",
                        Sum = 30,
                        Currency = "EUR",
                        Category = "Category_test3",
                        DateUtc = new DateTime(2003, 3, 3).ToShortDateString()
                    },
                    new()
                    {
                        Id = 4,
                        AccountId = 4,
                        AccountName = "Account_test4",
                        Sum = 40,
                        Currency = "AED",
                        Category = "Category_test4",
                        DateUtc = new DateTime(2004, 4, 4).ToShortDateString()
                    }
                });

            yield return new TestCaseData(
                "When StartUtc is not null",
                new DateTime(2002, 2, 2),
                null,
                new List<CsvData>
                {
                    new()
                    {
                        Id = 2,
                        AccountId = 2,
                        AccountName = "Account_test2",
                        Sum = 20,
                        Currency = "CAD",
                        Category = "Category_test2",
                        DateUtc = new DateTime(2002, 2, 2).ToShortDateString()
                    },
                    new()
                    {
                        Id = 3,
                        AccountId = 3,
                        AccountName = "Account_test3",
                        Sum = 30,
                        Currency = "EUR",
                        Category = "Category_test3",
                        DateUtc = new DateTime(2003, 3, 3).ToShortDateString()
                    },
                    new()
                    {
                        Id = 4,
                        AccountId = 4,
                        AccountName = "Account_test4",
                        Sum = 40,
                        Currency = "AED",
                        Category = "Category_test4",
                        DateUtc = new DateTime(2004, 4, 4).ToShortDateString()
                    }
                });

            yield return new TestCaseData(
                "When EndUtc is not null",
                null,
                new DateTime(2002, 2, 2),
                new List<CsvData>
                {
                    new()
                    {
                        Id = 1,
                        AccountId = 1,
                        AccountName = "Account_test1",
                        Sum = 10,
                        Currency = "USD",
                        Category = "Category_test1",
                        DateUtc = new DateTime(2001, 1, 1).ToShortDateString()
                    },
                    new()
                    {
                        Id = 2,
                        AccountId = 2,
                        AccountName = "Account_test2",
                        Sum = 20,
                        Currency = "CAD",
                        Category = "Category_test2",
                        DateUtc = new DateTime(2002, 2, 2).ToShortDateString()
                    }
                });

            yield return new TestCaseData(
                "When StartUtc and EndUtc are not null",
                new DateTime(2002, 2, 2),
                new DateTime(2002, 2, 2),
                new List<CsvData>
                {
                    new()
                    {
                        Id = 2,
                        AccountId = 2,
                        AccountName = "Account_test2",
                        Sum = 20,
                        Currency = "CAD",
                        Category = "Category_test2",
                        DateUtc = new DateTime(2002, 2, 2).ToShortDateString()
                    }
                });
        }
    }

    [TestCaseSource(nameof(GetPeriodTransactionTestCases))]
    public async Task Handle_WhenSuccess_ShouldReturnSomeTransactions(string caseName, DateTime? start, DateTime? end,
        List<CsvData> expected)
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

        var account4 = new AccountEntity
        {
            UserId = _userId,
            Name = "Account_test4",
            Balance = 40,
            Currency = "AED"
        };
        await dbContext.Accounts.AddAsync(account1);
        await dbContext.Accounts.AddAsync(account2);
        await dbContext.Accounts.AddAsync(account3);
        await dbContext.Accounts.AddAsync(account4);
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

        var category4 = new CategoryEntity
        {
            UserId = _userId,
            Name = "Category_test4"
        };
        await dbContext.Categories.AddAsync(category1);
        await dbContext.Categories.AddAsync(category2);
        await dbContext.Categories.AddAsync(category3);
        await dbContext.Categories.AddAsync(category4);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account1,
            Category = category1,
            Sum = 10,
            DateUtc = new DateTime(2001, 1, 1)
        });

        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account2,
            Category = category2,
            Sum = 20,
            DateUtc = new DateTime(2002, 2, 2)
        });

        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account3,
            Category = category3,
            Sum = 30,
            DateUtc = new DateTime(2003, 3, 3)
        });

        await dbContext.Transactions.AddAsync(new TransactionEntity
        {
            Account = account4,
            Category = category4,
            Sum = 40,
            DateUtc = new DateTime(2004, 4, 4)
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        UserContext.SetUserContext(_userId);

        var parameters = new CsvParameters
        {
            AccountIds = new List<int> { 1, 2, 3, 4 },
            StartUtc = start,
            EndUtc = end
        };

        var csvReport = new CsvReport(dbContext);

        // Act
        var result = await csvReport.GetFilteredTransactions(parameters, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expected);
        await dbContext.DisposeAsync();
    }
}