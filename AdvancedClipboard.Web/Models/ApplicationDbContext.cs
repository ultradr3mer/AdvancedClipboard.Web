using AdvancedClipboard.Web.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdvancedClipboard.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ClipboardContentEntity> ClipboardContent { get; set; }

        public DbSet<ContentTypeEntity> ContentType { get; set; }

        public DbSet<FileAccessTokenEntity> FileAccessToken { get; set; }

        public DbSet<LaneEntity> Lane { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("newsequentialid()");
            });

            builder.Entity<Role>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("newsequentialid()");
            });
        }
    }
}
