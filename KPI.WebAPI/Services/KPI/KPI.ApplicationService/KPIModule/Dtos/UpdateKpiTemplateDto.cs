using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.ApplicationService.KPIModule.Dtos
{
    public class UpdateKpiTemplateDto
    {
        [Required]
        [MaxLength(256)]
        public string TemplateName { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        [Required]
        public int Year { get; set; }
    }
}
