using KPI.Auth.ApplicationService.AutheticationModule.Abstract;
using KPI.Auth.ApplicationService.AutheticationModule.Dtos.RoleDto;
using KPI.Auth.ApplicationService.AutheticationModule.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace KPI.Auth.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/users
        /// <summary>
        /// Tạo tài khoản người dùng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
        //[Authorize(Roles = "Admin, hieu truong")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
           
        }

        // GET: api/users/{userId}
        /// <summary>
        /// Tìm kiếm người dùng bằng ID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
        //[Authorize(Roles = "Admin, hieu truong")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // DELETE: api/users/{userId}
        /// <summary>
        /// Xoá tài khoản 
        /// </summary>
        /// <returns></returns> 
        [Authorize(Roles = "Admin, hieu truong")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var deleted = await _userService.DeleteUserAsync(userId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // GET: api/users
        /// <summary>
        /// Lấy danh sách tất cả User
        /// </summary>
        /// <returns></returns> 
        /// 
        //[Authorize(Roles = "Admin, hieu truong")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);

        }
        [Authorize(Roles = "Admin, hieu truong")]
        [HttpGet("user-roles")]
        public async Task<IActionResult> GetAllUserRole()
        {
            var list = await _userService.GetAllUserRole();
            return Ok(list);
        }
        //[Authorize(Roles = "Admin, hieu truong")]
        [HttpGet("user-roles/{userId}")]
        public async Task<IActionResult> GetUserRoleById(int userId)
        {
            var userRole = await _userService.GetUserRoleById(userId);

            if (userRole == null)
            {
                return NotFound(new { message = "User không tồn tại hoặc chưa có role" });
            }

            return Ok(userRole);
        }


        /// <summary>
        /// Đăng nhập tài khoản
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
        
        
        [HttpPost ("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestDto dto)
        {
            try
            {
                var user = await _userService.Login(dto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
               
           
        }
        [HttpGet("users/by-unit/{unitId}")]
        public async Task<IActionResult> GetUsersByUnitId(int unitId)
        {
            var users = await _userService.GetUsersByUnitId(unitId);
            return Ok(users);
        }

        [HttpGet("count")]
        public async Task<int> GetUserCount([FromQuery] int unitId)
        {
            if (unitId <= 0) return 0;
            return await _userService.GetUserCount(unitId);
        }





    }
}
