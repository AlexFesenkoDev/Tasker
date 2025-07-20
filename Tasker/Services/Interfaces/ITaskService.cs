using Tasker.Models;
using Tasker.Models.Dtos;

namespace Tasker.Services.Interfaces
{
    public interface ITaskService
    {
        Task<PagedResult<TaskItem>> GetTasksAsync(int userId, TaskPriority? priority, bool? isCompleted, int page, int pageSize);
        Task<TaskItem?> GetByIdAsync(int userId, int taskId);
        Task<TaskItem> CreateAsync(int userId, TaskItemDto dto);
        Task<bool> UpdateAsync(int userId, int taskId, TaskItemDto dto);
        Task<bool> DeleteAsync(int userId, int taskId);
    }
}
