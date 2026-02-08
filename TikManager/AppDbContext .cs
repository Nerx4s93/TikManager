using Microsoft.EntityFrameworkCore;
using TikManager.Models;

namespace TikManager;

public class AppDbContext  : DbContext
{
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Proxy> Proxies { get; set; } = null!;

    public AppDbContext ()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=app.db");
    }
}