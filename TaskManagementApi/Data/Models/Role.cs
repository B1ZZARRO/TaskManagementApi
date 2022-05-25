using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("role")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("role_name")]
        [Required]
        [MaxLength(255)]
        public string RoleName { get; set; }
    }
}