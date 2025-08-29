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
            optionsBuilder.UseSqlServer("Server=THANHMOI\\SQLEXPRESS;Database=KPIAuthDb;Trusted_Connection=True;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True");

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
