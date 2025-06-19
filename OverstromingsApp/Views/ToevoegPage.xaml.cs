using Microsoft.Maui.Controls;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using System;
using System.Globalization;
using System.Linq;

namespace OverstromingsApp.Views
{
    public partial class ToevoegPage : ContentPage
    {
        /* parameterloze ctor voor Shell */
        public ToevoegPage()
            : this(App.Current!.Handler!.MauiContext!.Services
                       .GetRequiredService<AppDbContext>())
        { }

        private readonly AppDbContext _context;

        public ToevoegPage(AppDbContext context)
        {
            InitializeComponent();
            _context = context;

            /* vul picker met lokale maandnamen */
            var months = Enumerable.Range(1, 12)
                                   .Select(i => CultureInfo.CurrentCulture
                                                         .DateTimeFormat
                                                         .MonthNames[i - 1])
                                   .ToList();
            foreach (var m in months) MonthPicker.Items.Add(m);

            MonthPicker.SelectedIndexChanged += (_, __) => UpdateSeasonLabel();
        }

        /* seizoen alleen voor weergave */
        private static string GetSeason(int month) => month switch
        {
            12 or 1 or 2 => "Winter",
            3 or 4 or 5 => "Lente",
            6 or 7 or 8 => "Zomer",
            _ => "Herfst"
        };

        private void UpdateSeasonLabel()
        {
            SeasonLabel.Text = MonthPicker.SelectedIndex switch
            {
                -1 => "",
                var i => $"Seizoen: {GetSeason(i + 1)}"
            };
        }

        /* Opslaan */
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            /* basisvalidatie */
            if (!int.TryParse(YearEntry.Text, out int jaar) ||
                jaar < 1900 || jaar > DateTime.Now.Year + 1)
            {
                await DisplayAlert("Fout", "Voer een geldig jaar in.", "OK");
                return;
            }

            if (MonthPicker.SelectedIndex is < 0 or > 11)
            {
                await DisplayAlert("Fout", "Kies een maand.", "OK");
                return;
            }
            int maand = MonthPicker.SelectedIndex + 1;

            if (!int.TryParse(RainEntry.Text, out int mm) || mm < 0)
            {
                await DisplayAlert("Fout", "Voer een positief getal voor neerslag in.", "OK");
                return;
            }

            /* maak en voeg toe – géén Seizoen-setter nodig */
            var nieuw = new DataModel
            {
                Jaar = jaar,
                Maand = maand,
                NeerslagMM = mm
                // Seizoen wordt automatisch bepaald door DataModel
            };

            try
            {
                _context.Neerslag.Add(nieuw);
                await _context.SaveChangesAsync();

                await DisplayAlert("Gelukt", "Gegevens opgeslagen.", "OK");
                RainEntry.Text = "";          // laat jaar & maand staan
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Opslaan mislukt: {ex.Message}", "OK");
            }
        }
    }
}
