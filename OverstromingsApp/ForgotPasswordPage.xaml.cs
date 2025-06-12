using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverstromingsApp;

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

        // Hier zou je logica kunnen toevoegen om de aanvraag door te sturen
        await DisplayAlert("Verzonden", $"Er is een aanvraag verstuurd naar {email}", "OK");
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // Gaat terug naar de loginpagina
    }
}


