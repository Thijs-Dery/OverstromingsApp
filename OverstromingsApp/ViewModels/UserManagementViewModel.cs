using System.Collections.ObjectModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp.ViewModels;

public partial class UserManagementViewModel : ObservableObject
{
    private readonly AppDbContext _db;

    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<string> Rollen { get; } = new() { "Standaard", "Analist", "Admin" };

    [ObservableProperty] private string zoekterm = string.Empty;
    [ObservableProperty] private string nieuwEmail = string.Empty;
    [ObservableProperty] private string nieuwWachtwoord = string.Empty;

    public UserManagementViewModel(AppDbContext db)
    {
        _db = db;
        _ = LoadUsersAsync();
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        Users.Clear();
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(Zoekterm))
            query = query.Where(u => u.Email.Contains(Zoekterm));

        var result = await query.OrderBy(u => u.Email).ToListAsync();
        foreach (var user in result)
            Users.Add(user);
    }

    [RelayCommand]
    private async Task DeleteUserAsync(User user)
    {
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        await LoadUsersAsync();
    }

    // 🔧 Dit is de methode die we aanroepen in .xaml.cs
    public async Task UpdateUserRoleAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    [RelayCommand]
    private async Task AddNewUserAsync()
    {
        if (string.IsNullOrWhiteSpace(NieuwEmail) || string.IsNullOrWhiteSpace(NieuwWachtwoord))
            return;

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            NieuwWachtwoord,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = NieuwEmail,
            PasswordSalt = salt,
            PasswordHash = hash,
            Role = "Standaard"
        };

        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();

        NieuwEmail = string.Empty;
        NieuwWachtwoord = string.Empty;

        await LoadUsersAsync();
    }
}
