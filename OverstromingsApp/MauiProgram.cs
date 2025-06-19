using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OverstromingsApp.Controllers;
using OverstromingsApp.Core;
using OverstromingsApp.ViewModels;
using OverstromingsApp.Views;

namespace OverstromingsApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        var connectionString = "Server=np:\\\\.\\pipe\\LOCALDB#E56D628E\\tsql\\query;Database=OverstromingsDb;Trusted_Connection=True;";

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // ViewModels
        builder.Services.AddTransient<UserManagementViewModel>();
        builder.Services.AddTransient<LoginViewModel>();

        // Pagina's
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TabelPage>();
        builder.Services.AddTransient<FilterPage>();
        builder.Services.AddTransient<AdminPage>();

        // Controllers / services
        builder.Services.AddSingleton<NeerslagController>();

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
