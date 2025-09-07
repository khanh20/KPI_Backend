using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Auth.Domain
{
    [Table(nameof(User))]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UnitId { get; set; }

        [MaxLength(100)]
        [Required]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        [Required]
        public string? UserName { get; set; }

        [MaxLength(256)]
        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(256)]
        public string? Email { get; set; }


        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(256)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Chức vụ trong trường (Rector, ViceRector, Dean, ...)
        /// Dùng constants ở PositionTypes
        /// </summary>
        public int Position { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }


        // Audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }

     
    }
}
