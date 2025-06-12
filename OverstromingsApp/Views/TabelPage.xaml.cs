using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Data;
using OverstromingsApp.Models;

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
        { "Winter", 300 },
        { "Lente", 250 },
        { "Zomer", 260 },
        { "Herfst", 280 }
    };

        TableGrid.RowDefinitions.Clear();
        TableGrid.Children.Clear();

        int rowIndex = 0;

        foreach (var year in data.OrderBy(y => y.Key))
        {
            TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var yearLabel = new Label
            {
                Text = year.Key.ToString(),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 5,
                Margin = new Thickness(1),
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.White
            };
            Grid.SetRow(yearLabel, rowIndex);
            Grid.SetColumn(yearLabel, 0);
            TableGrid.Children.Add(yearLabel);

            var seasons = new[] { "Winter", "Lente", "Zomer", "Herfst" };

            for (int i = 0; i < seasons.Length; i++)
            {
                var season = seasons[i];
                var list = season switch
                {
                    "Winter" => data[year.Key].Winter,
                    "Lente" => data[year.Key].Lente,
                    "Zomer" => data[year.Key].Zomer,
                    "Herfst" => data[year.Key].Herfst,
                    _ => new List<DataModel>()
                };

                int totaal = list.Sum(m => m.NeerslagMM);

                var cell = new Label
                {
                    Text = totaal.ToString(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = 5,
                    Margin = new Thickness(1),
                    BackgroundColor = GetSeasonColor(season, totaal, thresholds[season]),
                    TextColor = Colors.White
                };

                Grid.SetRow(cell, rowIndex);
                Grid.SetColumn(cell, i + 1);
                TableGrid.Children.Add(cell);
            }

            rowIndex++;
        }
    }


    private Color GetSeasonColor(string season, int totaal, int grens)
    {
        if (totaal >= grens + 10)
            return Colors.Red;
        if (totaal >= grens)
            return Colors.Yellow;
        return Colors.Green;
    }
}