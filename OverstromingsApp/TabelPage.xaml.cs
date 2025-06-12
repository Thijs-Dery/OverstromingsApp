using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace OverstromingsApp;

public partial class TabelPage : ContentPage
{
    public TabelPage()
    {
        InitializeComponent();
        GenerateTable();
    }

    private void GenerateTable()
    {
        var data = new Dictionary<int, List<int>>()
        {
            { 2005, new List<int>{ 108, 67, 54, 73, 68, 74, 79, 98, 89, 68, 98, 98 } },
            { 2006, new List<int>{ 103, 66, 45, 61, 61, 61, 77, 94, 100, 0, 0, 0 } },
            { 2007, new List<int>{ 104, 62, 56, 64, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2008, new List<int>{ 115, 66, 45, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2009, new List<int>{ 115, 67, 46, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2010, new List<int>{ 118, 63, 54, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2011, new List<int>{ 107, 65, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2012, new List<int>{ 103, 61, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2013, new List<int>{ 111, 66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2014, new List<int>{ 114, 66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2015, new List<int>{ 110, 69, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2016, new List<int>{ 111, 60, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2017, new List<int>{ 106, 66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 2018, new List<int>{ 119, 74, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
        };

        int rowIndex = 1;
        foreach (var year in data)
        {
            TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var yearLabel = new Label
            {
                Text = year.Key.ToString(),
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.FromArgb("#F5F5F5"),
                FontAttributes = FontAttributes.Bold,
                Padding = 5
            };
            Grid.SetRow(yearLabel, rowIndex);
            Grid.SetColumn(yearLabel, 0);
            TableGrid.Children.Add(yearLabel);

            for (int i = 0; i < 12; i++)
            {
                string text = i < year.Value.Count ? year.Value[i].ToString() : "";
                var valueLabel = new Label
                {
                    Text = text,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor = GetColorForValue(text),
                    Padding = 5
                };
                Grid.SetRow(valueLabel, rowIndex);
                Grid.SetColumn(valueLabel, i + 1);
                TableGrid.Children.Add(valueLabel);
            }

            rowIndex++;
        }
    }

    private Color GetColorForValue(string value)
    {
        if (int.TryParse(value, out int intValue))
        {
            if (intValue > 100)
                return Colors.Red;
            if (intValue > 70)
                return Colors.Yellow;
            return Colors.Green;
        }
        return Colors.Transparent;
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
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
   

}
