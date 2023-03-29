using Microsoft.EntityFrameworkCore;

namespace Demo.MiniProducts.Api.Models;

public class Todo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}

class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
        
    }

    public DbSet<Todo> Todos => Set<Todo>();
}