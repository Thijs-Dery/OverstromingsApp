using Microsoft.Maui.Controls;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq; // <-- Belangrijk

namespace OverstromingsApp.Views;

public partial class FilterPage : ContentPage, INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private bool _busy;

    public List<int?> Maanden { get; } =
        new List<int?> { null }.Concat(Enumerable.Range(1, 12).Select(i => (int?)i)).ToList();

    public List<string> Seizoenen { get; } = new() { "", "Winter", "Lente", "Zomer", "Herfst" };

    private int? _maand;
    private string _seizoen = "";
    private double _min = 0, _max = 500;
    private bool _afdalend = true;
    private bool _sortJaarAflopend = true;

    public int? GeselecteerdeMaand
    {
        get => _maand;
        set { _maand = value; Notify(); }
    }

    public string GeselecteerdSeizoen
    {
        get => _seizoen;
        set { _seizoen = value; Notify(); }
    }

    public int MinNeerslag { get; } = 0;
    public int MaxNeerslag { get; } = 500;

    public double MinFilterNeerslag
    {
        get => _min;
        set { _min = value; Notify(nameof(MinFilterNeerslag), true); }
    }

    public double MaxFilterNeerslag
    {
        get => _max;
        set { _max = value; Notify(nameof(MaxFilterNeerslag), true); }
    }

    public bool SorteerAfdalend
    {
        get => _afdalend;
        set { _afdalend = value; Notify(); }
    }

    public bool SorteerOpJaarAflopend
    {
        get => _sortJaarAflopend;
        set { _sortJaarAflopend = value; Notify(); }
    }

    public string NeerslagBereikLabel =>
        $"Tussen {MinFilterNeerslag:F0} en {MaxFilterNeerslag:F0} mm";

    public FilterPage(AppDbContext context)
    {
        InitializeComponent();
        _db = context;
        BindingContext = this;
        _ = VerversAsync();
    }

    private async Task VerversAsync()
    {
        if (_busy || TableContainer == null) return;
        _busy = true;

        try
        {
            TableContainer.Children.Clear();

            var data = await _db.Neerslag.AsNoTracking().ToListAsync();

            var query = data.Where(g =>
                (!GeselecteerdeMaand.HasValue || g.Maand == GeselecteerdeMaand) &&
                (string.IsNullOrWhiteSpace(GeselecteerdSeizoen) ||
                 g.Seizoen.Equals(GeselecteerdSeizoen, StringComparison.OrdinalIgnoreCase)) &&
                g.NeerslagMM >= MinFilterNeerslag &&
                g.NeerslagMM <= MaxFilterNeerslag);

            IOrderedEnumerable<DataModel> orderedQuery = SorteerOpJaarAflopend
                ? query.OrderByDescending(g => g.Jaar)
                : query.OrderBy(g => g.Jaar);

            orderedQuery = SorteerAfdalend
                ? orderedQuery.ThenByDescending(g => g.NeerslagMM)
                : orderedQuery.ThenBy(g => g.NeerslagMM);

            foreach (var g in orderedQuery)
                TableContainer.Children.Add(MaakRij(g));
        }
        finally { _busy = false; }
    }



    private static View MaakRij(DataModel m) => new HorizontalStackLayout
    {
        Spacing = 15,
        Children =
        {
            new Label { Text = m.Jaar.ToString(),    WidthRequest = 60 },
            new Label { Text = m.Maand.ToString(),   WidthRequest = 90 },
            new Label { Text = $"{m.NeerslagMM} mm", WidthRequest = 100 }
        }
    };

    private void Notify([CallerMemberName] string? prop = null, bool onlyLabel = false)
    {
        OnPropertyChanged(prop);
        if (onlyLabel || prop is nameof(MinFilterNeerslag) || prop is nameof(MaxFilterNeerslag))
            OnPropertyChanged(nameof(NeerslagBereikLabel));

        _ = VerversAsync();
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
