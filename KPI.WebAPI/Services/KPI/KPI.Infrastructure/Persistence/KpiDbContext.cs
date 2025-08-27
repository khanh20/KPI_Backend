using KPI.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Infrastructure.Persistence
{
    public class KpiDbContext : DbContext
    {
        public KpiDbContext(DbContextOptions<KpiDbContext> options) : base(options)
        {

        }

        public DbSet<KPIAssignment> KpiAssignments { get; set; }
        public DbSet<KPIItem> KpiItems { get; set; }
        public DbSet<KPITemplate> KpiTemplates { get; set; }
        public DbSet<KpiScore> KpiScores { get; set; }
        public DbSet<KpiViolation> kpiViolations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<KPIAssignment>(entity =>
            {
                entity.Property(r => r.Status).HasDefaultValue(1); // Active
            });

            modelBuilder.Entity<KPIItem>()
            .HasOne<KPITemplate>()
            .WithMany()
            .HasForeignKey(ki => ki.KpiTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<KPIAssignment>()
            .HasOne<KPIItem>()
            .WithMany()
            .HasForeignKey(ka  => ka.KpiItemId)
            .OnDelete(DeleteBehavior.Cascade);


        }
    }
    }