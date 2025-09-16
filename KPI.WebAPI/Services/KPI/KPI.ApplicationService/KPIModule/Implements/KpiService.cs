
using ClosedXML.Excel;
using KPI.ApplicationService.KPIModule.Abstract;
using KPI.ApplicationService.KPIModule.Dtos;
using KPI.ApplicationService.KPIModule.Dtos.ApprovalDto;
using KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto;
using KPI.ApplicationService.KPIModule.Dtos.UnitDto;
using KPI.ApplicationService.KPIModule.Dtos.ViolationDto;
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
                TargetValue = item.TargetValue,
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
        // Sửa đổi phương thức ApproveAsync để xử lý phê duyệt hàng loạt
        public async Task ApproveBulkAsync(List<int> assignmentIds, string comment, int approverId)
        {
            // Lấy tất cả các assignment cần phê duyệt
            var assignmentsToApprove = await _context.KpiAssignments
                .Where(a => assignmentIds.Contains(a.Id))
                .ToListAsync();

            if (assignmentsToApprove == null || !assignmentsToApprove.Any())
            {
                throw new Exception("Assignments not found");
            }

            foreach (var assignment in assignmentsToApprove)
            {
                // Cập nhật trạng thái
                assignment.Status = "Approved";
                assignment.ModifiedBy = approverId;
                assignment.ModifiedDate = DateTime.UtcNow;

                // Ghi log cho từng assignment
                var log = new ApprovalLog
                {
                    TargetType = "KpiAssignment",
                    TargetId = assignment.Id,
                    UserId = approverId,
                    Action = "Approved",
                    Comment = comment,
                    Timestamp = DateTime.UtcNow
                };
                _context.ApprovalLogs.Add(log);
            }

            // Lưu tất cả các thay đổi vào database
            await _context.SaveChangesAsync();
        }

        // Sửa đổi phương thức RejectAsync để xử lý từ chối hàng loạt
        public async Task RejectBulkAsync(List<int> assignmentIds, string comment, int approverId)
        {
            // Lấy tất cả các assignment cần từ chối
            var assignmentsToReject = await _context.KpiAssignments
                .Where(a => assignmentIds.Contains(a.Id))
                .ToListAsync();

            if (assignmentsToReject == null || !assignmentsToReject.Any())
            {
                throw new Exception("Assignments not found");
            }

            foreach (var assignment in assignmentsToReject)
            {
                // Cập nhật trạng thái
                assignment.Status = "Rejected";
                assignment.ModifiedBy = approverId;
                assignment.ModifiedDate = DateTime.UtcNow;

                // Ghi log cho từng assignment
                var log = new ApprovalLog
                {
                    TargetType = "KpiAssignment",
                    TargetId = assignment.Id,
                    UserId = approverId,
                    Action = "Rejected",
                    Comment = comment,
                    Timestamp = DateTime.UtcNow
                };
                _context.ApprovalLogs.Add(log);
            }

            // Lưu tất cả các thay đổi vào database
            await _context.SaveChangesAsync();
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


            var existing = await _context.KpiAssignments
            .FirstOrDefaultAsync(a => a.UserId == dto.UserId
                           && a.KpiItemId == dto.KpiItemId
                           && a.Year == dto.Year);

            if (existing != null)
            {
                throw new InvalidOperationException("KPI Item này đã được giao cho người dùng trong năm này.");
            }

            var item = await _context.KpiItems.FindAsync(dto.KpiItemId);
            if (item == null) throw new Exception("KPI Item not found");


            //string status = (creatorRole == "Admin" || creatorRole == "Principal") ? "Approved" : "PendingApproval";
            string status = "Assigned";

            var assignment = new KPIAssignment
            {
                UserId = dto.UserId,
                UnitId = dto.UnitId,
                KpiItemId = dto.KpiItemId,
                ContributionWeight = dto.ContributionWeight.HasValue
                    ? dto.ContributionWeight.Value
                    : 100f,

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
                Comment = "KPI item assigned, waiting for self-evaluation",
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

            string status =  "Assigned";

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


        public async Task<List<KPIAssignment>> SelfEvaluate(int userId, SelfEvaluateDto dto)
        {
            var userAssignments = await _context.KpiAssignments
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (!userAssignments.Any())
                throw new KeyNotFoundException("Không tìm thấy KPI assignments cho người này");

            // Lấy tất cả KPIItems
            var kpiItems = await _context.KpiItems.ToListAsync();

            // Lấy assignment đang self-evaluate
            var assignment = userAssignments.FirstOrDefault(a => a.Id == dto.AssignmentId);
            if (assignment == null)
                throw new KeyNotFoundException("Không tìm thấy KPI assignment này");

            // Kiểm tra nếu đã đánh giá rồi
            if (assignment.Status == "Evaluated")
                throw new InvalidOperationException("KPI này đã được đánh giá, không thể đánh giá lại");

            // Cập nhật ActualResults và trạng thái
            assignment.ActualResults = dto.ActualResults;
            assignment.Status = "Evaluated";
            assignment.ModifiedDate = DateTime.Now;
            assignment.ModifiedBy = userId;

            // Tính ComponentScore cho tất cả assignments của user
            CalculateUserKpiScores(userAssignments, assignment);
            await _context.SaveChangesAsync();

            return new List<KPIAssignment> { assignment };
        }


        public void CalculateUserKpiScores(List<KPIAssignment> assignments, KPIAssignment currentAssignment, bool needNormalize = true)
        {
            if (assignments == null || assignments.Count == 0) return;

            
            var kpiItems = _context.KpiItems.ToList();
            // lấy assignmentId ở trên hàm SelfEvaluate để tính điểm KPI
            var assignmentToEvaluate = assignments
                .FirstOrDefault(a => a.Id == currentAssignment.Id);
            // Join bảng KpiItem để lấy data
            var assignmentWithItem = assignments
                .Join(
                    kpiItems,
                    a => a.KpiItemId,
                    k => k.Id,
                    (a, k) => new
                    {
                        AssignmentId = a.Id,
                        Assignment = a,
                        ItemType = k.KpiType,
                        Weight = k.Weight,
                        TargetValue = k.TargetValue
                    }
                ).ToList();

            // tính trọng số trước chuẩn hoá ( tỉ lệ tham gia / trọng số )
            var preNormWeights = assignmentWithItem.ToDictionary(
                x => x.AssignmentId,
                x => x.Assignment.ContributionWeight / x.Weight
            );

            // tính tổng trọng số trước chuẩn hoá theo từng KPIType 
            var sumPreNormByType = assignmentWithItem
                .GroupBy(x => x.ItemType)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => preNormWeights[x.AssignmentId])
                );
            // tính tổng trọng sô ( những cái có tỉ lệ tham gia > 0)
            var sumWeight = assignmentWithItem
                .Where(x => x.Assignment.ContributionWeight > 0)
                .GroupBy(x => x.ItemType)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.Weight)
                );
            // tính tỉ lệ điều chỉnh
            var adjustFactorByType = sumPreNormByType
                .Where(kv => kv.Value > 0 && sumWeight.ContainsKey(kv.Key))
                .ToDictionary(
                    kv => kv.Key,
                    kv => sumWeight[kv.Key] / kv.Value
                );

            // tính component score
            var currentAssignmentWithItem = assignmentWithItem
                .Where(x => x.AssignmentId == currentAssignment.Id);

            foreach (var x in currentAssignmentWithItem)
            {
                float score = 0;

                if (x.Assignment.ContributionWeight > 0)
                {
                    float adjustFactor = 1;
                    if (needNormalize && adjustFactorByType.ContainsKey(x.ItemType))
                    {
                        adjustFactor = adjustFactorByType[x.ItemType];
                    }

                    float finishWeight = preNormWeights[x.AssignmentId] * adjustFactor;
                    score = (x.TargetValue > 0)
                        ? (x.Assignment.ActualResults / x.TargetValue) * finishWeight
                        : 0;
                }
                else
                {
                    score = (x.TargetValue > 0)
                        ? (x.Assignment.ActualResults / x.TargetValue) * x.Weight
                        : 0;
                }

                x.Assignment.ComponentScore = score;
            }

        }




        public async Task<KpiTypeScoreResultDto> GetTotalComponentScoreByUser(int userId)
        {
            // 1. Lấy assignments đã evaluated
            var query = await _context.KpiAssignments
                .Where(a => a.UserId == userId && a.Status == "Evaluated")
                .Join(
                    _context.KpiItems,
                    assignment => assignment.KpiItemId,
                    item => item.Id,
                    (assignment, item) => new { assignment, item }
                )
                .ToListAsync();

            if (!query.Any())
            {
                return null;
            }

            var userIdValue = query.First().assignment.UserId;
            var unitIdValue = query.First().assignment.UnitId;
            var yearValue = query.First().assignment.Year;

            // 2. Nhóm theo KpiType (Chức năng / Mục tiêu)
            var scores = query
                .GroupBy(x => x.item.KpiType)
                .Select(g => new KpiTypeScoreDto
                {
                    KpiType = g.Key,
                    TotalComponentScore = g.Sum(x => x.assignment.ComponentScore)
                })
                .ToList();

            // 3. Lấy Tuân thủ từ KpiViolationCore
            var violation = await _context.KpiViolationCores
                .FirstOrDefaultAsync(v => v.UserId == userIdValue && v.UnitId == unitIdValue);

            var compliance = violation?.TotalDeduction ?? 0;

            scores.Add(new KpiTypeScoreDto
            {
                KpiType = "Tuân thủ",
                TotalComponentScore = compliance
            });

            // 4. Tính FinishTotal = Objective + Functional - Compliance
            var functional = scores.FirstOrDefault(x => x.KpiType == "Chức năng")?.TotalComponentScore ?? 0;
            var objective = scores.FirstOrDefault(x => x.KpiType == "Mục tiêu")?.TotalComponentScore ?? 0;
            var complianceScore = compliance;

            var final = objective + functional - complianceScore;

            return new KpiTypeScoreResultDto
            {
                UserId = userIdValue,
                UnitId = unitIdValue,
                Year = yearValue,
                ScoresByType = scores,
                FinishTotal = final
            };
        }





        public async Task<List<KpiTypeScoreResultDto>> GetAllKpiScores()
        {
            // 1. Lấy điểm KPI đã evaluated 
            var scores = await _context.KpiAssignments
                .Where(a => a.Status == "Evaluated")
                .Join(
                    _context.KpiItems,
                    assignment => assignment.KpiItemId,
                    item => item.Id,
                    (assignment, item) => new { assignment, item }
                )
              
                .GroupBy(x => new { x.assignment.UserId, x.assignment.UnitId, x.item.KpiType, x.assignment.Year })
                .Select(g => new
                {
                    g.Key.UserId,
                    g.Key.UnitId,
                    g.Key.Year,
                    g.Key.KpiType,
                    Total = g.Sum(x => x.assignment.ComponentScore)
                })
                .ToListAsync();

            // 2. Lấy dữ liệu TotalDeduction từ bảng KpiViolationCore
            var violationScores = await _context.KpiViolationCores.ToListAsync();

            // 3. Gom nhóm và build kết quả
            var result = scores
                .GroupBy(x => new { x.UserId, x.UnitId, x.Year })
                .Select(g =>
                {
                    var compliance = violationScores
                        .FirstOrDefault(v => v.UserId == g.Key.UserId && v.UnitId == g.Key.UnitId)?.TotalDeduction ?? 0;

                    var scoresByType = g.Select(s => new KpiTypeScoreDto
                    {
                        KpiType = s.KpiType, // Chức năng / Mục tiêu
                        TotalComponentScore = s.Total
                    }).ToList();

                    // Thêm Tuân thủ từ bảng KpiViolationCore
                    scoresByType.Add(new KpiTypeScoreDto
                    {
                        KpiType = "Tuân thủ",
                        TotalComponentScore = compliance
                    });

                    // Tính FinishTotal theo công thức
                    var functional = scoresByType.FirstOrDefault(x => x.KpiType == "Chức năng")?.TotalComponentScore ?? 0;
                    var objective = scoresByType.FirstOrDefault(x => x.KpiType == "Mục tiêu")?.TotalComponentScore ?? 0;
                    var complianceScore = compliance;

                    var final = objective + functional - complianceScore;

                    return new KpiTypeScoreResultDto
                    {
                        UserId = g.Key.UserId,
                        UnitId = g.Key.UnitId,
                        Year = g.Key.Year,
                        ScoresByType = scoresByType,
                        FinishTotal = final
                    };
                })
                .ToList();

            // 4. Lưu vào bảng KPIScore
            foreach (var r in result)
            {
                var functional = r.ScoresByType.FirstOrDefault(x => x.KpiType == "Chức năng")?.TotalComponentScore ?? 0;
                var objective = r.ScoresByType.FirstOrDefault(x => x.KpiType == "Mục tiêu")?.TotalComponentScore ?? 0;
                var compliance = r.ScoresByType.FirstOrDefault(x => x.KpiType == "Tuân thủ")?.TotalComponentScore ?? 0;

                var finalScore = objective + functional - compliance;

                var kpiScore = new KPIScore
                {
                    UserId = r.UserId,
                    UnitId = r.UnitId,
                    Year = r.Year,
                    TotalFunctionalScore = functional,
                    TotalObjectiveScore = objective,
                    TotalComplianceScore = compliance,
                    FinalScore = finalScore,
                    Status = 1 // Finalized
                };

                _context.KpiScores.Add(kpiScore);
            }

            await _context.SaveChangesAsync();

            return result;
        }


        //Get  tất cả Assignment trong một Unit
        public async Task<List<AssignmentDetailsDto>> GetAssignmentsByUnitAsync(int unitId, int year)
        {
            var assignments = await _context.KpiAssignments
                .Join(_context.KpiItems,
                    a => a.KpiItemId,
                    i => i.Id,
                   (a, i) => new {a, i}
                )
                .Where(x => x.a.UnitId == unitId && x.a.Year == year)
                .GroupBy(x => new { x.a.UserId, x.a.UnitId, x.a.Year, x.a.Status })
                .Select(g => new AssignmentDetailsDto
                {
                    UserId = g.Key.UserId,
                    UnitId = g.Key.UnitId,
                    Year = g.Key.Year,
                    Status = g.Key.Status,
                    KpiItems = g.Select(x => new AssignmentItemDto
                    {
                        KpiItemId = x.a.KpiItemId,
                        ContributionWeight = x.a.ContributionWeight,
                        ActualResults = x.a.ActualResults,
                        ComponentScore = x.a.ComponentScore,
                        Weight = x.i.Weight,
                        TargetValue = x.i.TargetValue,
                        CalculationFormula = x.i.CalculationFormula,
                        KpiType = x.i.KpiType,

                    }).ToList()
                })
                .ToListAsync();

            return assignments;
        }


        //Get tất cả Assignment của member thuộc quyền tôi
        public async Task<List<AssignmentDetailsDto>> GetAssignmentsByUnitMembersAsync(int headOfUnitId, int year)
        {
            // Xác định đơn vị của trưởng đơn vị
            var unit = await _context.Units.FirstOrDefaultAsync(u => u.HeadOfUnitId == headOfUnitId);
            if (unit == null) return new List<AssignmentDetailsDto>();

            var assignments = await _context.KpiAssignments
                .Join(_context.KpiItems,
                      a => a.KpiItemId,
                      i => i.Id,
                      (a, i) => new { a, i })
                .Where(x =>  x.a.UnitId == unit.Id && x.a.Year == year && x.a.UserId != unit.HeadOfUnitId)
                .GroupBy(x => new { x.a.UserId, x.a.UnitId, x.a.Year, x.a.Status})
                .Select(g => new AssignmentDetailsDto
                {
                    UserId = g.Key.UserId,
                    UnitId = g.Key.UnitId,
                    Year = g.Key.Year,
                    Status = g.Key.Status,
                    KpiItems = g.Select(x => new AssignmentItemDto
                    {
                        AssignmentId = x.a.Id,
                        KpiItemId = x.a.KpiItemId,
                        ContributionWeight = x.a.ContributionWeight,
                        ActualResults = x.a.ActualResults,
                        ComponentScore = x.a.ComponentScore,
                        Weight = x.i.Weight,
                        TargetValue = x.i.TargetValue,
                        CalculationFormula = x.i.CalculationFormula,
                        KpiType     = x.i.KpiType,
                    }).ToList()
                })
                .ToListAsync();

            return assignments;
        }




        //Get  Assignment của trưởng đơn vị trong một Unit

        public async Task<List<UnitAssignmentDetailsDto>> GetUnitAssignmentsAsync(int year)
        {
            var assignments = await _context.KpiAssignments
                .Join(_context.Units,
                      a => a.UnitId,
                      u => u.Id,
                      (a, u) => new { a, u })
                .Join(_context.KpiItems,
                    b => b.a.KpiItemId,
                    i => i.Id,
                    (b, i) => new {b.a, b.u, i})
                .Where(x => x.a.Year == year && x.a.UserId == x.u.HeadOfUnitId) // chỉ KPI đơn vị
                .GroupBy(x => new { x.a.UnitId, x.u.Name, x.a.UserId, x.a.Year, x.a.Status })
                .Select(g => new UnitAssignmentDetailsDto
                {
                    UnitId = g.Key.UnitId,
                    UnitName = g.Key.Name,
                    UserId = g.Key.UserId,   // trưởng đơn vị
                    Year = g.Key.Year,
                    Status = g.Key.Status,
                    KpiItems = g.Select(x => new AssignmentItemDto
                    {
                        AssignmentId = x.a.Id,
                        KpiItemId = x.a.KpiItemId,
                        TargetValue = x.i.TargetValue,
                        Weight = x.i.Weight,
                        CalculationFormula = x.i.CalculationFormula,
                        ContributionWeight = x.a.ContributionWeight,
                        KpiType = x.i.KpiType,
                        ActualResults = x.a.ActualResults,
                        ComponentScore = x.a.ComponentScore
                    }).ToList()
                })
                .ToListAsync();

            return assignments;
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

        #region Export Excel

        public async Task<byte[]> ExportAssignmentToExcelAsync(int? unitId, int? userId, int year)
        {
            var query = from a in _context.KpiAssignments
                        join i in _context.KpiItems on a.KpiItemId equals i.Id
                        where a.Year == year && !a.Deleted
                        select new
                        {
                            a.UnitId,
                            a.UserId,
                            i.KpiName,
                            i.KpiType,
                            i.Weight,
                            i.TargetValue,
                            i.CalculationFormula,
                            i.DeadLine,
                            a.ActualResults,
                            a.ComponentScore,
                            a.ContributionWeight
                        };

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);
            else if (unitId.HasValue)
                query = query.Where(x => x.UnitId == unitId.Value);

            var data = await query.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("KPI");
                int row = 1;

                // Header chung
                worksheet.Cell(row, 1).Value = "STT";
                worksheet.Cell(row, 2).Value = "Tên KPI";
                worksheet.Cell(row, 3).Value = "Loại KPI";
                worksheet.Cell(row, 4).Value = "Trọng số (%)";
                //worksheet.Cell(row, 5).Value = "Phân bổ (%)";
                worksheet.Cell(row, 5).Value = "Giá trị mục tiêu";
                worksheet.Cell(row, 6).Value = "Công thức tính";
                worksheet.Cell(row, 7).Value = "Kết quả thực tế";
                worksheet.Cell(row, 8).Value = "Điểm KPI";

                worksheet.Range(row, 1, row, 8).Style.Font.SetBold();
                row++;

                // Group theo loại KPI
                var grouped = data.GroupBy(x => x.KpiType).ToList();

                foreach (var group in grouped)
                {
                    // Dòng tiêu đề loại KPI
                    worksheet.Cell(row, 1).Value = group.Key;
                    worksheet.Range(row, 1, row, 8).Merge().Style
                        .Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.LightGray);
                    row++;

                    int stt = 1;
                    foreach (var item in group)
                    {
                        worksheet.Cell(row, 1).Value = stt++;
                        worksheet.Cell(row, 2).Value = item.KpiName;
                        worksheet.Cell(row, 3).Value = item.KpiType;

                        // Trọng số
                        //worksheet.Cell(row, 4).Value = item.Weight;
                        //worksheet.Cell(row, 4).Style.NumberFormat.Format = "0.00\\%";

                        // Phân bổ = Weight x ContributionWeight
                        worksheet.Cell(row, 4).Value = (item.Weight * item.ContributionWeight) + "%";
                        worksheet.Cell(row, 4).Style.NumberFormat.Format = "0.00\\%";

                        worksheet.Cell(row, 5).Value = item.TargetValue;
                        worksheet.Cell(row, 6).Value = item.CalculationFormula;
                        worksheet.Cell(row, 7).Value = item.ActualResults;
                        worksheet.Cell(row, 8).Value = item.ComponentScore;

                        row++;
                    }

                    row++; // cách 1 dòng giữa các nhóm
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> ExportTemplateToExcelAsync(int? templateId)
        {
            var query = from t in _context.KpiTemplates
                        join i in _context.KpiItems on t.Id equals i.KpiTemplateId
                        where !t.Deleted
                        select new
                        {
                            t.Id,
                            t.Year,
                            t.TemplateName,
                            i.KpiName,
                            i.KpiType,
                            i.Weight,
                            i.TargetValue,
                            i.CalculationFormula,
                        };

            // Lọc theo TemplateId nếu có truyền vào
            if (templateId.HasValue)
            {
                query = query.Where(x => x.Id == templateId.Value);
            }

            var data = await query.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("KPI");
                int row = 1;

                // Header
                worksheet.Cell(row, 1).Value = "STT";
                worksheet.Cell(row, 2).Value = "Tên KPI";
                worksheet.Cell(row, 3).Value = "Loại KPI";
                worksheet.Cell(row, 4).Value = "Trọng số (%)";
                worksheet.Cell(row, 5).Value = "Giá trị mục tiêu";
                worksheet.Cell(row, 6).Value = "Công thức tính";
                worksheet.Cell(row, 7).Value = "Kết quả thực tế";
                worksheet.Cell(row, 8).Value = "Điểm KPI";

                worksheet.Range(row, 1, row, 8).Style.Font.SetBold();
                row++;

                // Group theo loại KPI
                var grouped = data.GroupBy(x => x.KpiType).ToList();

                foreach (var group in grouped)
                {
                    // Dòng tiêu đề loại KPI
                    worksheet.Cell(row, 1).Value = group.Key;
                    worksheet.Range(row, 1, row, 8).Merge().Style
                        .Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.LightGray);
                    row++;

                    int stt = 1;
                    foreach (var item in group)
                    {
                        worksheet.Cell(row, 1).Value = stt++;
                        worksheet.Cell(row, 2).Value = item.KpiName;
                        worksheet.Cell(row, 3).Value = item.KpiType;

                        worksheet.Cell(row, 4).Value = item.Weight;
                        worksheet.Cell(row, 4).Style.NumberFormat.Format = "0.00\\%";

                        worksheet.Cell(row, 5).Value = item.TargetValue;
                        worksheet.Cell(row, 6).Value = item.CalculationFormula;

                        row++;
                    }

                    row++; // cách 1 dòng giữa các nhóm
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        #endregion


        #region Violation
        public async Task<KPIViolation> CreateViolationAsync(CreateKpiViolationDto dto)
        {
            var violation = new KPIViolation
            {
                UserId = dto.UserId,
                UnitId = dto.UnitId,
                CategoryId = dto.CategoryId,
                ViolationCount = dto.ViolationCount,
                ViolationDate = dto.ViolationDate
            };

            _context.KpiViolations.Add(violation);
            await _context.SaveChangesAsync();
            await SaveUserViolationSummary(dto.UserId, dto.UnitId);

            return new KPIViolation
            {
                Id = violation.Id,
                UserId = violation.UserId,
                UnitId = violation.UnitId,
                CategoryId = violation.CategoryId,
                ViolationCount = violation.ViolationCount,
                DeductionScore = violation.DeductionScore,
                ViolationDate = violation.ViolationDate
            };
        }
        public async Task<ViolationCategoryDto> CreateViolationCategory(CreateKpiViolationCateDto dto)
        {
            var entity = new KpiViolationCategory
            {
                Name = dto.Name,
                TargetValue = dto.TargetValue,
                CalculationFormula = dto.CalculationFormula,
            };

            _context.KpiViolationCategories.Add(entity);
            await _context.SaveChangesAsync();

            return new ViolationCategoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                TargetValue = entity.TargetValue,
                CalculationFormula = entity.CalculationFormula,
            };
        }
        public async Task<ViolationLevelDto> CreateViolationLevel(CreateKpiViolationLevelDto dto)
        {
            var entity = new KpiViolationLevel
            {
                CategoryId = dto.CategoryId,
                MaxDeduction = dto.MaxDeduction,
                ViolationCount = dto.ViolationCount,
                Description = dto.Description
            };


            _context.KpiViolationLevels.Add(entity);
            await _context.SaveChangesAsync();


            return new ViolationLevelDto
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                MaxDeduction = entity.MaxDeduction,
                ViolationCount = entity.ViolationCount,
                Description = entity.Description
            };
        }
        public async Task<IEnumerable<ViolationLevelDto>> GetAllViolationLevel()
        {
            return await _context.KpiViolationLevels
                .Select(x => new ViolationLevelDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    MaxDeduction = x.MaxDeduction,
                    ViolationCount = x.ViolationCount,
                    Description = x.Description
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<ViolationLevelDto>> GetViolationLevelByCategoryId(int categoryId)
        {
            return await _context.KpiViolationLevels
                .Where(x => x.CategoryId == categoryId)
                .Select(x => new ViolationLevelDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    MaxDeduction = x.MaxDeduction,
                    ViolationCount = x.ViolationCount,
                    Description = x.Description
                })
                .ToListAsync();
        }
        public IEnumerable<ViolationCategoryDto> GetAllViolationCategoryWithLevels()
        {

                return _context.KpiViolationCategories
                    .Select(c => new ViolationCategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        TargetValue = c.TargetValue,
                        CalculationFormula = c.CalculationFormula,
                        Levels = _context.KpiViolationLevels
                            .Where(l => l.CategoryId == c.Id)
                            .Select(l => new ViolationLevelDto
                            {
                                Id = l.Id,
                                CategoryId = l.CategoryId,
                                MaxDeduction = l.MaxDeduction,
                                ViolationCount = l.ViolationCount,
                                Description = l.Description
                            }).ToList()
                    })
                    .ToList();
         }

        public async Task<ViolationCategoryDto?> GetViolationCategoryById(int id)
{
    return await _context.KpiViolationCategories
        .Where(x => x.Id == id)
        .Select(x => new ViolationCategoryDto
        {
            Id = x.Id,
            Name = x.Name,
            TargetValue = x.TargetValue,
            CalculationFormula = x.CalculationFormula,
            Levels = _context.KpiViolationLevels
                .Where(l => l.CategoryId == x.Id)
                .Select(l => new ViolationLevelDto
                {
                    Id = l.Id,
                    CategoryId = l.CategoryId,
                    ViolationCount = l.ViolationCount,
                    MaxDeduction = l.MaxDeduction
                })
                .ToList()
        })
        .FirstOrDefaultAsync();
}

        public async Task<bool> DeleteViolationCategory(int id)
        {
            var entity = await _context.KpiViolationCategories.FindAsync(id);
            if (entity == null)
            {
                return false; 
            }

            _context.KpiViolationCategories.Remove(entity);
            await _context.SaveChangesAsync();
            return true; 
        }
        public async Task<ViolationSummaryResultDto> CalculateUserViolation(int userId)
        {
            var grouped = await _context.KpiViolations
                .Where(v => v.UserId == userId)
                .GroupBy(v => v.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    TotalCount = g.Sum(x => x.ViolationCount)
                })
                .ToListAsync();

            var categoryIds = grouped.Select(g => g.CategoryId).ToList();

            var categories = await _context.KpiViolationCategories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            var levels = await _context.KpiViolationLevels
                .Where(l => categoryIds.Contains(l.CategoryId))
                .ToListAsync();

            var result = grouped.Select(g =>
            {
                var category = categories.FirstOrDefault(c => c.Id == g.CategoryId);

                var level = levels
                    .Where(l => l.CategoryId == g.CategoryId)
                    .OrderByDescending(l => l.ViolationCount)
                    .FirstOrDefault(l => l.ViolationCount == g.TotalCount)
                    ?? levels
                        .Where(l => l.CategoryId == g.CategoryId && l.ViolationCount <= g.TotalCount)
                        .OrderByDescending(l => l.ViolationCount)
                        .FirstOrDefault();

                return new SumViolationDto
                {
                    CategoryId = g.CategoryId,
                    CategoryName = category?.Name,
                    TotalCount = g.TotalCount,
                    TotalComponent = level?.MaxDeduction ?? 0
                };
            }).ToList();

            return new ViolationSummaryResultDto
            {
                Details = result,
                TotalDeduction = (int)result.Sum(r => r.TotalComponent)
            };
        }
        public async Task SaveUserViolationSummary(int userId, int unitId)
        {
            // Gọi hàm tính toán
            var summary = await CalculateUserViolation(userId);

            // Kiểm tra xem đã có bản ghi chưa
            var existingCore = await _context.KpiViolationCores
                .FirstOrDefaultAsync(c => c.UserId == userId && c.UnitId == unitId);

            if (existingCore != null)
            {
                existingCore.TotalDeduction = summary.TotalDeduction;
                existingCore.LastUpdated = DateTime.UtcNow;
                _context.KpiViolationCores.Update(existingCore);
            }
            else
            {
                var newCore = new KpiViolationCore
                {
                    UserId = userId,
                    UnitId = unitId,
                    TotalDeduction = summary.TotalDeduction,
                    LastUpdated = DateTime.UtcNow
                };
                await _context.KpiViolationCores.AddAsync(newCore);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<KpiViolationCore>> GetAllTotalDeductions()
        {
            return await _context.KpiViolationCores
                .OrderByDescending(c => c.LastUpdated)
                .ToListAsync();
        }

        public async Task<List<KPIViolation>> GetViolationsByUserIdAsync(int userId)
        {
            var violations = await _context.KpiViolations
                .Where(v => v.UserId == userId)
                .ToListAsync();

            return violations;
        }

        

    }



    #endregion

}

