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

    [ObservableProperty]
    private string _zoekterm = string.Empty;

    [ObservableProperty]
    private string _nieuwEmail = string.Empty;

    [ObservableProperty]
    private string _nieuwWachtwoord = string.Empty;

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
        try
        {
            if (user == null)
            {
                await Shell.Current.DisplayAlert("Fout", "Geen geldige gebruiker geselecteerd.", "OK");
                return;
            }

            // Haal de gebruiker vers uit de database om te voorkomen dat de lokale Role-binding null is
            var userInDb = await _db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (userInDb == null)
            {
                await Shell.Current.DisplayAlert("Fout", "Gebruiker bestaat niet meer.", "OK");
                return;
            }

            _db.Entry(userInDb).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            await Shell.Current.DisplayAlert("Verwijderd", $"Gebruiker '{user.Email}' is verwijderd.", "OK");
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            await Shell.Current.DisplayAlert("Fout", $"Verwijderen mislukt: {msg}", "OK");
            System.Diagnostics.Debug.WriteLine($"[DeleteUserAsync] ERROR: {ex}");
        }
    }



    public async Task UpdateUserRoleAsync(User user)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(user.Role))
                user.Role = "Standaard";

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Fout", $"Rol bijwerken mislukt: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"[UpdateUserRoleAsync] ERROR: {ex}");
        }
    }

    [RelayCommand]
    private async Task AddNewUserAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NieuwEmail) || string.IsNullOrWhiteSpace(NieuwWachtwoord))
            {
                await Shell.Current.DisplayAlert("Fout", "Vul een e-mailadres en wachtwoord in.", "OK");
                return;
            }

            if (NieuwWachtwoord.Length < 6)
            {
                await Shell.Current.DisplayAlert("Fout", "Wachtwoord moet minstens 6 tekens zijn.", "OK");
                return;
            }

            var bestaatAl = await _db.Users.AnyAsync(u => u.Email == NieuwEmail);
            if (bestaatAl)
            {
                await Shell.Current.DisplayAlert("Bestaat al", "Er bestaat al een gebruiker met dit e-mailadres.", "OK");
                return;
            }

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

            await Shell.Current.DisplayAlert("Succes", "Gebruiker toegevoegd.", "OK");
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Fout", $"Er is iets misgegaan: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"[AddNewUserAsync] ERROR: {ex}");
        }
    }
}
