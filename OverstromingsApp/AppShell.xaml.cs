using OverstromingsApp.Helpers;

namespace OverstromingsApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();

        /* eerste render + automatisch verversen via event */
        SetFlyoutVisibility();
        Auth.UserChanged += (_, _) => SetFlyoutVisibility();

        Navigating += OnNavigating;
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("ToevoegPage", typeof(Views.ToevoegPage));
        Routing.RegisterRoute("AdminPage", typeof(Views.AdminPage));
        Routing.RegisterRoute("TabelPage", typeof(Views.TabelPage));
        Routing.RegisterRoute("FilterPage", typeof(Views.FilterPage));
        Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
        Routing.RegisterRoute("GrafiekPage", typeof(Views.GrafiekPage));
    }

    /* — menu-items tonen / verbergen — */
    private void SetFlyoutVisibility()
    {
        ToevoegFlyout.IsVisible = Auth.IsAdmin || Auth.IsAnalist; // admin+analist
        AdminFlyout.IsVisible = Auth.IsAdmin;                   // enkel admin
    }

    /* — navigatiebeveiliging — */
    private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        string url = e.Target.Location.OriginalString;

        if (url.Contains("AdminPage") && !Auth.IsAdmin)
        {
            e.Cancel();
            await DisplayAlert("Toegang geweigerd",
                               "Alleen admins mogen deze pagina openen.", "OK");
        }
        else if (url.Contains("ToevoegPage") && !(Auth.IsAdmin || Auth.IsAnalist))
        {
            e.Cancel();
            await DisplayAlert("Toegang geweigerd",
                               "Alleen admins of analisten mogen deze pagina openen.", "OK");
        }
    }
}
