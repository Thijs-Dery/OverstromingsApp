using OverstromingsApp.Core;
using Microsoft.Extensions.DependencyInjection;

namespace OverstromingsApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;

        var context = Services.GetRequiredService<AppDbContext>();
        _ = SeedDatabaseAsync(context);

        MainPage = new AppShell();
    }

    private async Task SeedDatabaseAsync(AppDbContext context)
    {
        try
        {
            await Seeder.SeedAsync(context);
            System.Diagnostics.Debug.WriteLine("Database seeded.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Seeding failed: {ex.Message}");
        }
    }
}
