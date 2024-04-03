using Mango.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    public DbSet<ShoppingCartHeader> Headers { get; set; }
    public DbSet<ShoppingCartDetail> Details { get; set; }
}
