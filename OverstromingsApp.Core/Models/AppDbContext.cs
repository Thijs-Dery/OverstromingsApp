using OverstromingsApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace OverstromingsApp.Core;

public class AppDbContext : DbContext
{
    public DbSet<DataModel> Neerslag { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!; 

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        // bestaande composite key
        b.Entity<DataModel>().HasKey(x => new { x.Jaar, x.Maand });

        // unieke email-index
        b.Entity<User>()
         .HasIndex(u => u.Email)
         .IsUnique();

        // ===== seed admin-account =====
        var salt = Convert.FromBase64String("hE7l4xU9WlIlbNKUPRPhkg==");
        var hash = Convert.FromBase64String("Ua8rmmP4oYmNg6YK/X0Hpd2ha08Kjq+A9fKAWIf6eVU=");

        b.Entity<User>().HasData(new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "admin@example.com",
            PasswordSalt = salt,
            PasswordHash = hash,
            CreatedUtc = DateTime.UtcNow
        });
    }
}
