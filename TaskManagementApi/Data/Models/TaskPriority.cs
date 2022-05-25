using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("task_priority")]
    public class TaskPriority
    {
        [Key]
        [Column("priority_id")]
        public int PriorityId { get; set; }

        [Column("priority_name")]
        [Required]
        [MaxLength(255)]
        public string PriorityName { get; set; }
    }
}