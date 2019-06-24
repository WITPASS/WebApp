using Data;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasIndex(c => c.Email);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}