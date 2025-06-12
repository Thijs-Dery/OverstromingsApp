using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Data;
using OverstromingsApp.Models;
using OverstromingsApp.Views;

namespace OverstromingsApp.Views;

public partial class TabelPage : ContentPage
{
    private readonly AppDbContext _context;

    public class JaarSeizoensGegevens
    {
        public List<DataModel> Winter { get; set; } = new();
        public List<DataModel> Lente { get; set; } = new();
        public List<DataModel> Zomer { get; set; } = new();
        public List<DataModel> Herfst { get; set; } = new();
    }

    public TabelPage(AppDbContext context)
    {
        InitializeComponent();
        _context = context;
        _ = LoadAndBuildTableAsync();
    }

    private async Task LoadAndBuildTableAsync()
    {
        try
        {
            var records = await _context.Neerslag
                .OrderBy(n => n.Jaar)
                .ThenBy(n => n.Maand)
                .ToListAsync();

            var grouped = records
                .GroupBy(r => r.Jaar)
                .ToDictionary(g => g.Key, g =>
                {
                    var model = new JaarSeizoensGegevens
                    {
                        Lente = g.Where(m => m.Seizoen == "Lente").ToList(),
                        Zomer = g.Where(m => m.Seizoen == "Zomer").ToList(),
                        Herfst = g.Where(m => m.Seizoen == "Herfst").ToList(),
                        Winter = g.Where(m => m.Seizoen == "Winter").ToList()
                    };

                    var vorigeDecember = records
                        .Where(m => m.Jaar == g.Key - 1 && m.Maand == 12)
                        .ToList();

                    model.Winter = vorigeDecember.Concat(model.Winter).ToList();

                    return model;
                });

            BuildSeasonTable(grouped);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FOUT bij laden van data: {ex.Message}");
        }
    }

    private void BuildSeasonTable(Dictionary<int, JaarSeizoensGegevens> data)
    {
        var thresholds = new Dictionary<string, int>
        {
            {"Winter", 300},
            {"Lente", 250},
            {"Zomer", 260},
            {"Herfst", 280}
        };

        int rowIndex = 1;

        foreach (var year in data.OrderBy(y => y.Key))
        {
            TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var yearLabel = new Label
            {
                Text = year.Key.ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                Padding = 5
            };
            Grid.SetRow(yearLabel, rowIndex);
            Grid.SetColumn(yearLabel, 0);
            TableGrid.Children.Add(yearLabel);

            var seasons = new[] { "Winter", "Lente", "Zomer", "Herfst" };

            for (int i = 0; i < seasons.Length; i++)
            {
                string season = seasons[i];
                List<DataModel> list = season switch
                {
                    "Winter" => data[year.Key].Winter,
                    "Lente" => data[year.Key].Lente,
                    "Zomer" => data[year.Key].Zomer,
                    "Herfst" => data[year.Key].Herfst,
                    _ => new List<DataModel>()
                };

                int totaal = list.Sum(m => m.NeerslagMM);

                var label = new Label
                {
                    Text = totaal.ToString(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor = GetSeasonColor(season, totaal, thresholds[season]),
                    Padding = 5
                };

                Grid.SetRow(label, rowIndex);
                Grid.SetColumn(label, i + 1);
                TableGrid.Children.Add(label);
            }

            rowIndex++;
        }
    }

    private Color GetSeasonColor(string season, int totaal, int grens)
    {
        int verschil = totaal - grens;

        if (verschil > 10)
            return Colors.Red;
        if (verschil > 0)
            return Colors.Yellow;
        return Colors.Green;
    }

    private async void OnFilterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilterPage());
    }

    private async void OnAdminClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminPage());
    }

    private async void OnUserClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Uitloggen", "Ben je zeker dat je wilt uitloggen?", "Ja", "Nee");
        if (confirm)
        {
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
