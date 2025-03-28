using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using MoneyControl.Infrastructure;

namespace MoneyControl.Application.CSV;

public class CsvReport
{
    private readonly ApplicationDbContext _dbContext;

    public CsvReport(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [ExcludeFromCodeCoverage]
    public async Task<byte[]> CreateCsvReportAsync(CsvParameters parameters, CancellationToken cancellationToken)
    {
        var filteredTransactions = await GetFilteredTransactions(parameters, cancellationToken);

        using var memoryStream = new MemoryStream();
        await using (var streamWriter = new StreamWriter(memoryStream))
        await using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            await csvWriter.WriteRecordsAsync(filteredTransactions, cancellationToken);
        }

        return memoryStream.ToArray();
    }

    internal async Task<List<CsvData>> GetFilteredTransactions(CsvParameters parameters, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions
            .Where(x => parameters.AccountIds.Contains(x.Account.Id));

        if (parameters.StartUtc.HasValue)
        {
            query = query.Where(x => x.DateUtc >= parameters.StartUtc.Value);
        }

        if (parameters.EndUtc.HasValue)
        {
            query = query.Where(x => x.DateUtc <= parameters.EndUtc.Value);
        }

        query = query.OrderByDescending(x => x.DateUtc)
            .ThenBy(x => x.Account.Name);

        var filteredTransactions = await query.Select(x => new CsvData
        {
            Id = x.Id,
            AccountId = x.Account.Id,
            AccountName = x.Account.Name,
            Sum = x.Sum,
            Currency = x.Account.Currency,
            Category = x.Category.Name,
            DateUtc = x.DateUtc.ToShortDateString()
        }).ToListAsync(cancellationToken);
        
        return filteredTransactions;
    }
}