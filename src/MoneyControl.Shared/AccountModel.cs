namespace MoneyControl.Shared;

public class AccountModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Balance { get; set; } = 0;
    public string Currency { get; set; }
}