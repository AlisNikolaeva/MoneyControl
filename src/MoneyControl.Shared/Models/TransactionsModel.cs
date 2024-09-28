namespace MoneyControl.Shared.Models;

public class TransactionsModel
{
    public IEnumerable<TransactionModel> Items { get; set; }
    public int TotalCount { get; set; }
}