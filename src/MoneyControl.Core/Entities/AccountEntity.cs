namespace MoneyControl.Core.Entities;

public class AccountEntity
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public double Balance { get; set; } = 0;
    public string Currency { get; set; }
    public DateTime CreatedUtc { get; set; }
}