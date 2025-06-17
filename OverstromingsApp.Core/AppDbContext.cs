using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp.Core;

public class AppDbContext : DbContext
{
    public DbSet<DataModel> Neerslag { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataModel>().HasKey(x => new { x.Jaar, x.Maand });
    }
}
