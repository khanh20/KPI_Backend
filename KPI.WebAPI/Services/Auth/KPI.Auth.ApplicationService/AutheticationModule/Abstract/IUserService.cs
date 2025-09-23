using KPI.Auth.ApplicationService.AutheticationModule.Dtos.RoleDto;
using KPI.Auth.ApplicationService.AutheticationModule.Dtos.UserDto;
using KPI.Auth.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Auth.ApplicationService.AutheticationModule.Abstract
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> CheckPasswordAsync(string username, string password);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
        Task<LoginDto> Login(LoginRequestDto dto);
        public string GenerateJwtToken(User user);
        Task<IEnumerable<UserRoleDto>> GetAllUserRole();
        Task<UserRoleDto?> GetUserRoleById(int userId);
        Task<IEnumerable<User>> GetUsersByUnitId(int unitId);

        Task<int> GetUserCount( int unitId);
    }
}
