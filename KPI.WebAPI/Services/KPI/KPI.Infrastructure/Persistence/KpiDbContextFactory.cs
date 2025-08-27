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
            optionsBuilder.UseSqlServer("Data Source=ADMIN-PC\\MSSQLSERVER_1;Initial Catalog=KPI;Integrated Security=True;Trust Server Certificate=True");

            return new KpiDbContext(optionsBuilder.Options);
        }
    }
}
