using Microsoft.Maui.Controls;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OverstromingsApp.Views;

public partial class FilterPage : ContentPage, INotifyPropertyChanged
{
    private readonly AppDbContext _context;
    private bool _isLoading = false;

    public List<int?> Maanden { get; } = new List<int?> { null }
        .Concat(Enumerable.Range(1, 12).Select(m => (int?)m)).ToList();

    public List<int?> Jaartallen { get; set; } = new() { null };

    public List<string> Seizoenen { get; } = new() { "", "Winter", "Lente", "Zomer", "Herfst" };

    private int? _geselecteerdeMaand = null;
    public int? GeselecteerdeMaand
    {
        get => _geselecteerdeMaand;
        set { _geselecteerdeMaand = value; OnPropertyChanged(); _ = LoadGegevensAsync(); }
    }

    private int? _geselecteerdJaar = null;
    public int? GeselecteerdJaar
    {
        get => _geselecteerdJaar;
        set { _geselecteerdJaar = value; OnPropertyChanged(); _ = LoadGegevensAsync(); }
    }

    private string _geselecteerdSeizoen = "";
    public string GeselecteerdSeizoen
    {
        get => _geselecteerdSeizoen;
        set { _geselecteerdSeizoen = value; OnPropertyChanged(); _ = LoadGegevensAsync(); }
    }

    public int MinNeerslag { get; set; } = 0;
    public int MaxNeerslag { get; set; } = 500;

    private double _minFilterNeerslag = 0;
    public double MinFilterNeerslag
    {
        get => _minFilterNeerslag;
        set { _minFilterNeerslag = value; OnPropertyChanged(); OnPropertyChanged(nameof(NeerslagBereikLabel)); _ = LoadGegevensAsync(); }
    }

    private double _maxFilterNeerslag = 500;
    public double MaxFilterNeerslag
    {
        get => _maxFilterNeerslag;
        set { _maxFilterNeerslag = value; OnPropertyChanged(); OnPropertyChanged(nameof(NeerslagBereikLabel)); _ = LoadGegevensAsync(); }
    }

    private bool _sorteerAfdalend = true;
    public bool SorteerAfdalend
    {
        get => _sorteerAfdalend;
        set { _sorteerAfdalend = value; OnPropertyChanged(); _ = LoadGegevensAsync(); }
    }

    public string NeerslagBereikLabel => $"Tussen {MinFilterNeerslag:F0} en {MaxFilterNeerslag:F0} mm";

    public FilterPage(AppDbContext context)
    {
        InitializeComponent();
        _context = context;
        BindingContext = this;

        MinFilterNeerslag = MinNeerslag;
        MaxFilterNeerslag = MaxNeerslag;

        _ = LoadJarenAsync();
        _ = LoadGegevensAsync();
    }

    private async Task LoadJarenAsync()
    {
        var jaren = await _context.Neerslag
            .Select(n => n.Jaar).Distinct().OrderBy(y => y).ToListAsync();

        Jaartallen = new List<int?> { null };
        Jaartallen.AddRange(jaren.Select(j => (int?)j));
        OnPropertyChanged(nameof(Jaartallen));
    }

    private async Task LoadGegevensAsync()
    {
        if (_isLoading || TableContainer == null) return;
        _isLoading = true;

        try
        {
            TableContainer.Children.Clear();

            var data = await _context.Neerslag.ToListAsync();

            var gefilterd = data.Where(d =>
                (!GeselecteerdeMaand.HasValue || d.Maand == GeselecteerdeMaand) &&
                (!GeselecteerdJaar.HasValue || d.Jaar == GeselecteerdJaar) &&
                (string.IsNullOrWhiteSpace(GeselecteerdSeizoen) || d.Seizoen.Equals(GeselecteerdSeizoen, StringComparison.OrdinalIgnoreCase)) &&
                d.NeerslagMM >= MinFilterNeerslag &&
                d.NeerslagMM <= MaxFilterNeerslag);

            gefilterd = SorteerAfdalend
                ? gefilterd.OrderByDescending(d => d.NeerslagMM)
                : gefilterd.OrderBy(d => d.NeerslagMM);

            foreach (var item in gefilterd)
                TableContainer.Children.Add(CreateRij(item));
        }
        finally { _isLoading = false; }
    }

    private View CreateRij(DataModel model) => new HorizontalStackLayout
    {
        Spacing = 15,
        Children =
        {
            new Label { Text = model.Jaar.ToString(), WidthRequest = 60 },
            new Label { Text = model.Maand.ToString(), WidthRequest = 100 },
            new Label { Text = $"{model.NeerslagMM} mm", WidthRequest = 100 }
        }
    };

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        BindingContext = null;
    }

    public new event PropertyChangedEventHandler PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
