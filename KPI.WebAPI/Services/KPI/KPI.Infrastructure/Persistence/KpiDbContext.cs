using KPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace KPI.Infrastructure.Persistence
{
    public class KpiDbContext : DbContext
    {
        public KpiDbContext(DbContextOptions<KpiDbContext> options) : base(options)
        {
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<KPITemplate> KpiTemplates { get; set; }
        public DbSet<KPIItem> KpiItems { get; set; }
        public DbSet<KPIAssignment> KpiAssignments { get; set; }
        public DbSet<KpiScore> KpiScores { get; set; }
        public DbSet<KpiViolation> KpiViolations { get; set; }
        public DbSet<ApprovalLog> ApprovalLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // KPIItem -> KPITemplate (1-n)
            modelBuilder.Entity<KPIItem>()
                .HasOne<KPITemplate>()
                .WithMany()
                .HasForeignKey(i => i.KpiTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // KPIAssignment -> KPIItem (1-n)
            modelBuilder.Entity<KPIAssignment>()
                .HasOne<KPIItem>()
                .WithMany()
                .HasForeignKey(a => a.KpiItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApprovalLog.Timestamp mặc định = GETDATE()
            modelBuilder.Entity<ApprovalLog>()
                .Property(l => l.Timestamp)
                .HasDefaultValueSql("GETDATE()");

            // KpiViolation.ViolationDate mặc định = GETDATE()
            modelBuilder.Entity<KpiViolation>()
                .Property(v => v.ViolationDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
