
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
                .Where(t => !t.Deleted)
                .Select(t => new KpiTemplateDto
                {
                    Id = t.Id,
                    TemplateName = t.TemplateName,
                    Description = t.Description,
                    Year = t.Year
                }).ToListAsync();
        }

        public async Task<KpiTemplateDto?> GetByIdAsync(int id)
        {
            var template = await _context.KpiTemplates
            .Where(t => t.Id == id && !t.Deleted)
            .FirstOrDefaultAsync();

            if (template == null) return null;

            return new KpiTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Description = template.Description,
                Year = template.Year
            };
        }

        public async Task<KpiTemplateDto> CreateAsync(CreateKpiTemplateDto dto, int createdBy)
        {
            var template = new KPITemplate
            {
                TemplateName = dto.TemplateName,
                Description = dto.Description,
                Year = dto.Year,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.KpiTemplates.Add(template);
            await _context.SaveChangesAsync();

            return new KpiTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Description = template.Description,
                Year = template.Year
            };
        }
        //UPdate TEmplate
        public async Task<KpiTemplateDto?> UpdateAsync(int id, UpdateKpiTemplateDto dto, int modifiedBy)
        {
            var template = await _context.KpiTemplates.FindAsync(id);
            if (template == null || template.Deleted) return null;

            template.TemplateName = dto.TemplateName;
            template.Description = dto.Description;
            template.Year = dto.Year;
            template.ModifiedBy = modifiedBy;
            template.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new KpiTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Description = template.Description,
                Year = template.Year
            };
        }


        // Request Delete Template
        public async Task<bool> RequestDeleteTemplateAsync(int id, int userId, string? comment = null)
        {
            var template = await _context.KpiTemplates.FindAsync(id);
            if (template == null) return false;

            // Tạo log chờ phê duyệt
            var log = new ApprovalLog
            {
                TargetType = "Template",
                TargetId = id,
                UserId = userId,
                Action = "RequestDelete",
                Comment = comment,
                Timestamp = DateTime.UtcNow
            };

            _context.ApprovalLogs.Add(log);
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete Direct ne
        public async Task<bool> DeleteTemplateAsync(int id, int deletedBy)
        {
            var template = await _context.KpiTemplates.FindAsync(id);
            if (template == null || template.Deleted) return false;

            template.Deleted = true;
            template.DeletedBy = deletedBy;
            template.DeletedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }




        #endregion

        #region KPI Item


        //KPI Item
        public async Task<List<KpiItemDto>> GetAllItemsAsync()
        {
            return await _context.KpiItems
                   .Where(i => !i.Deleted)  // lấy delete = false
                   .Select(i => new KpiItemDto
        {
                    Id = i.Id,
                    KpiName = i.KpiName,
                    KpiType = i.KpiType,
                    Weight = i.Weight,
                    TargetValue = i.TargetValue,
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
                TargetValue = item.TargetValue,
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
                TargetValue = dto.TargetValue,
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
                TargetValue= item.TargetValue,
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
            item.TargetValue = dto.TargetValue;
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
                TargetValue = item.TargetValue,
                KpiTemplateId = item.KpiTemplateId,
                CalculationFormula = item.CalculationFormula,
                DeadLine = item.DeadLine
            };
        }

        public async Task<bool> RequestDeleteItemAsync(int id, int userId, string? comment = null)
        {
            var item = await _context.KpiItems.FindAsync(id);
            if (item == null) return false;

            var log = new ApprovalLog
            {
                TargetType = "Item",
                TargetId = id,
                UserId = userId,
                Action = "RequestDelete",
                Comment = comment,
                Timestamp = DateTime.UtcNow
            };

            _context.ApprovalLogs.Add(log);
            await _context.SaveChangesAsync();
            return true;
        }
        //Lấy Item theo Template ID
        public async Task<List<KpiItemDto>> GetItemsByTemplateAsync(int templateId)
        {
            return await _context.KpiItems
                .Where(i => i.KpiTemplateId == templateId && !i.Deleted)
                .Select(i => new KpiItemDto
                {
                    Id = i.Id,
                    KpiName = i.KpiName,
                    KpiType = i.KpiType,
                    Weight = i.Weight,
                    TargetValue = i.TargetValue,
                    CalculationFormula = i.CalculationFormula,
                    KpiTemplateId = i.KpiTemplateId,
                    DeadLine = i.DeadLine
                })
                .ToListAsync();
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
                    TargetValue = i.TargetValue,
                    CalculationFormula = i.CalculationFormula,
                    KpiTemplateId = i.KpiTemplateId,
                    DeadLine = i.DeadLine
                }).ToListAsync();
        }
        #endregion

        #region ApprovalLog
        //Chua fix
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
                TargetId = assignment.Id,
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
                KpiAssignmentId = log.TargetId,
                UserId = log.UserId,
                Action = log.Action,
                Comment = log.Comment,
                Timestamp = log.Timestamp
            };
        }

        //Chua fix
        public async Task<List<ApprovalLogDto>> GetLogsByAssignmentIdAsync(int assignmentId)
        {
            return await _context.ApprovalLogs
                .Where(l => l.TargetId == assignmentId)
                .Select(l => new ApprovalLogDto
                {
                    Id = l.Id,
                    KpiAssignmentId = l.TargetId,
                    UserId = l.UserId,
                    Action = l.Action,
                    Comment = l.Comment,
                    Timestamp = l.Timestamp
                }).ToListAsync();
        }

        //AFter Delete
        public async Task<bool> ApproveDeleteAsync(int logId, int approverId, bool approve, string? comment = null)
        {
            var log = await _context.ApprovalLogs.FindAsync(logId);
            if (log == null || log.Action != "RequestDelete") return false;

            if (approve)
            {
                if (log.TargetType == "Template")
                {
                    var template = await _context.KpiTemplates.FindAsync(log.TargetId);
                    if (template != null)
                    {
                        _context.KpiTemplates.Remove(template);
                    }
                }
                else if (log.TargetType == "Item")
                {
                    var item = await _context.KpiItems.FindAsync(log.TargetId);
                    if (item != null)
                    {
                        item.Deleted = true;
                        item.DeletedBy = approverId;
                        item.DeletedDate = DateTime.UtcNow;
                    }
                }

                log.Action = "ApproveDelete";
            }
            else
            {
                log.Action = "RejectDelete";
            }

            log.UserId = approverId;
            log.Comment = comment;
            log.Timestamp = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion


        #region Assignment
        // Giao 1 KPIItem
        public async Task<KpiAssignmentDto> AssignItemAsync(CreateKpiAssignmentDto dto, int createdBy, string creatorRole)
        {
            var item = await _context.KpiItems.FindAsync(dto.KpiItemId);
            if (item == null) throw new Exception("KPI Item not found");

            string status = (creatorRole == "Admin" || creatorRole == "Principal") ? "Approved" : "PendingApproval";

            var assignment = new KPIAssignment
            {
                UserId = dto.UserId,
                UnitId = dto.UnitId,
                KpiItemId = dto.KpiItemId,
                ContributionWeight = dto.ContributionWeight ?? item.Weight,
                Year = dto.Year,
                Status = status,
                CreatedByUserId = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            _context.KpiAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            _context.ApprovalLogs.Add(new ApprovalLog
            {
                TargetType = "KPIItem",
                TargetId = dto.KpiItemId,
                UserId = createdBy,
                Action = "Assigned",
                Comment = status == "Approved" ? "Assigned and auto-approved by Admin/Principal" : "Assigned and pending approval",
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return new KpiAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                UnitId = assignment.UnitId,
                KpiItemId = assignment.KpiItemId,
                ContributionWeight = assignment.ContributionWeight,
                Status = assignment.Status,
                Year = assignment.Year
            };
        }

        // Giao cả Template
        public async Task<List<KpiAssignmentDto>> AssignTemplateAsync(AssignTemplateDto dto, int createdBy, string creatorRole)
        {
            var items = await _context.KpiItems
                .Where(i => i.KpiTemplateId == dto.TemplateId && !i.Deleted)
                .ToListAsync();

            if (!items.Any()) throw new Exception("No KPI items in template");

            string status = (creatorRole == "Admin" || creatorRole == "Principal") ? "Approved" : "PendingApproval";

            var assignments = items.Select(item => new KPIAssignment
            {
                UserId = dto.UserId,
                UnitId = dto.UnitId,
                KpiItemId = item.Id,
                ContributionWeight = dto.DefaultContributionWeight ?? item.Weight,
                Year = dto.Year,
                Status = status,
                CreatedByUserId = createdBy,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            _context.KpiAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();

            _context.ApprovalLogs.Add(new ApprovalLog
            {
                TargetType = "KpiTemplate",
                TargetId = dto.TemplateId,
                UserId = createdBy,
                Action = "Assigned",
                Comment = status == "Approved"
                    ? $"Assigned {assignments.Count} KPI items from template and auto-approved"
                    : $"Assigned {assignments.Count} KPI items from template, pending approval",
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return assignments.Select(a => new KpiAssignmentDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UnitId = a.UnitId,
                KpiItemId = a.KpiItemId,
                ContributionWeight = a.ContributionWeight,
                Status = a.Status,
                Year = a.Year
            }).ToList();
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
                //TargetValue = entity.TargetValue,
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
                    //TargetValue = a.TargetValue,
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
                    //TargetValue = a.TargetValue,
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

            //entity.TargetValue = dto.TargetValue;
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
                //TargetValue = entity.TargetValue,
                ContributionWeight = entity.ContributionWeight,
                ActualResults = entity.ActualResults,
                ComponentScore = entity.ComponentScore,
                Status = entity.Status,
                Year = entity.Year
            };
        }

        public async Task<List<KpiAssignmentDto>> GetAllAssigment()
        {
            return await _context.KpiAssignments
                .Select(a => new KpiAssignmentDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UnitId = a.UnitId,
                    KpiItemId = a.KpiItemId,
                    //TargetValue = a.TargetValue,
                    ContributionWeight = a.ContributionWeight,
                    ActualResults = a.ActualResults,
                    ComponentScore = a.ComponentScore,
                    Status = a.Status,
                    Year = a.Year
                })
                .ToListAsync();
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
