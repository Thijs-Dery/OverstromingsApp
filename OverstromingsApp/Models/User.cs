using System;

namespace OverstromingsApp.Models;

public class User
{
    public Guid Id { get; set; }   
    public string Email { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
