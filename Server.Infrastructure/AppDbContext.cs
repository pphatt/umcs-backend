using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUsers, AppRoles, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    internal DbSet<AcademicYear> AcademicYears { get; set; }
    internal DbSet<Contribution> Contributions { get; set; }
    internal DbSet<ContributionComment> ContributionComments { get; set; }
    internal DbSet<ContributionLike> ContributionLikes { get; set; }
    internal DbSet<ContributionTag> ContributionTags { get; set; }
    internal DbSet<Faculty> Faculties { get; set; }
    internal DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Identity Configuration

        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);

        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims").HasKey(x => x.Id);

        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);

        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles")
            .HasKey(x => new { x.UserId, x.RoleId });

        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens")
            .HasKey(x => new { x.UserId });

        #endregion Identity Configuration

        #region Table Relationship Configuration

        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Faculty)
            .WithMany(f => f.Contributions)
            .HasForeignKey(c => c.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contribution>()
            .HasOne(c => c.AcademicYear)
            .WithMany(a => a.Contributions)
            .HasForeignKey(c => c.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ContributionTag>()
            .HasKey(ct => new { ct.ContributionId, ct.TagId });

        modelBuilder.Entity<ContributionTag>()
            .HasOne(ct => ct.Contribution)
            .WithMany(c => c.ContributionTags)
            .HasForeignKey(ct => ct.ContributionId);

        modelBuilder.Entity<ContributionTag>()
            .HasOne(ct => ct.Tag)
            .WithMany(t => t.ContributionTags)
            .HasForeignKey(ct => ct.TagId);

        #endregion Table Relationship Configuration
    }
}
