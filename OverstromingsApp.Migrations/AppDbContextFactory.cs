using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OverstromingsApp.Core;

namespace OverstromingsApp.Migrations
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OverstromingsDb;Trusted_Connection=True;");
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
