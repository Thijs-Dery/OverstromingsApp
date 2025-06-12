using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using OverstromingsApp.Data;

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
            await Shell.Current.GoToAsync("///TabelPage");
        }
        else
        {
            await DisplayAlert("Fout", "Database context niet beschikbaar.", "OK");
        }
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ForgotPasswordPage");
    }
}
