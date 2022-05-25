using System;

namespace TaskManagementApi.ViewModels
{
    public class UserAuthData
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    
    public class TaskCreationData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int TaskCreatorId { get; set; }
    }
}