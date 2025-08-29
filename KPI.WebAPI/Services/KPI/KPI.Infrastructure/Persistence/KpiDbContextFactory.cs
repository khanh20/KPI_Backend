using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Infrastructure.Persistence
{
    public class KpiDbContextFactory : IDesignTimeDbContextFactory<KpiDbContext>
    {
        public KpiDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<KpiDbContext>();
            // Hardcode tạm connection string
            optionsBuilder.UseSqlServer("Server=THANHMOI\\SQLEXPRESS;Database=KPICoreDB;Trusted_Connection=True;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True");

            return new KpiDbContext(optionsBuilder.Options);
        }
    }
}
