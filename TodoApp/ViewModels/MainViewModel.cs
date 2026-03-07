using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public enum SortOption
{
    DataCriacao,
    DataConclusao,
    Alfabetica
}

public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

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
    private TodoStatus? selectedFilterStatus;

    [ObservableProperty]
    private SortOption selectedSortOption = SortOption.DataCriacao;

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

    [ObservableProperty]
    private bool isDarkTheme;

    public MainViewModel()
    {
        _databaseService = new DatabaseService();
        Tasks = new ObservableCollection<TaskItem>();
        FilteredTasks = new ObservableCollection<TaskItem>();
        Categories = new ObservableCollection<Category>();
        
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await LoadCategoriesAsync();
        await LoadTasksAsync();
        UpdateCounters();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categoriesFromDb = await _databaseService.GetCategoriesAsync();
            
            if (categoriesFromDb.Count == 0)
            {
                // Criar categorias padrão
                var defaultCategories = new List<Category>
                {
                    new() { Name = "Pessoal", Color = "#EC4899", Icon = "🏠" },
                    new() { Name = "Trabalho", Color = "#3B82F6", Icon = "💼" },
                    new() { Name = "Estudos", Color = "#10B981", Icon = "📚" },
                    new() { Name = "Saúde", Color = "#F59E0B", Icon = "💪" },
                    new() { Name = "Compras", Color = "#8B5CF6", Icon = "🛒" }
                };

                foreach (var category in defaultCategories)
                {
                    await _databaseService.SaveCategoryAsync(category);
                    Categories.Add(category);
                }
            }
            else
            {
                foreach (var category in categoriesFromDb)
                    Categories.Add(category);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
        }
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            var tasksFromDb = await _databaseService.GetTasksAsync();
            Tasks.Clear();
            foreach (var task in tasksFromDb)
                Tasks.Add(task);
            
            ApplyFilter();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar tarefas: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskName))
            return;

        var task = new TaskItem
        {
            Name = NewTaskName.Trim(),
            Description = NewTaskDescription?.Trim() ?? string.Empty,
            CategoryId = SelectedCategoryForNewTask?.Id ?? 0,
            Category = SelectedCategoryForNewTask ?? Categories.FirstOrDefault(),
            Status = TodoStatus.EmAndamento,
            CreatedAt = DateTime.Now
        };

        await _databaseService.SaveTaskAsync(task);
        Tasks.Insert(0, task);
        
        NewTaskName = string.Empty;
        NewTaskDescription = string.Empty;
        SelectedCategoryForNewTask = null;
        
        ApplyFilter();
        UpdateCounters();
    }

    [RelayCommand]
    private async Task RemoveTask(TaskItem task)
    {
        if (task == null) return;
        
        await _databaseService.DeleteTaskAsync(task);
        Tasks.Remove(task);
        ApplyFilter();
        UpdateCounters();
    }

    [RelayCommand]
    private async Task CompleteTask(TaskItem task)
    {
        if (task == null) return;
        
        task.Status = TodoStatus.Concluido;
        task.CompletedAt = DateTime.Now;
        
        await _databaseService.SaveTaskAsync(task);
        ApplyFilter();
        UpdateCounters();
    }

    [RelayCommand]
    private async Task ChangeTaskStatus(TaskItem task)
    {
        if (task == null) return;
        
        // Ciclar entre os status
        task.Status = task.Status switch
        {
            TodoStatus.EmAndamento => TodoStatus.Concluido,
            TodoStatus.Concluido => TodoStatus.Atrasado,
            TodoStatus.Atrasado => TodoStatus.EmAndamento,
            _ => TodoStatus.EmAndamento
        };

        if (task.Status == TodoStatus.Concluido)
            task.CompletedAt = DateTime.Now;

        await _databaseService.SaveTaskAsync(task);
        ApplyFilter();
        UpdateCounters();
    }

    [RelayCommand]
    private void FilterByCategory(Category? category)
    {
        SelectedFilterCategory = category;
        ApplyFilter();
    }

    [RelayCommand]
    private void FilterByStatus(TodoStatus? status)
    {
        SelectedFilterStatus = status;
        ApplyFilter();
    }

    [RelayCommand]
    private void ShowAllTasks()
    {
        SelectedFilterCategory = null;
        SelectedFilterStatus = null;
        ApplyFilter();
    }

    [RelayCommand]
    private void ChangeSortOption(SortOption sortOption)
    {
        SelectedSortOption = sortOption;
        ApplyFilter();
    }

    [RelayCommand]
    private async Task AddCategory()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
            return;

        var category = new Category
        {
            Name = NewCategoryName.Trim(),
            Color = NewCategoryColor,
            Icon = string.IsNullOrWhiteSpace(NewCategoryIcon) ? "📋" : NewCategoryIcon
        };

        await _databaseService.SaveCategoryAsync(category);
        Categories.Add(category);

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
    private async Task RemoveCategory(Category category)
    {
        if (category == null) return;

        var tasksToRemove = Tasks.Where(t => t.CategoryId == category.Id).ToList();
        foreach (var task in tasksToRemove)
            await RemoveTaskCommand.ExecuteAsync(task);

        await _databaseService.DeleteCategoryAsync(category);
        Categories.Remove(category);

        if (SelectedFilterCategory == category)
            SelectedFilterCategory = null;

        ApplyFilter();
        UpdateCounters();
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
        Application.Current!.UserAppTheme = IsDarkTheme ? AppTheme.Dark : AppTheme.Light;
    }

    private void ApplyFilter()
    {
        FilteredTasks.Clear();
        
        var source = Tasks.AsEnumerable();

        // Filtrar por categoria
        if (SelectedFilterCategory != null)
            source = source.Where(t => t.CategoryId == SelectedFilterCategory.Id);

        // Filtrar por status
        if (SelectedFilterStatus != null)
            source = source.Where(t => t.Status == SelectedFilterStatus);

        // Ordenar
        source = SelectedSortOption switch
        {
            SortOption.DataCriacao => source.OrderByDescending(t => t.CreatedAt),
            SortOption.DataConclusao => source.OrderByDescending(t => t.CompletedAt ?? DateTime.MaxValue),
            SortOption.Alfabetica => source.OrderBy(t => t.Name),
            _ => source.OrderByDescending(t => t.CreatedAt)
        };

        foreach (var task in source)
            FilteredTasks.Add(task);
    }

    private void UpdateCounters()
    {
        TotalTasks = Tasks.Count;
        CompletedTasks = Tasks.Count(t => t.Status == TodoStatus.Concluido);
        PendingTasks = TotalTasks - CompletedTasks;
    }
}
