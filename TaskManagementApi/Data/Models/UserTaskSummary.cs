using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("user_task_summary")]
    public class UserTaskSummary
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Key]
        [Column("task_date")]
        public DateTime TaskDate { get; set; }

        [Column("task_count")]
        public int TaskCount { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}