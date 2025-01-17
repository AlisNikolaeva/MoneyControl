using System.Collections;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MoneyControl.Application.Handlers.Transaction.SearchTransactions;
using MoneyControl.Core.Entities;
using MoneyControl.Infrastructure;
using MoneyControl.Shared.Models;
using MoneyControl.Shared.Queries.Transaction.SearchTransactions;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace MoneyControl.Application.UnitTests.Handlers.Transaction.SearchTransactions;

public class SearchTransactionsHandlerTests
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

    private static IEnumerable GetPeriodTransactionTestCases
    {
        get
        {
            yield return new TestCaseData(
                "When StartUtc and EndUtc are null",
                null,
                null,
                new List<TransactionModel>
                {
                    new()
                    {
                        Id = 1,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 10,
                        DateUtc = new DateTime(2024, 1, 1)
                    },
                    new()
                    {
                        Id = 2,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 20,
                        DateUtc = new DateTime(2023, 1, 1)
                    },
                    new()
                    {
                        Id = 3,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 30,
                        DateUtc = new DateTime(2022, 1, 1)
                    }
                });
            
            yield return new TestCaseData(
                "When StartUtc is not null",
                new DateTime(2023, 1, 1),
                null,
                new List<TransactionModel>
                {
                    new()
                    {
                        Id = 1,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 10,
                        DateUtc = new DateTime(2024, 1, 1)
                    },
                    new()
                    {
                        Id = 2,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 20,
                        DateUtc = new DateTime(2023, 1, 1)
                    }
                });
            
            yield return new TestCaseData(
                "When EndUtc is not null",
                null,
                new DateTime(2023, 1, 1),
                new List<TransactionModel>
                {
                    new()
                    {
                        Id = 2,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 20,
                        DateUtc = new DateTime(2023, 1, 1)
                    },
                    new()
                    {
                        Id = 3,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 30,
                        DateUtc = new DateTime(2022, 1, 1)
                    }
                });
            
            yield return new TestCaseData(
                "When StartUtc and EndUtc are not null",
                new DateTime(2023, 1, 1),
                new DateTime(2023, 1, 1),
                new List<TransactionModel>
                {
                    new()
                    {
                        Id = 2,
                        AccountId = 1,
                        AccountName = "Account_test",
                        Sum = 20,
                        DateUtc = new DateTime(2023, 1, 1)
                    }
                });
        }
    }
    
    [TestCaseSource(nameof(GetPeriodTransactionTestCases))]
    public async Task Handle_WhenSuccess_ShouldReturnSomeTransactions(string caseName, DateTime? start, DateTime? end, List<TransactionModel> expected)
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
            DateUtc = new DateTime(2024,1,1)
        });
        await dbContext.Transactions.AddAsync(new()
        {
            Account = account,
            Sum = 20,
            DateUtc = new DateTime(2023,1,1)
        });
        await dbContext.Transactions.AddAsync(new()
        {
            Account = account,
            Sum = 30,
            DateUtc = new DateTime(2022,1,1)
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);
    
        var request = new SearchTransactionsQuery
        {
            AccountIds = new List<int> {1},
            StartUtc = start,
            EndUtc = end
        };
        var handler = new SearchTransactionsHandler(dbContext);
    
        // Act
        var result = await handler.Handle(request, CancellationToken.None);
    
        // Assert
        result.Should().BeEquivalentTo(expected);
        await dbContext.DisposeAsync();
    }
}