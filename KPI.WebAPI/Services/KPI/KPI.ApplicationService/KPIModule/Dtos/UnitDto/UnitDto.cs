using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos.UnitDto
{
    public class UnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? HeadOfUnitId { get; set; }
    }
}
