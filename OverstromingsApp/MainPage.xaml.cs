using Microsoft.Maui.Controls;

namespace OverstromingsApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Simpele navigatie zonder loginvalidatie
        await Navigation.PushAsync(new TabelPage());
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage());
    }

}

