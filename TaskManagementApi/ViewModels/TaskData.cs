using System;
using System.Collections.Generic;

namespace TaskManagementApi.ViewModels
{
    public class TaskData
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? Deadline { get; set; }
        public int? PriorityId { get; set; }
        public string PriorityName { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; }
        public int? TaskCreatorId { get; set; }
        public string TaskCreatorName { get; set; }
        public int? AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }
        public int? AssignedGroupId { get; set; }
        public string GroupName { get; set; }
    }
    
    public class UpdateTaskData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string ResultsNotes { get; set; }
    }
}