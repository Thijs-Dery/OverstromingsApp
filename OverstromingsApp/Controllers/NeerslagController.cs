using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace OverstromingsApp.Controllers;

public class NeerslagController
{
    private readonly AppDbContext _context;
    public NeerslagController(AppDbContext context) => _context = context;

    public Task<List<DataModel>> GetAllAsync()
        => _context.Neerslag
                   .OrderBy(n => n.Jaar)
                   .ThenBy(n => n.Maand)
                   .ToListAsync();

    public Task<List<YearMonthValue>> GetYearTotalsAsync()
        => _context.Neerslag
            .GroupBy(n => n.Jaar)
            .Select(g => new YearMonthValue
            {
                Year = g.Key,
                Value = g.Sum(x => x.NeerslagMM)
            })
            .OrderBy(v => v.Year)
            .ToListAsync();

    public Task<List<YearMonthValue>> GetMonthsForYearAsync(int year)
        => _context.Neerslag
            .Where(n => n.Jaar == year)
            .GroupBy(n => n.Maand)
            .Select(g => new YearMonthValue
            {
                Year = year,
                Month = g.Key,
                Value = g.Sum(x => x.NeerslagMM)
            })
            .OrderBy(v => v.Month)
            .ToListAsync();

    public Task<List<YearMonthValue>> GetAllMonthsAsync()
    => _context.Neerslag
        .GroupBy(n => new { n.Jaar, n.Maand })
        .Select(g => new YearMonthValue
        {
            Year = g.Key.Jaar,
            Month = g.Key.Maand,
            Value = g.Sum(x => x.NeerslagMM)
        })
        .OrderBy(v => v.Year)
        .ThenBy(v => v.Month)
        .ToListAsync();
}

/// DTO die de grafiek gebruikt
public class YearMonthValue
{
    public int Year { get; set; }
    public int? Month { get; set; }
    public float Value { get; set; }
}
