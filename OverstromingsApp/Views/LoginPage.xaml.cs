using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using OverstromingsApp.Data;
using OverstromingsApp.Views;

namespace OverstromingsApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var dbContext = App.Services.GetService<AppDbContext>();
        if (dbContext is not null)
        {
            await Navigation.PushAsync(new TabelPage(dbContext));
        }
        else
        {
            await DisplayAlert("Fout", "Database context niet beschikbaar.", "OK");
        }
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage());
    }
}