namespace MoneyControl.Shared;

public class TransactionModel
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public double Sum { get; set; }
    public DateTime DateUtc { get; set; }
}