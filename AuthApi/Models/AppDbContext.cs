using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AuthApi.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApplicationUser> applicationUsers { get; set; } = null!;
        public DbSet<PostTable> posts { get; set; } = null!;
        public DbSet<PostComment> comments { get; set; }
        public DbSet<PlaceTable> places { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string conn = "Server=localhost;Port=3306;Database=Auth;user=root;password=";
                optionsBuilder.UseMySQL(conn);
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Aspnetuserlogin>()
            .HasKey(al => new { al.UserId, al.LoginProvider });
            builder.Entity<Aspnetusertoken>()
            .HasKey(a => new { a.UserId, a.LoginProvider, a.Name });
            base.OnModelCreating(builder);
            builder.Entity<PostTable>().ToTable("PostTable");
            
        }
    }
}
