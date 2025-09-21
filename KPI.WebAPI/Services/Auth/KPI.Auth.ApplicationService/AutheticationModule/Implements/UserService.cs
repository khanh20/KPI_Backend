using KPI.Auth.ApplicationService.AutheticationModule.Abstract;
using KPI.Auth.ApplicationService.AutheticationModule.Dtos;
using KPI.Auth.ApplicationService.AutheticationModule.Dtos.RoleDto;
using KPI.Auth.ApplicationService.AutheticationModule.Dtos.UserDto;
using KPI.Auth.Domain;
using KPI.Auth.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace KPI.Auth.ApplicationService.AutheticationModule.Implements
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;
        public UserService(AuthDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
           
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            // kiểm tra email,username có tồn tại ko
            var infoUserExists = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName || u.Email == dto.Email);
            if (infoUserExists != null)
            {
                if (infoUserExists.UserName == dto.UserName)
                {
                    throw new Exception("Đã tồn tại username");
                }
                if (infoUserExists.Email == dto.Email)
                {
                    throw new Exception("Đã tồn tại email");
                }
            }
            var user = new Domain.User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                UnitId = dto.UnitId,
                Email = dto.Email,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Position = dto.Position,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                UnitId =user.UnitId, 
                RoleId = user.RoleId,
                Position = user.Position
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                UserName = user.UserName,
                RoleId = user.RoleId,
                UnitId=user.UnitId,
                Position = user.Position
            };
        }

        public async Task<bool> CheckPasswordAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    UserName = u.UserName,
                    RoleId = u.RoleId,
                    UnitId = u.UnitId,
                    Position = u.Position
                }).ToListAsync();
        }
        public async Task <LoginDto> Login ( LoginRequestDto dto)
        {
            // kiểm tra thông tin đăng nhập
            if (String.IsNullOrEmpty(dto.Password) || String.IsNullOrEmpty(dto.UserNameOrEmail))
            {
                throw new Exception("Thong tin dang nhap khong hop le!");
            }
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(
                u => u.Email == dto.UserNameOrEmail || u.UserName == dto.UserNameOrEmail
            );
            if (user == null)
            {
                throw new Exception("Không tồn tại tài khoản");
            }
            // check mật khẩu
            bool PasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!PasswordValid) 
            {
                throw new Exception("Sai mat khau");
            }
            var result = new LoginDto
            {
                UserID = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.Name,
                FullName = user.FirstName + " " + user.LastName,
                Address = user.Address ,
                PhoneNumber = user.PhoneNumber,
                Token = GenerateJwtToken(user),
            };
            return result;
        }
        public string GenerateJwtToken(User user)
        {
            // 1. Tạo claims (thông tin sẽ lưu trong token)
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "")
        };

            // 2. Lấy secret key từ appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Tạo token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // token hết hạn sau 2 giờ
                signingCredentials: creds
            );

            // 4. Trả về token dạng string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<IEnumerable<UserRoleDto>> GetAllUserRole()
        {
            var result = await (from u in _context.Users
                                join r in _context.Roles on u.RoleId equals r.Id
                                select new UserRoleDto
                                {
                                    UserId = u.Id,
                                    RoleId = r.Id,
                                    FullName = u.FirstName + " " + u.LastName,
                                    RoleName = r.Name,
                                    RoleDescription = r.Description,
                                    Permissions = _context.RolePermissions
                                        .Where(rp => rp.RoleId == r.Id)
                                        .Select(rp => rp.PermissionKey)
                                        .ToArray()
                                }).ToListAsync();

            return result;
        }
        public async Task<UserRoleDto?> GetUserRoleById(int userId)
        {
            var result = await (from u in _context.Users
                                join r in _context.Roles on u.RoleId equals r.Id
                                where u.Id == userId
                                select new UserRoleDto
                                {
                                    UserId = u.Id,
                                    RoleId = r.Id,
                                    FullName = u.FirstName + " " + u.LastName,
                                    RoleName = r.Name,
                                    RoleDescription = r.Description,
                                    Permissions = _context.RolePermissions
                                        .Where(rp => rp.RoleId == r.Id)
                                        .Select(rp => rp.PermissionKey)
                                        .ToArray()
                                }).FirstOrDefaultAsync();

            return result;
        }
        public async Task<IEnumerable<User>> GetUsersByUnitId(int unitId)
        {
            return await _context.Users
                                 .Where(u => u.UnitId == unitId)
                                 .ToListAsync();
        }

        public async Task<int> GetUserCount(int unitId)
        {
            var count = await _context.Users.CountAsync(u => u.UnitId == unitId);
            return count;
        }

    }
}
