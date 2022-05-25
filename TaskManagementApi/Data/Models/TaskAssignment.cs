using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("task_assignments")]
    public class TaskAssignment
    {
        [Key]
        [Column("assignment_id")]
        public int AssignmentId { get; set; }

        [Column("task_id")]
        public int TaskId { get; set; }

        [Column("assigned_user_id")]
        public int AssignedUserId { get; set; }

        [Column("assigned_group_id")]
        public int AssignedGroupId { get; set; }

        [ForeignKey(nameof(TaskId))]
        public Task Task { get; set; }

        [ForeignKey(nameof(AssignedUserId))]
        public User AssignedUser { get; set; }

        [ForeignKey(nameof(AssignedGroupId))]
        public UserGroup AssignedGroup { get; set; }
    }
}