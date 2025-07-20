using Microsoft.EntityFrameworkCore;
using Tasker.Data;
using Tasker.Models;
using Tasker.Models.Dtos;
using Tasker.Services.Interfaces;

namespace Tasker.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _dbContext;

        public TaskService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TaskItem> CreateAsync(int userId, TaskItemDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted,
                UserId = userId,
                Priority = dto.Priority
            };

            _dbContext.TaskItems.Add(task);
            await _dbContext.SaveChangesAsync();

            return task;
        }

        public async Task<bool> DeleteAsync(int userId, int taskId)
        {
            var task = await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return false;

            _dbContext.TaskItems.Remove(task);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }

        public async Task<TaskItem?> GetByIdAsync(int userId, int taskId)
        {
            return await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        }

        public async Task<PagedResult<TaskItem>> GetTasksAsync(int userId, TaskPriority? priority, bool? isCompleted, int page, int pageSize)
        {
            var query = _dbContext.TaskItems
                .Where(t => t.UserId == userId)
                .AsQueryable();

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted.Value);

            var totalCount = await query.CountAsync();

            var tasks = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedResult<TaskItem>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Items = tasks
            };

            return result;
        }

        public async Task<bool> UpdateAsync(int userId, int taskId, TaskItemDto dto)
        {
            var task = await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return false;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
