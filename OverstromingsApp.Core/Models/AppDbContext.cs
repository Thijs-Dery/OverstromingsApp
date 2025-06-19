using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp.Core;

/// <summary>
/// EF Core context.  Bevat een vangnet zodat Role nooit als NULL wegschrijft.
/// </summary>
public class AppDbContext : DbContext
{
    /* DbSets */
    public DbSet<DataModel> Neerslag => Set<DataModel>();
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /* ─── VANGNET voor Role ───────────────────────── */
    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<User>()
                                           .Where(e => e.State is EntityState.Added
                                                    or EntityState.Modified))
        {
            if (string.IsNullOrWhiteSpace(entry.Entity.Role))
                entry.Entity.Role = "Standaard";   // nooit NULL naar SQL
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    /* ─── configuratie + seed ─────────────────────── */
    protected override void OnModelCreating(ModelBuilder b)
    {
        /* composite key voor DataModel */
        b.Entity<DataModel>().HasKey(x => new { x.Jaar, x.Maand });

        /* unieke e-mail */
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();

        /* admin-seed */
        var salt = Convert.FromBase64String("hE7l4xU9WlIlbNKUPRPhkg==");
        var hash = Convert.FromBase64String("Ua8rmmP4oYmNg6YK/X0Hpd2ha08Kjq+A9fKAWIf6eVU=");

        b.Entity<User>().HasData(new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "admin@example.com",
            PasswordSalt = salt,
            PasswordHash = hash,
            Role = "Admin",
            CreatedUtc = DateTime.UtcNow
        });
    }
}
