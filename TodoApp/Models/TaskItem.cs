using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoApp.Models;

public partial class TaskItem : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private bool isCompleted;

    [ObservableProperty]
    private Category? category;

    [ObservableProperty]
    private DateTime createdAt = DateTime.Now;
}
