using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table("KpiAssignment")]
    public class KPIAssignment
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public int KpiItemId { get; set; }
        public float TargetValue { get; set; }  
        public float ContributionWeight { get; set; } // tỷ lệ đóng góp
        public float ActualResults { get; set; } // kết quả thực tế
        public float ComponentScore { get; set; } // điểm thành phần
        public string Status { get; set; } = null!;
        public int Year { get; set; }
        public int CreatedByUserId { get; set; }
        //Audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }

}
