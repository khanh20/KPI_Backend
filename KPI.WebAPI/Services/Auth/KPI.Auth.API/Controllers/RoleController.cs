using KPI.Auth.ApplicationService.AutheticationModule.Abstract;
using KPI.Auth.ApplicationService.AutheticationModule.Dtos.RoleDto;
using KPI.Auth.ApplicationService.AutheticationModule.Implements;
using KPI.Auth.Domain;
using KPI.Shared.ApplicationService;
using KPI.Shared.Constant.Common;
using KPI.Shared.Constant.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPI.Auth.API.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Tạo role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
        
        [HttpPost]
        [AuthorizePermission("AddRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            try
            {
                var role = await _roleService.CreateRole(dto);
                return CreatedAtAction(nameof(GetRoleById), new { roleId = role.Id }, role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        
        [HttpGet("{roleId}")]
        [AuthorizePermission("GetRoleById")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            var role = await _roleService.GetRoleById(roleId);
            if (role == null) return NotFound();
            return Ok(role);
        }

        /// <summary>
        /// Lấy danh sách role theo Keyword 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
       
        [HttpGet("get-all-role")]
        public async Task<IActionResult> GetAllRole([FromQuery] FilterDto input)
        {
            var roles = await _roleService.GetAllRole(input);
            return Ok(roles);
        }
        
        [HttpGet("get-all-permission")]
        public async Task<IActionResult> GetPermission()
        {
            var keys = await _roleService.GetAllPermission();

            // Trả về kiểu string[] để dễ copy trên Swagger
            return Ok(keys.ToArray());
        }

        /// <summary>
        /// Thêm permission cho role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
        
        [HttpPost("assign")]
        public async Task<IActionResult> AssignPermission([FromBody] RolePermissionDto request)
        {
            try
            {
                // Lấy danh sách permissionkey 
                var validPermissions = await _roleService.GetAllPermission();

                // Kiểm tra permission
                var invalidPermissions = request.Permissions
                    .Where(p => !validPermissions.Contains(p))
                    .ToList();

                if (invalidPermissions.Any())
                {
                    return BadRequest(new
                    {
                        message = "Permission không hợp lệ",
                        invalidPermissions = invalidPermissions
                    });
                }

                var permissionKeys = request.Permissions;

                await _roleService.AssignPermissionToRole(request.RoleId, request.RoleName, permissionKeys);

                var rolePermission = await _roleService.GetRolePermission(request.RoleId);

                var result = new RolePermissionDto
                {
                    RoleId = rolePermission.RoleId,
                    RoleName = rolePermission.RoleName,
                    Permissions = rolePermission.Permissions
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xoá quyền của role
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
        
        [HttpPost("remove")]
        public async Task<IActionResult> RemovePermission([FromBody] RolePermissionDto request)
        {
            await _roleService.RemovePermissionFromRole(request.RoleId, request.Permissions);
            var result = await _roleService.GetRolePermission(request.RoleId);
            return Ok(result);
        }


        /// <summary>
        /// Xem quyền của role theo Id 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns> 
        /// 
        
        [HttpGet("role-permission/{roleId}")]
        public async Task<IActionResult> GetRolePermission(int roleId )
        {
            try
            {
                var rolePermission = await _roleService.GetRolePermission(roleId);
                return Ok(rolePermission);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("delete-role")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            try
            {
                var role = await _roleService.GetRoleById(roleId);
                await _roleService.DeleteRole(roleId);

                return Ok(new { message = "Xoá role thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            };
        }
    }
}
