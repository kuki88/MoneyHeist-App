using Microsoft.EntityFrameworkCore;
using MonesyHeist_App.Data.Model;

namespace MonesyHeist_App.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Member>().HasIndex(u => u.Email).IsUnique();
            builder.Entity<Heist>().HasIndex(u => u.Name).IsUnique();
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Heist> Heists { get; set; }
        public DbSet<HeistSkills> HeistSkills { get; set; }
        public DbSet<HeistMembers> HeistMembers { get; set; }
    }
}
