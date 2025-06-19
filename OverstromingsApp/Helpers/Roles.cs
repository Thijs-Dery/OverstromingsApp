namespace OverstromingsApp.Helpers;

/// <summary>
/// Centrale rol-strings + normalisatiehulpmiddel.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Analist = "Analist";
    public const string Standaard = "Standaard";

    /// <summary>Gebruik steeds dezelfde object-instanties → referentie-gelijk.</summary>
    public static readonly string[] All = { Admin, Analist, Standaard };

    /// <summary>
    /// Mapt willekeurige invoer (“admin ”, “analist”, null) naar een canonieke rol-string.
    /// Onbekend → Standaard.
    /// </summary>
    public static string Normalize(string? value) =>
        value?.Trim().Equals(Admin, StringComparison.OrdinalIgnoreCase) == true ? Admin
      : value?.Trim().Equals(Analist, StringComparison.OrdinalIgnoreCase) == true ? Analist
      : Standaard;
}
