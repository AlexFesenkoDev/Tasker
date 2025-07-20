namespace Tasker.Models.Dtos
{
    public class TaskItemDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public TaskPriority Priority { get; set; }
    }
}
