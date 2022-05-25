using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApi.Data.Models
{
    [Table("task")]
    public class Task
    {
        [Key]
        [Column("task_id")]
        public int TaskId { get; set; }

        [Column("title")]
        [Required]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("deadline")]
        public DateTime? Deadline { get; set; }

        [Column("priority_id")]
        public int? PriorityId { get; set; }

        [Column("status_id")]
        public int? StatusId { get; set; }

        [Column("task_creator_id")]
        public int? TaskCreatorId { get; set; }

        [Column("completion_date")]
        public DateTime? CompletionDate { get; set; }

        [Column("results_notes")]
        public string ResultsNotes { get; set; }

        [ForeignKey(nameof(PriorityId))]
        public TaskPriority TaskPriority { get; set; }

        [ForeignKey(nameof(StatusId))]
        public TaskStatus TaskStatus { get; set; }

        [ForeignKey(nameof(TaskCreatorId))]
        public User TaskCreator { get; set; }
    }
}