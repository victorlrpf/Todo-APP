using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace TodoApp.Models;

public enum TodoStatus
{
    EmAndamento,
    Concluido,
    Atrasado
}

[Table("tasks")]
public partial class TaskItem : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    public TodoStatus Status { get; set; } = TodoStatus.EmAndamento;

    public int CategoryId { get; set; }

    [Ignore]
    public Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? CompletedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsOverdue =>
        DueDate.HasValue &&
        DueDate.Value < DateTime.Now &&
        Status != TodoStatus.Concluido;
}
