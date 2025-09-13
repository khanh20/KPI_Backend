using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace KPI.Auth.Infrastructure.Persistence
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            // Hardcode tạm connection string
            optionsBuilder.UseSqlServer("Data Source=ADMIN-PC\\MSSQLSERVER_1;Initial Catalog=KPI;Integrated Security=True;Trust Server Certificate=True");

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
