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
        string email = emailEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            await DisplayAlert("Fout", "Geef een geldig e-mailadres in.", "OK");
            return;
        }

        // Hier zou je een echte reset-link kunnen genereren / e-mail versturen
        await DisplayAlert("Verzonden", $"Er is een reset-aanvraag verstuurd naar: {email}", "OK");
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
