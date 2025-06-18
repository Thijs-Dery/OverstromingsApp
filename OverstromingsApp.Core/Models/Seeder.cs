using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Core;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp.Core.Models;

public static class Seeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var data = GetSeedData();

        foreach (var item in data)
        {
            bool exists = await context.Neerslag.AnyAsync(n =>
                n.Jaar == item.Jaar && n.Maand == item.Maand);

            if (!exists)
                await context.Neerslag.AddAsync(item);
        }

        await context.SaveChangesAsync();
    }

    private static List<DataModel> GetSeedData()
    {
        var waarden = new int[,]
        {
            {67,54,73,68,74,79,98,89,68,98,98,108},
            {66,45,61,57,70,82,85,78,77,94,100,103},
            {62,56,64,62,72,79,87,94,70,90,104,104},
            {66,45,63,61,79,81,89,83,63,91,98,115},
            {67,46,72,58,72,83,95,90,66,93,102,115},
            {63,54,64,54,79,87,90,90,72,92,102,118},
            {65,63,57,64,75,79,90,75,69,97,107,107},
            {61,52,75,62,72,83,90,90,66,93,98,103},
            {66,56,70,59,68,78,88,81,69,97,109,111},
            {66,55,60,60,75,92,89,87,70,89,106,114},
            {69,50,77,53,78,91,85,82,70,92,92,110},
            {60,57,65,68,71,78,94,79,71,102,92,111},
            {66,59,64,53,78,81,91,87,67,96,101,106},
            {74,57,64,63,70,84,96,81,75,97,104,119},
            {64,51,66,56,75,82,91,89,70,102,99,124},
            {68,51,65,62,74,84,92,85,66,87,98,114},
            {66,49,71,62,71,81,90,79,72,98,105,115},
            {58,50,73,63,78,99,93,91,75,98,98,114},
            {61,54,68,60,87,71,93,77,68,100,100,105},
            {61,58,66,61,75,77,101,88,60,96,97,114}
        };

        var data = new List<DataModel>();
        int startJaar = 2005;

        for (int i = 0; i < waarden.GetLength(0); i++)
        {
            for (int maand = 1; maand <= 12; maand++)
            {
                data.Add(new DataModel
                {
                    Jaar = startJaar + i,
                    Maand = maand,
                    NeerslagMM = waarden[i, maand - 1]
                });
            }
        }

        return data;
    }
}
