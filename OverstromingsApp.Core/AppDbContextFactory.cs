using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OverstromingsApp.Core.Models;

namespace OverstromingsApp.Core;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
                   .UseSqlServer(
                       "Server=np:\\\\.\\pipe\\LOCALDB#E56D628E\\tsql\\query;Database=OverstromingsDb;Trusted_Connection=True;")
                   .Options;

        return new AppDbContext(opts);
    }
}
