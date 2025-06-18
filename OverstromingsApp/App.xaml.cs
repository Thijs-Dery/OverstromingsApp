using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    // Nieuw: actieve ingelogde gebruiker
    public static User? CurrentUser { get; set; }

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;

        // database migreren + seeden
        var db = Services.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        _ = SeedDatabaseAsync(db);

        MainPage = new AppShell();
    }

    private static async Task SeedDatabaseAsync(AppDbContext context)
    {
        try
        {
            await Seeder.SeedAsync(context);
            System.Diagnostics.Debug.WriteLine("Database seeded.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Seeding failed: {ex}");
        }
    }
}
