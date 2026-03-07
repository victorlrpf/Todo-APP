using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace TodoApp.Models;

public enum TaskStatus
{
    EmAndamento,
    Concluido,
    Atrasado
}

[Table("tasks")]
public partial class TaskItem : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private TaskStatus status = TaskStatus.EmAndamento;

    [ObservableProperty]
    private int categoryId;

    [Ignore]
    [ObservableProperty]
    private Category? category;

    [ObservableProperty]
    private DateTime createdAt = DateTime.Now;

    [ObservableProperty]
    private DateTime? completedAt;

    [ObservableProperty]
    private DateTime? dueDate;

    public bool IsOverdue => dueDate.HasValue && dueDate.Value < DateTime.Now && status != TaskStatus.Concluido;
}
