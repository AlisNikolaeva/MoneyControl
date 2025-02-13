using CsvHelper.Configuration.Attributes;

namespace MoneyControl.Application.CSV;

public class CsvData
{
    [Name("Id")] public int Id { get; set; }
    [Name("Account Id")] public int AccountId { get; set; }
    [Name("Account Name")] public string AccountName { get; set; }
    [Name("Sum")] public double Sum { get; set; }
    [Name("Currency")] public string Currency { get; set; }
    [Name("Category")] public string Category { get; set; }
    [Name("Date")] public string DateUtc { get; set; }
}