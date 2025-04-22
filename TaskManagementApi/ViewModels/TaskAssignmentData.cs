using System;

namespace TaskManagementApi.ViewModels
{
    public class TaskAssignmentData
    {
        public int TaskId { get; set; }
        public int? AssignedUserId { get; set; }
        public int? AssignedGroupId { get; set; }
    }
    
    public class TaskAssignmentData1
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string ResultsNotes { get; set; }
        
        public int UserId { get; set; }
        public string FullName { get; set; }
        
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
}