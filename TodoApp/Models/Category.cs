using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoApp.Models;

public partial class Category : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string color = "#808080";

    [ObservableProperty]
    private string icon = "📋";

    public override string ToString() => Name;
}
