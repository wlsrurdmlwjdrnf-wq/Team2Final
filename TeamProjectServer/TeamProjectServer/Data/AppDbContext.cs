using Microsoft.EntityFrameworkCore;
using TeamProjectServer.Models;

namespace TeamProjectServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Weapon> weapons { get; set; }
        public DbSet<Accessory> accessorys { get; set; }
        public DbSet<Artifact> artifacts { get; set; }
        public DbSet<PlayerInit> playerInits { get; set; }
        public DbSet<Skill> skills { get; set; }
        public DbSet<Stage> stages { get; set; }
        public DbSet<PlayerAccountData> playerAccountData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerAccountData>().Property(p => p.Inventory).HasColumnType("jsonb");                
        }
    }
}
