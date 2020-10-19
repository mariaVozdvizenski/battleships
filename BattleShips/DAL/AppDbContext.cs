using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<BattleShipsSave> BattleShipsSaves { get; set; } = default!;
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=barrel.itcollege.ee,1533; User Id=student;Password=Student.Bad.password.0;Database=mavozd_battleships;MultipleActiveResultSets=true");
        }
    }
}