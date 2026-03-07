using SQLite;
using TodoApp.Models;

namespace TodoApp.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private const string DbFileName = "todoapp.db3";

    public async Task InitializeAsync()
    {
        if (_database is not null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbFileName);
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<Category>();
        await _database.CreateTableAsync<TaskItem>();
    }

    // Métodos para Categorias
    public async Task<List<Category>> GetCategoriesAsync()
    {
        await InitializeAsync();
        return await _database!.Table<Category>().ToListAsync();
    }

    public async Task<int> SaveCategoryAsync(Category category)
    {
        await InitializeAsync();
        if (category.Id == 0)
            return await _database!.InsertAsync(category);
        else
            return await _database!.UpdateAsync(category);
    }

    public async Task<int> DeleteCategoryAsync(Category category)
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(category);
    }

    // Métodos para Tarefas
    public async Task<List<TaskItem>> GetTasksAsync()
    {
        await InitializeAsync();
        var tasks = await _database!.Table<TaskItem>().ToListAsync();
        
        // Carregar categorias para cada tarefa
        foreach (var task in tasks)
        {
            if (task.CategoryId > 0)
            {
                task.Category = await _database.Table<Category>()
                    .Where(c => c.Id == task.CategoryId)
                    .FirstOrDefaultAsync();
            }
        }
        
        return tasks;
    }

    public async Task<int> SaveTaskAsync(TaskItem task)
    {
        await InitializeAsync();
        if (task.Id == 0)
            return await _database!.InsertAsync(task);
        else
            return await _database!.UpdateAsync(task);
    }

    public async Task<int> DeleteTaskAsync(TaskItem task)
    {
        await InitializeAsync();
        return await _database!.DeleteAsync(task);
    }

    public async Task<List<TaskItem>> GetTasksByCategoryAsync(int categoryId)
    {
        await InitializeAsync();
        return await _database!.Table<TaskItem>()
            .Where(t => t.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetTasksByStatusAsync(TaskStatus status)
    {
        await InitializeAsync();
        return await _database!.Table<TaskItem>()
            .Where(t => t.Status == status)
            .ToListAsync();
    }
}
