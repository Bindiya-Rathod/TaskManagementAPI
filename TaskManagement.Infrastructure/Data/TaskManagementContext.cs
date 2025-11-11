using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Models;
using TaskStatus = TaskManagement.Core.Models.TaskStatus;

namespace TaskManagement.Infrastructure.Data
{
    public class TaskManagementContext : DbContext
    {
        public TaskManagementContext(DbContextOptions<TaskManagementContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");
                entity.HasKey(e => e.UserId);
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("TASK");
                entity.HasKey(e => e.TaskId);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedTo)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedToUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskStatus>(entity =>
            {
                entity.ToTable("TASK_STATUS");
                entity.HasKey(e => e.StatusId);
            });

            modelBuilder.Entity<TaskPriority>(entity =>
            {
                entity.ToTable("TASK_PRIORITY");
                entity.HasKey(e => e.PriorityId);
            });

            modelBuilder.Entity<TaskAttachment>(entity =>
            {
                entity.ToTable("TASK_ATTACHMENT");
                entity.HasKey(e => e.AttachmentId);
            });

            modelBuilder.Entity<TaskHistory>(entity =>
            {
                entity.ToTable("TASK_HISTORY");
                entity.HasKey(e => e.HistoryId);
            });
        }
    }
}
