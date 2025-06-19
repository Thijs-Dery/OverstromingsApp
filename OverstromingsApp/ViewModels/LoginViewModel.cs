using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using OverstromingsApp.Helpers;

namespace OverstromingsApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AppDbContext _db;
    public LoginViewModel(AppDbContext db) => _db = db;

    /* ── bindable fields ────────────────────────── */
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    /* ── inloggen ───────────────────────────────── */
    [RelayCommand]
    private async Task SignInAsync()
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == Email);
        if (user is null || !VerifyPassword(Password, user.PasswordSalt, user.PasswordHash))
        {
            await Shell.Current.DisplayAlert("Fout", "E-mail of wachtwoord onjuist.", "OK");
            return;
        }

        /* centrale login-state instellen */
        App.CurrentUser = user;          // indien elders gebruikt
        Auth.CurrentUser = user;          // triggert UserChanged-event

        await Shell.Current.GoToAsync("//TabelPage");
    }

    /* ── uitloggen ─────────────────────────────── */
    [RelayCommand]
    private async Task SignOutAsync()
    {
        App.CurrentUser = null;
        Auth.CurrentUser = null;          // menu & guards updaten

        await Shell.Current.GoToAsync("//LoginPage");
    }

    /* ── helper ────────────────────────────────── */
    private static bool VerifyPassword(string password, byte[] salt, byte[] storedHash)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(
                       password, salt, 100_000,
                       HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }
}
