namespace MoneyControl.Application.CSV;

public class CsvParameters
{
    public List<int> AccountIds { get; set; } = new();
    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
}