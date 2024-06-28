namespace MoneyControl.Core;

public class Transaction
{
    public int Id { get; set; }
    public Account Account { get; set; }
    public double Sum { get; set; }
    public DateTime DateUtc { get; set; }
}