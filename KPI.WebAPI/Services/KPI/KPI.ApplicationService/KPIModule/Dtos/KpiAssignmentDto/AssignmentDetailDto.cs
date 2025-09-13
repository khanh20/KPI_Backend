using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.KpiAssignmentDto
{
    public class AssignmentDetailsDto
{
    public int UserId { get; set; }
    public int UnitId { get; set; }
    public int Year { get; set; }
    public string Status { get; set; } = null!;
    public List<AssignmentItemDto> KpiItems { get; set; } = new();
}

public class AssignmentItemDto
{
    public int KpiItemId { get; set; }   // liên kết với bảng KpiItem
    public float ContributionWeight { get; set; }
    public float ActualResults { get; set; }
    public float ComponentScore { get; set; }

    public string KpiType { get; set; }
    public float Weight { get; set; }

    public float TargetValue { get; set; }
    public string? CalculationFormula { get; set; }
    }


 

}
