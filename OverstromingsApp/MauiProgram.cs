using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Data;
using OverstromingsApp.Views;
using OverstromingsApp.Controllers;

namespace OverstromingsApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        var config = new ConfigurationBuilder()
            .AddUserSecrets<App>()
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? config["DbConnectionString"]
            ?? throw new InvalidOperationException("Geen geldige connection string gevonden.");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddSingleton<NeerslagController>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TabelPage>();
        builder.Services.AddTransient<FilterPage>();
        builder.Services.AddTransient<AdminPage>();
        builder.Services.AddTransient<ForgotPasswordPage>();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
