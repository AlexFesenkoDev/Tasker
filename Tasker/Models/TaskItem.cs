using System.ComponentModel.DataAnnotations;

namespace Tasker.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsCompleted { get; set; } = false;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }
}
