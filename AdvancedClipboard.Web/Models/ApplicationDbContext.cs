using AdvancedClipboard.Web.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
