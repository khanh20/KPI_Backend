using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Auth.ApplicationService.AutheticationModule.Dtos.RoleDto
{
    public class LoginDto
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

        public int? UnitId { get; set; }
        }
}
