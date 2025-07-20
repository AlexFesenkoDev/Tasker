using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tasker.Data;
using Tasker.Models;
using Tasker.Models.Dtos;
using Tasker.Services.Interfaces;

namespace Tasker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
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
            
            var result = await _taskService.GetTasksAsync(userId, priority, isCompleted, page, pageSize);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.GetByIdAsync(userId, id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskItemDto dto)
        {
            var userId = GetCurrentUserId();
            
            var task = await _taskService.CreateAsync(userId, dto);

            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskItemDto dto)
        {
            var userId = GetCurrentUserId();

            var success = await _taskService.UpdateAsync(userId, id, dto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _taskService.DeleteAsync(userId, id);

            if(!success)
                return NotFound();

            return NoContent();
        }
    }
}
