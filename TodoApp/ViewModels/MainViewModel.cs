using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TodoApp.Models;

namespace TodoApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<TaskItem> tasks;

    [ObservableProperty]
    private ObservableCollection<TaskItem> filteredTasks;

    [ObservableProperty]
    private ObservableCollection<Category> categories;

    [ObservableProperty]
    private string newTaskName = string.Empty;

    [ObservableProperty]
    private string newTaskDescription = string.Empty;

    [ObservableProperty]
    private Category? selectedCategoryForNewTask;

    [ObservableProperty]
    private Category? selectedFilterCategory;

    [ObservableProperty]
    private string newCategoryName = string.Empty;

    [ObservableProperty]
    private string newCategoryColor = "#6366F1";

    [ObservableProperty]
    private string newCategoryIcon = "📋";

    [ObservableProperty]
    private bool isAddingCategory;

    [ObservableProperty]
    private int totalTasks;

    [ObservableProperty]
    private int completedTasks;

    [ObservableProperty]
    private int pendingTasks;

    public MainViewModel()
    {
        Categories = new ObservableCollection<Category>
        {
            new() { Name = "Pessoal", Color = "#EC4899", Icon = "🏠" },
            new() { Name = "Trabalho", Color = "#3B82F6", Icon = "💼" },
            new() { Name = "Estudos", Color = "#10B981", Icon = "📚" },
            new() { Name = "Saúde", Color = "#F59E0B", Icon = "💪" },
            new() { Name = "Compras", Color = "#8B5CF6", Icon = "🛒" }
        };

        Tasks = new ObservableCollection<TaskItem>
        {
            new() { Name = "Comprar leite e pão", Description = "Ir ao mercado pela manhã", Category = Categories[4], CreatedAt = DateTime.Now.AddDays(-1) },
            new() { Name = "Reunião com equipe", Description = "Preparar apresentação do projeto", Category = Categories[1], CreatedAt = DateTime.Now.AddHours(-3) },
            new() { Name = "Estudar .NET MAUI", Description = "Ler documentação oficial", Category = Categories[2], IsCompleted = true, CreatedAt = DateTime.Now.AddDays(-2) },
            new() { Name = "Caminhada no parque", Description = "30 minutos de exercício", Category = Categories[3], CreatedAt = DateTime.Now },
            new() { Name = "Pagar conta de luz", Description = "Vencimento dia 10", Category = Categories[0], CreatedAt = DateTime.Now.AddDays(-1) }
        };

        FilteredTasks = new ObservableCollection<TaskItem>(Tasks);
        UpdateCounters();
    }

    [RelayCommand]
    private void AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskName))
            return;

        var task = new TaskItem
        {
            Name = NewTaskName.Trim(),
            Description = NewTaskDescription?.Trim() ?? string.Empty,
            Category = SelectedCategoryForNewTask ?? Categories.FirstOrDefault(),
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };

        Tasks.Insert(0, task);
        NewTaskName = string.Empty;
        NewTaskDescription = string.Empty;
        SelectedCategoryForNewTask = null;
        ApplyFilter();
        UpdateCounters();
    }

    [RelayCommand]
    private void RemoveTask(TaskItem task)
    {
        if (task == null) return;
        Tasks.Remove(task);
        ApplyFilter();
        UpdateCounters();
    }

    [RelayCommand]
    private void ToggleTask(TaskItem task)
    {
        if (task == null) return;
        task.IsCompleted = !task.IsCompleted;
        UpdateCounters();
    }

    [RelayCommand]
    private void FilterByCategory(Category? category)
    {
        SelectedFilterCategory = category;
        ApplyFilter();
    }

    [RelayCommand]
    private void ShowAllTasks()
    {
        SelectedFilterCategory = null;
        ApplyFilter();
    }

    [RelayCommand]
    private void AddCategory()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
            return;

        Categories.Add(new Category
        {
            Name = NewCategoryName.Trim(),
            Color = NewCategoryColor,
            Icon = string.IsNullOrWhiteSpace(NewCategoryIcon) ? "📋" : NewCategoryIcon
        });

        NewCategoryName = string.Empty;
        NewCategoryColor = "#6366F1";
        NewCategoryIcon = "📋";
        IsAddingCategory = false;
    }

    [RelayCommand]
    private void ToggleAddCategory()
    {
        IsAddingCategory = !IsAddingCategory;
    }

    [RelayCommand]
    private void RemoveCategory(Category category)
    {
        if (category == null) return;

        var tasksToRemove = Tasks.Where(t => t.Category == category).ToList();
        foreach (var task in tasksToRemove)
            Tasks.Remove(task);

        Categories.Remove(category);

        if (SelectedFilterCategory == category)
            SelectedFilterCategory = null;

        ApplyFilter();
        UpdateCounters();
    }

    private void ApplyFilter()
    {
        FilteredTasks.Clear();
        var source = SelectedFilterCategory == null
            ? Tasks
            : Tasks.Where(t => t.Category == SelectedFilterCategory);

        foreach (var task in source)
            FilteredTasks.Add(task);
    }

    private void UpdateCounters()
    {
        TotalTasks = Tasks.Count;
        CompletedTasks = Tasks.Count(t => t.IsCompleted);
        PendingTasks = TotalTasks - CompletedTasks;
    }
}
