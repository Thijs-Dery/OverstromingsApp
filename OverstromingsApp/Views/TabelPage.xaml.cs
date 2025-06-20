using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Mvvm.Messaging;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using OverstromingsApp.Helpers;    // voor NeerslagChangedMessage

namespace OverstromingsApp.Views;

/// <summary>
/// Toont per jaar de totale neerslag per seizoen.
/// Ververst automatisch zodra NeerslagChangedMessage ontvangen wordt.
/// </summary>
public partial class TabelPage : ContentPage,
                                 IRecipient<NeerslagChangedMessage>
{
    private readonly AppDbContext _context;
    private bool _busy;

    // Hulp-DTO om per jaar de seizoenslijsten te bewaren
    private sealed class JaarSeizoensGegevens
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
    }

    // ── Lifecycle: begin/stop luisteren ─────────
    protected override void OnAppearing()
    {
        base.OnAppearing();
        WeakReferenceMessenger.Default.Register(this);
        _ = LoadAndBuildTableAsync();
    }

    protected override void OnDisappearing()
    {
        WeakReferenceMessenger.Default.Unregister<NeerslagChangedMessage>(this);
        base.OnDisappearing();
    }

    // ── IRecipient-handler: NIET expression-bodied! ──
    public void Receive(NeerslagChangedMessage message)
    {
        // Herlaad de tabel, fire-and-forget
        _ = LoadAndBuildTableAsync();
    }

    // ── Data ophalen & groeperen ─────────────────────
    private async Task LoadAndBuildTableAsync()
    {
        if (_busy || TableGrid is null)
            return;

        _busy = true;
        try
        {
            var records = await _context.Neerslag
                                        .AsNoTracking()
                                        .OrderBy(r => r.Jaar)
                                        .ThenBy(r => r.Maand)
                                        .ToListAsync();

            var grouped = records
                .GroupBy(r => r.Jaar)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var dto = new JaarSeizoensGegevens
                        {
                            Winter = g.Where(x => x.Seizoen == "Winter").ToList(),
                            Lente = g.Where(x => x.Seizoen == "Lente").ToList(),
                            Zomer = g.Where(x => x.Seizoen == "Zomer").ToList(),
                            Herfst = g.Where(x => x.Seizoen == "Herfst").ToList(),
                        };

                        // voeg vorige december toe aan winter
                        var decPrev = records.Where(x => x.Jaar == g.Key - 1 && x.Maand == 12);
                        dto.Winter = decPrev.Concat(dto.Winter).ToList();

                        return dto;
                    });

            BuildSeasonTable(grouped);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FOUT tijdens laden: {ex}");
        }
        finally
        {
            _busy = false;
        }
    }

    // ── Tabel opbouwen in Grid ─────────────────────
    private void BuildSeasonTable(Dictionary<int, JaarSeizoensGegevens> data)
    {
        var grens = new Dictionary<string, int>
        {
            { "Winter", 300 }, { "Lente", 250 },
            { "Zomer",  260 }, { "Herfst", 280 }
        };

        TableGrid.RowDefinitions.Clear();
        TableGrid.Children.Clear();

        int row = 0;
        foreach (var entry in data.OrderBy(e => e.Key))
        {
            TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AddCell(entry.Key.ToString(), row, 0, Colors.Transparent, bold: true);

            foreach (var (seizoen, col) in new[] { "Winter", "Lente", "Zomer", "Herfst" }
                                            .Select((s, i) => (s, i + 1)))
            {
                var lijst = seizoen switch
                {
                    "Winter" => entry.Value.Winter,
                    "Lente" => entry.Value.Lente,
                    "Zomer" => entry.Value.Zomer,
                    _ => entry.Value.Herfst
                };

                int totaal = lijst.Sum(x => x.NeerslagMM);
                var kleur = KleurVoorSeizoen(seizoen, totaal, grens[seizoen]);
                AddCell(totaal.ToString(), row, col, kleur);
            }

            row++;
        }
    }

    // ── Helper voor één cel ─────────────────────────
    private void AddCell(string text, int row, int col, Color bg, bool bold = false)
    {
        var lbl = new Label
        {
            Text = text,
            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            Padding = 5,
            Margin = new Thickness(1),
            BackgroundColor = bg,
            TextColor = bg == Colors.Transparent ? Colors.Black : Colors.White
        };

        Grid.SetRow(lbl, row);
        Grid.SetColumn(lbl, col);
        TableGrid.Children.Add(lbl);
    }


    private static Color KleurVoorSeizoen(string seizoen, int totaal, int grens) =>
        totaal >= grens + 10 ? Colors.Red :
        totaal >= grens ? Colors.Yellow :
                                Colors.Green;
}
