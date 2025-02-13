using MoneyControl.Shared.Models;

namespace MoneyControl.Client.Pages.Transaction.Models;

public class FilterParametersModel
{
    public List<AccountModel> Accounts { get; set; } = new();
    public List<int> SelectedAccounts { get; set; } = new();
    public DateTime? StartDate;
    public DateTime? EndDate;
}