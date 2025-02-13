namespace MoneyControl.Shared.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; }
    public double Sum { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime DateUtc { get; set; }
}