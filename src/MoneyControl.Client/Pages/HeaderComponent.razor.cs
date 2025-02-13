using Microsoft.AspNetCore.Components;

namespace MoneyControl.Client.Pages;

public class HeaderComponentBase : ComponentBase
{
    [Parameter] public string HeaderText { get; set; }
    [Parameter] public string AddButtonText { get; set; }
    [Parameter] public string ExportButtonText { get; set; }
    [Parameter] public string FilterButtonText { get; set; }
    [Parameter] public EventCallback OnAdd { get; set; }
    [Parameter] public EventCallback OnExport { get; set; }
    [Parameter] public EventCallback OnFilter { get; set; }

    protected void OnAddHandler()
    {
        if (OnAdd.HasDelegate)
        {
            OnAdd.InvokeAsync();
        }
    }

    protected void OnExportHandler()
    {
        if (OnExport.HasDelegate)
        {
            OnExport.InvokeAsync();
        }
    }

    protected void OnFilterHandler()
    {
        if (OnFilter.HasDelegate)
        {
            OnFilter.InvokeAsync();
        }
    }
}