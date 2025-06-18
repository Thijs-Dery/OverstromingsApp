using System;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp.Helpers;

/// <summary>
/// Helpers voor gebruikersauthenticatie en toegangscontrole.
/// </summary>
public static class Auth
{
    // Admin-ID zoals geseed
    private static readonly Guid AdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>
    /// True als er een gebruiker is ingelogd.
    /// </summary>
    public static bool IsLoggedIn => App.CurrentUser is not null;

    /// <summary>
    /// True als de ingelogde gebruiker de admin is.
    /// </summary>
    public static bool IsAdmin =>
        App.CurrentUser?.Id == AdminId;
}
