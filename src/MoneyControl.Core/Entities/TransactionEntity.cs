namespace MoneyControl.Core.Entities;

public class TransactionEntity
{
    public int Id { get; set; }
    public AccountEntity Account { get; set; }
    public double Sum { get; set; }
    public DateTime DateUtc { get; set; }
}