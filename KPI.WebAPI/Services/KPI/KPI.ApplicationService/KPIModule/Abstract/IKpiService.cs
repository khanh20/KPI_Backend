using KPI.ApplicationService.KPIModule.Dtos;
using KPI.ApplicationService.KPIModule.Dtos.ApprovalDto;
using KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto;
using KPI.ApplicationService.KPIModule.Dtos.UnitDto;
using KPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Abstract
{
    public interface IKpiService
    {
        //KPI TEMPLATE
        #region KPITemplate
        Task<List<KpiTemplateDto>> GetAllAsync();
        Task<KpiTemplateDto?> GetByIdAsync(int id);
        Task<KpiTemplateDto> CreateAsync(CreateKpiTemplateDto dto);





        #endregion

        #region KPIITem

        //Lấy toàn bộ KPI items trong hệ thống.
        Task<List<KpiItemDto>> GetAllItemsAsync();
        //Lấy chi tiết KPI item theo Id.
        Task<KpiItemDto?> GetItemByIdAsync(int id);
        //Tạo mới một KPI item gắn với người dùng tạo.
        Task<KpiItemDto> CreateItemAsync(CreateKpiItemDto dto, int userId);
        //Cập nhật thông tin KPI item theo Id.
        Task<KpiItemDto> UpdateItemAsync(int id, UpdateKpiItemDto dto, int userId);
        //Đánh dấu xóa mềm KPI item (Deleted = true).
        Task<bool> DeleteItemAsync(int id, int userId);
        //Lấy danh sách KPI items mà chính user đã tạo.
        Task<List<KpiItemDto>> GetItemsByCreatorAsync(int userId);

        #endregion

        #region KPIAssignment
        Task<KpiAssignmentDto> AssignAsync(CreateKpiAssignmentDto dto, int createdBy);
        Task<KpiAssignmentDto?> GetAssignmentByIdAsync(int id);
        Task<List<KpiAssignmentDto>> GetAssignmentByUserAsync(int userId);
        Task<List<KpiAssignmentDto>> GetAssignmentByUnitAsync(int unitId);
        Task<KpiAssignmentDto?> UpdateAssignmentAsync(int id, UpdateKpiAssignmentDto dto, int modifiedBy);
        #endregion

        #region Approval
        Task<ApprovalLogDto> ApproveAsync(ApproveKpiAssignmentDto dto, int approverId);
        Task<List<ApprovalLogDto>> GetLogsByAssignmentIdAsync(int assignmentId);
        #endregion

        #region Unit
        Task<List<UnitDto>> GetAllUnitAsync();
        Task<UnitDto?> GetUnitByIdAsync(int id);
        Task<UnitDto> CreateAsync(CreateUnitDto dto);
        Task<UnitDto?> UpdateAsync(int id, UpdateUnitDto dto);
        Task<bool> DeleteAsync(int id);
        #endregion


    }
}
