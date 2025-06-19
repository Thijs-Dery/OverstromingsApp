using OverstromingsApp.Helpers;
using Microsoft.Maui.Controls;


namespace OverstromingsApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("AdminPage", typeof(Views.AdminPage));
        Routing.RegisterRoute("TabelPage", typeof(Views.TabelPage));
        Routing.RegisterRoute("FilterPage", typeof(Views.FilterPage));
        Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
        Routing.RegisterRoute("GrafiekPage", typeof(Views.GrafiekPage));

        Navigating += OnNavigating;
    }

    private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        if (e.Target.Location.OriginalString.Contains("AdminPage") && !Auth.IsAdmin)
        {
            e.Cancel();
            await Shell.Current.DisplayAlert("Toegang geweigerd", "Je hebt geen toegang tot deze pagina.", "OK");
        }
    }
}
