using Microsoft.EntityFrameworkCore;
using EvoStockAPI.Models;


namespace EvoStockAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}