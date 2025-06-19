using System;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp.Helpers;

/// <summary>
/// Centrales helper voor authenticatie/autorisation.
/// Roept het <see cref="UserChanged"/>-event wanneer CurrentUser wijzigt.
/// </summary>
public static class Auth
{
    private static readonly Guid AdminId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static event EventHandler? UserChanged;

    /* — centraal ingelogde gebruiker — */
    private static User? _current;
    public static User? CurrentUser
    {
        get => _current;
        set
        {
            _current = value;
            UserChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    public static bool IsLoggedIn => CurrentUser is not null;

    public static bool IsAdmin =>
        CurrentUser?.Id == AdminId ||
        CurrentUser?.Role?.Trim()
                    .Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;

    public static bool IsAnalist =>
        !IsAdmin &&
        CurrentUser?.Role?.Trim()
                    .Equals("Analist", StringComparison.OrdinalIgnoreCase) == true;
}
