using Microsoft.AspNetCore.Components;

namespace MoneyControl.Client.Pages;

public class HeaderComponentBase : ComponentBase
{
    [Parameter] public string HeaderText { get; set; }
    [Parameter] public string ActionButtonText { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }
    protected void OnClickHandler()
    {
        if (OnClick.HasDelegate)
        {
            OnClick.InvokeAsync();
        }
    }
}