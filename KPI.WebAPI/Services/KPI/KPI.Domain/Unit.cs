using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Domain
{
    [Table("Units")]
    public class Unit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Trưởng đơn vị (UserId)
        /// </summary>
        public int? HeadOfUnitId { get; set; }
    }
}
