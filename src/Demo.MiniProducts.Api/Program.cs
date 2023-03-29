using Demo.MiniProducts.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoContext>(
    optionsBuilder => optionsBuilder.UseInMemoryDatabase("ToDoDatabase")
);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/todos", async (TodoContext context) => await context.Todos.ToListAsync());

app.MapGet(
    "/todos/completed",
    async (TodoContext context) => await context.Todos.Where(x => x.IsComplete).ToListAsync()
);

app.MapGet(
    "/todos/{id}",
    async (int id, TodoContext context) => await context.Todos.Where(x => x.Id == id).ToListAsync()
);

app.MapPost(
    "/todos",
    async (Todo todo, TodoContext context) =>
    {
        await context.Todos.AddAsync(todo);
        await context.SaveChangesAsync();

        return Results.Created($"/todos/{todo.Id}", todo);
    }
);

app.MapPut(
    "/todos/{id}",
    async (int id, Todo updated, TodoContext context) =>
    {
        var product = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
        if (product == null)
        {
            return Results.NotFound();
        }

        product.IsComplete = updated.IsComplete;
        product.Name = updated.Name;

        await context.SaveChangesAsync();
        return Results.Accepted($"/todos/{id}", product);
    }
);

app.Run();
