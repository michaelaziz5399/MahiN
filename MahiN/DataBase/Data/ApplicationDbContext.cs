using MahiN.DataBase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MahiN.DataBase.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : IdentityDbContext<AppUser>(options)
    {
        #region DbSets
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Playground> Playgrounds { get; set; }
        public DbSet<BookingPlayground> BookingPlaygrounds { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentMatch> TournamentMatches { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<MatchEvent> MatchEvents { get; set; }
        public DbSet<PlayerSuspension> PlayerSuspensions { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity Tables Rename
            builder.Entity<AppUser>().ToTable("users");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");

            #region Booking Relationships
            builder.Entity<Booking>(entity =>
            {
                entity.HasOne(b => b.CreatedBy)
                      .WithMany()
                      .HasForeignKey(b => b.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.ApprovedBy)
                      .WithMany()
                      .HasForeignKey(b => b.ApprovedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<BookingPlayground>().HasKey(bp => new { bp.BookingId, bp.PlaygroundId });
            #endregion

            #region Team Relationships
            // 
            builder.Entity<Team>()
                .HasIndex(t => new { t.TournamentId, t.GroupName });

            builder.Entity<TeamMember>()
                .HasIndex(tm => tm.TeamId)
                .IsUnique()
                .HasFilter("[IsCaptain] = 1");

            // Unique constraint لعدم تكرار نفس اللاعب في الفريق نفسه
            builder.Entity<TeamMember>()
                .HasIndex(tm => new { tm.TeamId, tm.UserId })
                .IsUnique();

            builder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany()
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region TournamentMatch Relationships
            builder.Entity<TournamentMatch>(entity =>
            {
                // 1. علاقة الفريق المستضيف (منع الحذف المتتابع)
                entity.HasOne(m => m.HomeTeam)
                      .WithMany()
                      .HasForeignKey(m => m.HomeTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 2. علاقة الفريق الضيف (منع الحذف المتتابع)
                entity.HasOne(m => m.AwayTeam)
                      .WithMany()
                      .HasForeignKey(m => m.AwayTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 3. علاقة الفريق الفائز (منع الحذف المتتابع)
                entity.HasOne(m => m.WinnerTeam)
                      .WithMany()
                      .HasForeignKey(m => m.WinnerTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 4. علاقة المباراة التالية (Self-Referencing Relationship)
                entity.HasOne(m => m.NextMatch)
                      .WithMany()
                      .HasForeignKey(m => m.NextMatchId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region MatchEvent Relationships
            // حماية MatchEvent من تعارض مسارات الحذف (Multiple Cascade Paths)
            builder.Entity<MatchEvent>()
                .HasOne(e => e.Player)
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MatchEvent>()
                .HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
