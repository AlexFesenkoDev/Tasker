using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tasker.Data;
using Tasker.Models;
using Tasker.Models.Dtos;

namespace Tasker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TasksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
        
        [HttpGet]
        public async Task<ActionResult<PagedResult<TaskItem>>> Get(
            [FromQuery] TaskPriority? priority,
            [FromQuery] bool? isCompleted,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            
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

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskItemDto dto)
        {
            var userId = GetCurrentUserId();
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

            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskItemDto dto)
        {
            var userId = GetCurrentUserId();
            var task = await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.IsCompleted = dto.IsCompleted;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound();

            _dbContext.TaskItems.Remove(task);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
