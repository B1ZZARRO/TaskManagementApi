using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data.Models;

namespace TaskManagementApi.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        // Таблицы базы данных
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<UserTaskSummary> UserTaskSummaries { get; set; }
        public DbSet<StorageItem> StorageItems { get; set; }
        public DbSet<StorageMovement> StorageMovements { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceComponent> DeviceComponents { get; set; }
        public DbSet<ComponentItem> ComponentItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка Role
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);
            
            modelBuilder.Entity<Role>()
                .Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(255);

            // Настройка UserGroup
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => ug.GroupId);

            modelBuilder.Entity<UserGroup>()
                .Property(ug => ug.GroupName)
                .IsRequired()
                .HasMaxLength(255);

            // Настройка TaskPriority
            modelBuilder.Entity<TaskPriority>()
                .HasKey(tp => tp.PriorityId);

            modelBuilder.Entity<TaskPriority>()
                .Property(tp => tp.PriorityName)
                .IsRequired()
                .HasMaxLength(255);

            // Настройка TaskStatus
            modelBuilder.Entity<TaskStatus>()
                .HasKey(ts => ts.StatusId);

            modelBuilder.Entity<TaskStatus>()
                .Property(ts => ts.StatusName)
                .IsRequired()
                .HasMaxLength(255);

            // Настройка User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserGroup)
                .WithMany()
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Настройка Task
            modelBuilder.Entity<Task>()
                .HasKey(t => t.TaskId);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskPriority)
                .WithMany()
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskStatus)
                .WithMany()
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskCreator)
                .WithMany()
                .HasForeignKey(t => t.TaskCreatorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Настройка TaskAssignment
            modelBuilder.Entity<TaskAssignment>()
                .HasKey(ta => ta.AssignmentId);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(ta => ta.Task)
                .WithMany()
                .HasForeignKey(ta => ta.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(ta => ta.AssignedUser)
                .WithMany()
                .HasForeignKey(ta => ta.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(ta => ta.AssignedGroup)
                .WithMany()
                .HasForeignKey(ta => ta.AssignedGroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Настройка UserTaskSummary
            modelBuilder.Entity<UserTaskSummary>()
                .HasKey(uts => new { uts.UserId, uts.TaskDate });

            modelBuilder.Entity<UserTaskSummary>()
                .HasOne(uts => uts.User)
                .WithMany()
                .HasForeignKey(uts => uts.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Настройка StorageItem
            modelBuilder.Entity<StorageItem>()
                .HasKey(i => i.ItemId);

            modelBuilder.Entity<StorageItem>()
                .Property(i => i.ItemName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<StorageItem>()
                .Property(i => i.Quantity)
                .HasDefaultValue(0);

           // Настройка StorageMovement
            modelBuilder.Entity<StorageMovement>()
                .HasKey(m => m.MovementId);

            modelBuilder.Entity<StorageMovement>()
                .Property(m => m.MovementType)
                .IsRequired();

            modelBuilder.Entity<StorageMovement>()
                .Property(m => m.MovementDate)
                .IsRequired();

            modelBuilder.Entity<StorageMovement>()
                .HasOne(m => m.Item)
                .WithMany()
                .HasForeignKey(m => m.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StorageMovement>()
                .HasOne(m => m.Task)
                .WithMany()
                .HasForeignKey(m => m.RelatedTaskId)
                .OnDelete(DeleteBehavior.SetNull);

            // Настройка Device
            modelBuilder.Entity<Device>()
                .HasKey(d => d.DeviceId);

            modelBuilder.Entity<Device>()
                .Property(d => d.DeviceName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Device>()
                .Property(d => d.DeviceModel)
                .HasMaxLength(255);

            // Настройка DeviceComponent
            modelBuilder.Entity<DeviceComponent>()
                .HasKey(c => c.ComponentId);

            modelBuilder.Entity<DeviceComponent>()
                .Property(c => c.ComponentName)
                .IsRequired();

            modelBuilder.Entity<DeviceComponent>()
                .HasOne(c => c.Device)
                .WithMany(d => d.Components)
                .HasForeignKey(c => c.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка ComponentItem
            modelBuilder.Entity<ComponentItem>()
                .HasKey(ci => ci.ComponentItemId);

            modelBuilder.Entity<ComponentItem>()
                .HasOne(ci => ci.Component)
                .WithMany(c => c.ComponentItems)
                .HasForeignKey(ci => ci.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComponentItem>()
                .HasOne(ci => ci.Item)
                .WithMany(i => i.ComponentItems)
                .HasForeignKey(ci => ci.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
