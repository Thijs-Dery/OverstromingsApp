using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using OverstromingsApp.ViewModels;

namespace OverstromingsApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = App.Services.GetRequiredService<LoginViewModel>();
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("ForgotPasswordPage");
    }
}
