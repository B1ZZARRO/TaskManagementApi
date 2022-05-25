using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("task_status")]
    public class TaskStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("status_name")]
        [Required]
        [MaxLength(255)]
        public string StatusName { get; set; }
    }
}