
using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos;
using KPI.ApplicationService.KPIModule.Dtos.ApprovalDto;
using KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto;
using KPI.ApplicationService.KPIModule.Dtos.UnitDto;
using KPI.Domain;
using KPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KPI.ApplicationService.KpiModule.Implements
{
    public class KpiService : IKpiService
    {
        private readonly KpiDbContext _context;

        public KpiService(KpiDbContext context)
        {
            _context = context;
        }

        //KPI Template
        #region KPI Template
        public async Task<List<KpiTemplateDto>> GetAllAsync()
        {
            return await _context.KpiTemplates
                .Select(t => new KpiTemplateDto
                {
                    Id = t.Id,
                    TemplateName = t.TemplateName,
                    Description = t.Description
                }).ToListAsync();
        }

        public async Task<KpiTemplateDto?> GetByIdAsync(int id)
        {
            var template = await _context.KpiTemplates.FindAsync(id);
            if (template == null) return null;

            return new KpiTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Description = template.Description
            };
        }

        public async Task<KpiTemplateDto> CreateAsync(CreateKpiTemplateDto dto)
        {
            var template = new KPITemplate
            {
                TemplateName = dto.TemplateName,
                Description = dto.Description
            };

            _context.KpiTemplates.Add(template);
            await _context.SaveChangesAsync();

            return new KpiTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Description = template.Description
            };
        }





        #endregion

        #region KPI Item


        //KPI Item
        public async Task<List<KpiItemDto>> GetAllItemsAsync()
        {
            return await _context.KpiItems
                .Select(i => new KpiItemDto
                {
                    Id = i.Id,
                    KpiName = i.KpiName,
                    KpiType = i.KpiType,
                    Weight = i.Weight,
                    KpiTemplateId = i.KpiTemplateId,
                    CalculationFormula = i.CalculationFormula,
                    DeadLine = i.DeadLine
                }).ToListAsync();
        }

        public async Task<KpiItemDto?> GetItemByIdAsync(int id)
        {
            var item = await _context.KpiItems.FindAsync(id);
            if (item == null) return null;

            return new KpiItemDto
            {
                Id = item.Id,
                KpiName = item.KpiName,
                KpiType = item.KpiType,
                Weight = item.Weight,
                KpiTemplateId = item.KpiTemplateId,
                CalculationFormula = item.CalculationFormula,
                DeadLine = item.DeadLine
            };
        }

        public async Task<KpiItemDto> CreateItemAsync(CreateKpiItemDto dto, int userId)
        {
            var item = new KPIItem
            {
                KpiName = dto.KpiName,
                KpiType = dto.KpiType,
                Weight = dto.Weight,
                DeadLine = dto.DeadLine,
                KpiTemplateId = dto.KpiTemplateId,
                CalculationFormula = dto.CalculationFormula,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.KpiItems.Add(item);
            await _context.SaveChangesAsync();

            return new KpiItemDto
            {
                Id = item.Id,
                KpiName = item.KpiName,
                KpiType = item.KpiType,
                Weight = item.Weight,
                CalculationFormula = item.CalculationFormula,
                KpiTemplateId = item.KpiTemplateId,
                DeadLine = item.DeadLine
            };
        }

        public async Task<KpiItemDto> UpdateItemAsync(int id, UpdateKpiItemDto dto, int userId)
        {
            var item = await _context.KpiItems.FindAsync(id);
            if (item == null) throw new Exception("KPI Item not found");

            item.KpiName = dto.KpiName;
            item.KpiType = dto.KpiType;
            item.Weight = dto.Weight;
            item.DeadLine = dto.DeadLine;
            item.KpiTemplateId = dto.KpiTemplateId;
            item.CalculationFormula = dto.CalculationFormula;
            item.ModifiedBy = userId;
            item.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new KpiItemDto
            {
                Id = item.Id,
                KpiName = item.KpiName,
                KpiType = item.KpiType,
                Weight = item.Weight,
                KpiTemplateId = item.KpiTemplateId,
                DeadLine = item.DeadLine
            };
        }

        public async Task<bool> DeleteItemAsync(int id, int userId)
        {
            var item = await _context.KpiItems.FindAsync(id);
            if (item == null) return false;

            item.Deleted = true;
            item.DeletedBy = userId;
            item.DeletedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<KpiItemDto>> GetItemsByCreatorAsync(int userId)
        {
            return await _context.KpiItems
                .Where(i => i.CreatedBy == userId && !i.Deleted) // chỉ lấy KPI mình tạo và chưa bị xóa
                .Select(i => new KpiItemDto
                {
                    Id = i.Id,
                    KpiName = i.KpiName,
                    KpiType = i.KpiType,
                    Weight = i.Weight,
                    CalculationFormula = i.CalculationFormula,
                    KpiTemplateId = i.KpiTemplateId,
                    DeadLine = i.DeadLine
                }).ToListAsync();
        }
        #endregion

        #region ApprovalLog
        public async Task<ApprovalLogDto> ApproveAsync(ApproveKpiAssignmentDto dto, int approverId)
        {
            var assignment = await _context.KpiAssignments.FindAsync(dto.AssignmentId);
            if (assignment == null) throw new Exception("Assignment not found");

            // Cập nhật trạng thái
            assignment.Status = dto.Action;
            assignment.ModifiedBy = approverId;
            assignment.ModifiedDate = DateTime.UtcNow;

            // Log lại
            var log = new ApprovalLog
            {
                KpiAssignmentId = assignment.Id,
                UserId = approverId,
                Action = dto.Action,
                Comment = dto.Comment,
                Timestamp = DateTime.UtcNow
            };

            _context.ApprovalLogs.Add(log);
            await _context.SaveChangesAsync();

            return new ApprovalLogDto
            {
                Id = log.Id,
                KpiAssignmentId = log.KpiAssignmentId,
                UserId = log.UserId,
                Action = log.Action,
                Comment = log.Comment,
                Timestamp = log.Timestamp
            };
        }

        public async Task<List<ApprovalLogDto>> GetLogsByAssignmentIdAsync(int assignmentId)
        {
            return await _context.ApprovalLogs
                .Where(l => l.KpiAssignmentId == assignmentId)
                .Select(l => new ApprovalLogDto
                {
                    Id = l.Id,
                    KpiAssignmentId = l.KpiAssignmentId,
                    UserId = l.UserId,
                    Action = l.Action,
                    Comment = l.Comment,
                    Timestamp = l.Timestamp
                }).ToListAsync();
        }
        #endregion


        #region Assignment
        public async Task<KpiAssignmentDto> AssignAsync(CreateKpiAssignmentDto dto, int createdBy)
        {
            var entity = new KPIAssignment
            {
                UserId = dto.UserId,
                UnitId = dto.UnitId,
                KpiItemId = dto.KpiItemId,
                TargetValue = dto.TargetValue,
                ContributionWeight = dto.ContributionWeight,
                Year = dto.Year,
                Status = "Pending",
                CreatedByUserId = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.KpiAssignments.Add(entity);
            await _context.SaveChangesAsync();

            return new KpiAssignmentDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                UnitId = entity.UnitId,
                KpiItemId = entity.KpiItemId,
                TargetValue = entity.TargetValue,
                ContributionWeight = entity.ContributionWeight,
                Status = entity.Status,
                Year = entity.Year
            };
        }

        public async Task<KpiAssignmentDto?> GetAssignmentByIdAsync(int id)
        {
            var entity = await _context.KpiAssignments.FindAsync(id);
            if (entity == null) return null;

            return new KpiAssignmentDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                UnitId = entity.UnitId,
                KpiItemId = entity.KpiItemId,
                TargetValue = entity.TargetValue,
                ContributionWeight = entity.ContributionWeight,
                ActualResults = entity.ActualResults,
                ComponentScore = entity.ComponentScore,
                Status = entity.Status,
                Year = entity.Year
            };
        }

        public async Task<List<KpiAssignmentDto>> GetAssignmentByUserAsync(int userId)
        {
            return await _context.KpiAssignments
                .Where(a => a.UserId == userId)
                .Select(a => new KpiAssignmentDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UnitId = a.UnitId,
                    KpiItemId = a.KpiItemId,
                    TargetValue = a.TargetValue,
                    ContributionWeight = a.ContributionWeight,
                    ActualResults = a.ActualResults,
                    ComponentScore = a.ComponentScore,
                    Status = a.Status,
                    Year = a.Year
                }).ToListAsync();
        }

        public async Task<List<KpiAssignmentDto>> GetAssignmentByUnitAsync(int unitId)
        {
            return await _context.KpiAssignments
                .Where(a => a.UnitId == unitId)
                .Select(a => new KpiAssignmentDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UnitId = a.UnitId,
                    KpiItemId = a.KpiItemId,
                    TargetValue = a.TargetValue,
                    ContributionWeight = a.ContributionWeight,
                    ActualResults = a.ActualResults,
                    ComponentScore = a.ComponentScore,
                    Status = a.Status,
                    Year = a.Year
                }).ToListAsync();
        }

        public async Task<KpiAssignmentDto?> UpdateAssignmentAsync(int id, UpdateKpiAssignmentDto dto, int modifiedBy)
        {
            var entity = await _context.KpiAssignments.FindAsync(id);
            if (entity == null) return null;

            entity.TargetValue = dto.TargetValue;
            entity.ContributionWeight = dto.ContributionWeight;
            entity.Status = dto.Status;
            entity.ModifiedBy = modifiedBy;
            entity.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new KpiAssignmentDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                UnitId = entity.UnitId,
                KpiItemId = entity.KpiItemId,
                TargetValue = entity.TargetValue,
                ContributionWeight = entity.ContributionWeight,
                ActualResults = entity.ActualResults,
                ComponentScore = entity.ComponentScore,
                Status = entity.Status,
                Year = entity.Year
            };
        }
            #endregion

        #region Unit
            public async Task<List<UnitDto>> GetAllUnitAsync()
        {
            return await _context.Units
                .Select(u => new UnitDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    HeadOfUnitId = u.HeadOfUnitId
                }).ToListAsync();
        }

        public async Task<UnitDto?> GetUnitByIdAsync(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null) return null;

            return new UnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                HeadOfUnitId = unit.HeadOfUnitId
            };
        }

        public async Task<UnitDto> CreateAsync(CreateUnitDto dto)
        {
            var unit = new Unit
            {
                Name = dto.Name,
                HeadOfUnitId = dto.HeadOfUnitId
            };

            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            return new UnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                HeadOfUnitId = unit.HeadOfUnitId
            };
        }

        public async Task<UnitDto?> UpdateAsync(int id, UpdateUnitDto dto)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null) return null;

            unit.Name = dto.Name;
            unit.HeadOfUnitId = dto.HeadOfUnitId;

            await _context.SaveChangesAsync();

            return new UnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                HeadOfUnitId = unit.HeadOfUnitId
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null) return false;

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion


    }
}
