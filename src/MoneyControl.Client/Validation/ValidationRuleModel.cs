using System.Linq.Expressions;

namespace MoneyControl.Client.Validation;

public class ValidationRuleModel
{
    public Func<bool> Satisfy { get; set; }
    public Expression<Func<object>> Accessor { get; set; }
    public string Message { get; set; }
}