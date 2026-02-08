using Microsoft.EntityFrameworkCore;
using TikManager.Models;

namespace TikManager;

public class SessionDbContext : DbContext
{
    public DbSet<Session> Sessions { get; set; } = null!;

    public SessionDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=sessions.db");
    }
}