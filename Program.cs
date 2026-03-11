using Microsoft.EntityFrameworkCore;
using EvoStockAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=EvoStock.db"));
builder.Services.AddEndpointsApiExplorer();  // ← required
builder.Services.AddSwaggerGen();             // ← required

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        // ← required
    app.UseSwaggerUI();      // ← required
}

app.MapControllers();
app.Run();