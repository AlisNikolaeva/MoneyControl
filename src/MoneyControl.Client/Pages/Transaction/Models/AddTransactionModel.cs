namespace MoneyControl.Client.Pages.Transaction.Models;

public class AddTransactionModel
{
    public int AccountId { get; set; }
    public double? Sum { get; set; }
    public int CategoryId { get; set; }
}