using Microsoft.Maui.Controls;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace OverstromingsApp.Views;

public partial class FilterPage : ContentPage, INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private bool _busy;

    public List<string> Maanden { get; } =
        new List<string> { "Toon alles" }.Concat(Enumerable.Range(1, 12).Select(i => i.ToString())).ToList();

    public List<string> Seizoenen { get; } = new() { "Toon alles", "Winter", "Lente", "Zomer", "Herfst" };

    private string _maand = "Toon alles";
    private string _seizoen = "Toon alles";
    private double _min = 0, _max = 500;
    private bool _afdalend = true;
    private bool _sortJaarAflopend = false;

    public string GeselecteerdeMaand
    {
        get => _maand;
        set
        {
            if (_maand != value)
            {
                _maand = value;
                if (value != "Toon alles" && GeselecteerdSeizoen != "Toon alles")
                    GeselecteerdSeizoen = "Toon alles";
                Notify();
            }
        }
    }

    public string GeselecteerdSeizoen
    {
        get => _seizoen;
        set
        {
            if (_seizoen != value)
            {
                _seizoen = value;
                if (value != "Toon alles" && GeselecteerdeMaand != "Toon alles")
                    GeselecteerdeMaand = "Toon alles";
                Notify();
            }
        }
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
        set
        {
            _sortJaarAflopend = value;
            OnPropertyChanged(nameof(SorteerTekst));
            Notify();
        }
    }

    public string SorteerTekst => SorteerOpJaarAflopend ? "Nieuw → Oud" : "Oud → Nieuw";

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
                (GeselecteerdeMaand == "Toon alles" || g.Maand.ToString() == GeselecteerdeMaand) &&
                (GeselecteerdSeizoen == "Toon alles" || g.Seizoen.Equals(GeselecteerdSeizoen, StringComparison.OrdinalIgnoreCase)) &&
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

    private static View MaakRij(DataModel m) =>
        new Frame
        {
            CornerRadius = 10,
            Padding = new Thickness(10),
            Margin = new Thickness(0, 2),
            BackgroundColor = Colors.LightGray,
            Content = new HorizontalStackLayout
            {
                Spacing = 20,
                Children =
                {
                    new Label { Text = m.Jaar.ToString(), WidthRequest = 60, VerticalOptions = LayoutOptions.Center },
                    new Label { Text = m.Maand.ToString(), WidthRequest = 60, VerticalOptions = LayoutOptions.Center },
                    new Label { Text = $"{m.NeerslagMM} mm", WidthRequest = 100, VerticalOptions = LayoutOptions.Center }
                }
            }
        };

    private void Notify([CallerMemberName] string? prop = null, bool onlyLabel = false)
    {
        OnPropertyChanged(prop);
        if (onlyLabel || prop is nameof(MinFilterNeerslag) || prop is nameof(MaxFilterNeerslag))
            OnPropertyChanged(nameof(NeerslagBereikLabel));

        _ = VerversAsync();
    }

    public void ToggleSorteerOpJaar_Clicked(object sender, EventArgs e)
    {
        SorteerOpJaarAflopend = !SorteerOpJaarAflopend;
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
