using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using OverstromingsApp.Helpers;

namespace OverstromingsApp.Views;

public partial class FilterPage : ContentPage, INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private bool _busy;

    /* ─── combobox-opties ───────────────────────── */
    public List<string> Maanden { get; } =
        new List<string> { "Toon alles" }
        .Concat(Enumerable.Range(1, 12).Select(i => i.ToString()))
        .ToList();

    public List<string> Seizoenen { get; } =
        new List<string> { "Toon alles", "Winter", "Lente", "Zomer", "Herfst" };

    /* ─── filter-state ─────────────────────────── */
    private string _maand = "Toon alles";
    private string _seizoen = "Toon alles";
    private double _min = 0;
    private double _max = 500;
    private bool _afdalend = true;
    private bool _sortJaarDesc = false;

    public string GeselecteerdeMaand
    {
        get => _maand;
        set
        {
            if (Set(ref _maand, value))
            {
                if (value != "Toon alles" && GeselecteerdSeizoen != "Toon alles")
                    GeselecteerdSeizoen = "Toon alles";
                _ = VerversAsync();
            }
        }
    }

    public string GeselecteerdSeizoen
    {
        get => _seizoen;
        set
        {
            if (Set(ref _seizoen, value))
            {
                if (value != "Toon alles" && GeselecteerdeMaand != "Toon alles")
                    GeselecteerdeMaand = "Toon alles";
                _ = VerversAsync();
            }
        }
    }

    public int MinNeerslag => 0;
    public int MaxNeerslag => 500;

    public double MinFilterNeerslag
    {
        get => _min;
        set { if (Set(ref _min, value)) _ = VerversAsync(); }
    }

    public double MaxFilterNeerslag
    {
        get => _max;
        set { if (Set(ref _max, value)) _ = VerversAsync(); }
    }

    public bool SorteerAfdalend
    {
        get => _afdalend;
        set { if (Set(ref _afdalend, value)) _ = VerversAsync(); }
    }

    public bool SorteerOpJaarAflopend
    {
        get => _sortJaarDesc;
        set
        {
            if (Set(ref _sortJaarDesc, value))
            {
                OnPropertyChanged(nameof(SorteerTekst));
                _ = VerversAsync();
            }
        }
    }

    public string SorteerTekst => SorteerOpJaarAflopend ? "Nieuw → Oud" : "Oud → Nieuw";

    public string NeerslagBereikLabel =>
        $"Tussen {MinFilterNeerslag:F0} en {MaxFilterNeerslag:F0} mm";

    /* ─── ctor ─────────────────────────────────── */
    public FilterPage(AppDbContext context)
    {
        InitializeComponent();
        _db = context;
        BindingContext = this;
        _ = VerversAsync();
    }

    /* ─── data verversen ───────────────────────── */
    private async Task VerversAsync()
    {
        if (_busy || TableContainer is null) return;
        _busy = true;

        try
        {
            TableContainer.Children.Clear();

            var data = await _db.Neerslag.AsNoTracking().ToListAsync();

            var q = data.Where(g =>
                (GeselecteerdeMaand == "Toon alles" || g.Maand.ToString() == GeselecteerdeMaand) &&
                (GeselecteerdSeizoen == "Toon alles" || g.Seizoen.Equals(GeselecteerdSeizoen, StringComparison.OrdinalIgnoreCase)) &&
                g.NeerslagMM >= MinFilterNeerslag &&
                g.NeerslagMM <= MaxFilterNeerslag);

            IOrderedEnumerable<DataModel> sorted = SorteerOpJaarAflopend
                ? q.OrderByDescending(g => g.Jaar)
                : q.OrderBy(g => g.Jaar);

            sorted = SorteerAfdalend
                ? sorted.ThenByDescending(g => g.NeerslagMM)
                : sorted.ThenBy(g => g.NeerslagMM);

            foreach (var g in sorted)
                TableContainer.Children.Add(MaakRij(g));
        }
        finally { _busy = false; }
    }

    /* ─── rij UI + verwijderknop ───────────────── */
    private View MaakRij(DataModel m)
    {
        var grid = new Grid
        {
            ColumnSpacing = 10,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 70              },
                new ColumnDefinition { Width = 50              },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        grid.Add(new Label
        {
            Text = m.Jaar.ToString(),
            VerticalOptions = LayoutOptions.Center
        }, 0, 0);

        grid.Add(new Label
        {
            Text = m.Maand.ToString(),
            VerticalOptions = LayoutOptions.Center
        }, 1, 0);

        grid.Add(new Label
        {
            Text = $"{m.NeerslagMM} mm",
            VerticalOptions = LayoutOptions.Center
        }, 2, 0);

        if (Auth.IsAdmin || Auth.IsAnalist)
        {
            var btn = new Button
            {
                Text = "Verwijder",
                TextColor = Colors.Red,
                BackgroundColor = Colors.Transparent,
                Padding = 0,
                WidthRequest = 80,
                HorizontalOptions = LayoutOptions.End
            };
            btn.Clicked += async (_, _) => await VerwijderAsync(m);
            grid.Add(btn, 3, 0);
        }

        return new Frame
        {
            CornerRadius = 10,
            Padding = new Thickness(10),
            Margin = new Thickness(0, 2),
            BackgroundColor = Colors.LightGray,
            Content = grid
        };
    }

    /* ─── verwijderen uit DB & UI ──────────────── */
    private async Task VerwijderAsync(DataModel m)
    {
        bool ok = await DisplayAlert("Bevestig",
                   $"Record {m.Jaar}-{m.Maand}: {m.NeerslagMM} mm verwijderen?",
                   "Ja", "Nee");
        if (!ok) return;

        var tracked = _db.Neerslag.Local
                        .FirstOrDefault(e => e.Jaar == m.Jaar && e.Maand == m.Maand);

        if (tracked is null)
        {
            tracked = new DataModel { Jaar = m.Jaar, Maand = m.Maand };
            _db.Neerslag.Attach(tracked);
        }

        _db.Neerslag.Remove(tracked);
        await _db.SaveChangesAsync();
        await VerversAsync();
    }

    /* ─── helper PropertyChanged ───────────────── */
    private bool Set<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(name);
        if (name is nameof(MinFilterNeerslag) or nameof(MaxFilterNeerslag))
            OnPropertyChanged(nameof(NeerslagBereikLabel));
        return true;
    }

    public void ToggleSorteerOpJaar_Clicked(object sender, EventArgs e)
        => SorteerOpJaarAflopend = !SorteerOpJaarAflopend;

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
