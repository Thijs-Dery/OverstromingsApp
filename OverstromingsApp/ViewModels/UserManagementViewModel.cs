using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using OverstromingsApp.Helpers;

namespace OverstromingsApp.ViewModels;

/// <summary>
/// ViewModel voor het beheren van gebruikers (admin-pagina).
/// </summary>
public partial class UserManagementViewModel : ObservableObject
{
    private readonly AppDbContext _db;
    private static readonly Guid AdminId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public ObservableCollection<User> Users { get; } = new();

    /// <summary>Rollen-lijst voor de Picker (Admin, Analist, Standaard).</summary>
    public ObservableCollection<string> Rollen { get; } =
        new(Roles.All);

    /* ── bindable velden ────────────────────────── */
    [ObservableProperty] private string _zoekterm = string.Empty;
    [ObservableProperty] private string _nieuwEmail = string.Empty;
    [ObservableProperty] private string _nieuwWachtwoord = string.Empty;

    public UserManagementViewModel(AppDbContext db)
    {
        _db = db;
        _ = LoadUsersAsync();      // init-load
    }

    /* ── lijst opnieuw laden ───────────────────── */
    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        Users.Clear();

        var query = _db.Users
                       .Where(u => u.Id != AdminId && u.Role != Roles.Admin);

        if (!string.IsNullOrWhiteSpace(Zoekterm))
            query = query.Where(u => u.Email.Contains(Zoekterm));

        var list = await query.OrderBy(u => u.Email).ToListAsync();

        foreach (var u in list)
        {
            u.Role = Roles.Normalize(u.Role);     // ★ canonieke instantie
            Users.Add(u);
        }
    }

    /* ── gebruiker verwijderen ─────────────────── */
    [RelayCommand]
    private async Task DeleteUserAsync(User user)
    {
        if (user is null) return;
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        await LoadUsersAsync();
    }

    /* ── rol wijzigen via Picker ───────────────── */
    public async Task UpdateUserRoleAsync(User user)
    {
        if (user is null) return;

        user.Role = Roles.Normalize(user.Role);   // ★ normaliseren
        await _db.SaveChangesAsync();
    }

    /* ── nieuwe gebruiker toevoegen ────────────── */
    [RelayCommand]
    private async Task AddNewUserAsync()
    {
        NieuwEmail = NieuwEmail.Trim();

        if (!new EmailAddressAttribute().IsValid(NieuwEmail) ||
            NieuwWachtwoord.Length < 6)
        {
            await Shell.Current.DisplayAlert("Fout",
                "Vul een geldig e-mailadres en een wachtwoord ≥ 6 tekens in.", "OK");
            return;
        }

        bool exists = await _db.Users
                               .AnyAsync(u => u.Email.ToLower() == NieuwEmail.ToLower());
        if (exists)
        {
            await Shell.Current.DisplayAlert("Bestaat al",
                "Er bestaat al een gebruiker met dit e-mailadres.", "OK");
            return;
        }

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            NieuwWachtwoord, salt, 100_000,
            HashAlgorithmName.SHA256, 32);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = NieuwEmail,
            PasswordSalt = salt,
            PasswordHash = hash,
            Role = Roles.Standaard,
            CreatedUtc = DateTime.UtcNow
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        NieuwEmail = NieuwWachtwoord = string.Empty;
        await LoadUsersAsync();
    }
}
