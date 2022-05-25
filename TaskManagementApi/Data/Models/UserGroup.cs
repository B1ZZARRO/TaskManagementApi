using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("user_group")]
    public class UserGroup
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("group_name")]
        [Required]
        [MaxLength(255)]
        public string GroupName { get; set; }
    }
}