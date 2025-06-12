using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace OverstromingsApp.Views;

public partial class FilterPage : ContentPage
{
    public FilterPage()
    {
        InitializeComponent();
        GenerateTable();
    }

    private void GenerateTable()
    {
        var data = new List<NeerslagItem>
        {
            new NeerslagItem { Jaar = 2005, Maand = "Dec", Neerslag = 108 },
            new NeerslagItem { Jaar = 2006, Maand = "Jan", Neerslag = 66 },
            new NeerslagItem { Jaar = 2006, Maand = "Feb", Neerslag = 45 },
            new NeerslagItem { Jaar = 2007, Maand = "Jan", Neerslag = 62 },
            new NeerslagItem { Jaar = 2008, Maand = "Jan", Neerslag = 66 },
            new NeerslagItem { Jaar = 2012, Maand = "Dec", Neerslag = 103 },
        };

        int row = 0;

        TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        AddToGrid(CreateCell("Jaar", true), 0, row);
        AddToGrid(CreateCell("Maand", true), 1, row);
        AddToGrid(CreateCell("Neerslag", true), 2, row);
        row++;

        foreach (var item in data)
        {
            TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AddToGrid(CreateCell(item.Jaar.ToString()), 0, row);
            AddToGrid(CreateCell(item.Maand), 1, row);
            AddToGrid(CreateCell(item.Neerslag.ToString()), 2, row);
            row++;
        }
    }

    private Label CreateCell(string text, bool isHeader = false)
    {
        return new Label
        {
            Text = text,
            BackgroundColor = isHeader ? Colors.LightGray : Colors.Transparent,
            FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            Padding = 5,
            Margin = 1
        };
    }

    private void AddToGrid(View view, int column, int row)
    {
        Grid.SetColumn(view, column);
        Grid.SetRow(view, row);
        TableGrid.Children.Add(view);
    }

    public class NeerslagItem
    {
        public int Jaar { get; set; }
        public string Maand { get; set; }
        public int Neerslag { get; set; }
    }
}
