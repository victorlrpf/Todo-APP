using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace TodoApp.Models;

[Table("categories")]
public partial class Category : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string color = "#808080";

    [ObservableProperty]
    private string icon = "📋";

    public override string ToString() => Name;
}
