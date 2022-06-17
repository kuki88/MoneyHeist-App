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
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<Skill> Skills { get; set; }
    }
}
