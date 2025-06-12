using Microsoft.EntityFrameworkCore;
using OverstromingsApp.Models;
using System.Reflection.Emit;
using Microsoft.Data.SqlClient;


namespace OverstromingsApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DataModel> Neerslag { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataModel>().HasKey(x => new { x.Jaar, x.Maand });
        }
    }
}
