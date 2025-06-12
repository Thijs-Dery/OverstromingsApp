using Microsoft.Maui.Controls;

namespace OverstromingsApp.Views;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnRequestClicked(object sender, EventArgs e)
    {
        string email = emailEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            await DisplayAlert("Fout", "Geef een geldig e-mailadres in.", "OK");
            return;
        }

        await DisplayAlert("Verzonden", $"Er is een aanvraag verstuurd naar {email}", "OK");
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("LoginPage");
    }
}
