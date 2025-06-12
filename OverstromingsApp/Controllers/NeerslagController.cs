using OverstromingsApp.Data;
using OverstromingsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace OverstromingsApp.Controllers;

public class NeerslagController
{
    private readonly AppDbContext _context;

    public NeerslagController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DataModel>> GetAllAsync()
    {
        return await _context.Neerslag
            .OrderBy(n => n.Jaar)
            .ThenBy(n => n.Maand)
            .ToListAsync();
    }
}
