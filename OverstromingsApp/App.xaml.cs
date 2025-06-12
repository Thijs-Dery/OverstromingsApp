using OverstromingsApp.Data;

namespace OverstromingsApp
{
    public partial class App : Application
    {
        public App(AppDbContext context)
        {
            InitializeComponent();
            MainPage = new AppShell();

            _ = SeedDatabaseAsync(context);
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
}
