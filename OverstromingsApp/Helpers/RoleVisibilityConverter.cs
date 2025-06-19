using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace OverstromingsApp.Helpers;

/// <summary>
/// Maakt een UI-element zichtbaar wanneer de ingelogde gebruiker
/// Admin of Analist is; anders onzichtbaar.
/// </summary>
public class RoleVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
        => Auth.IsAdmin || Auth.IsAnalist;         // true → zichtbaar

    public object ConvertBack(object v, Type t, object p, CultureInfo c)
        => throw new NotImplementedException();
}
