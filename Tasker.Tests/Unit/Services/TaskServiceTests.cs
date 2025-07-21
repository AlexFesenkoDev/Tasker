using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Tasker.Models;
using Tasker.Models.Dtos;
using Tasker.Services;
using Tasker.Tests.TestUtils;

namespace Tasker.Tests.Unit.Services
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task GetTasksAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            // Arrange
            using var context = DbContextHelper.CreateInMemoryContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var service = new TaskService(context, memoryCache);

            int userId = 1;
            context.TaskItems.AddRange(
                new TaskItem { Title = "T1", UserId = userId, Priority = TaskPriority.Medium },
                new TaskItem { Title = "T2", UserId = userId, Priority = TaskPriority.High, IsCompleted = true },
                new TaskItem { Title = "T3", UserId = 2, Priority = TaskPriority.Low }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetTasksAsync(userId, TaskPriority.High, true, 1, 10);

            // Assert
            result.TotalCount.Should().Be(1);
            result.Items.Should().ContainSingle(t => t.Title == "T2");
        }

        [Fact]
        public async Task GetTasksAsync_ShouldUseCache_OnSecondCall()
        {
            // Arrange
            using var context = DbContextHelper.CreateInMemoryContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new TaskService(context, memoryCache);

            int userId = 1;
            context.TaskItems.Add(new TaskItem
            {
                Title = "CachedTask",
                UserId = userId,
                Priority = TaskPriority.High,
                IsCompleted = false
            });
            await context.SaveChangesAsync();

            // Act
            var result1 = await service.GetTasksAsync(userId, TaskPriority.High, false, 1, 10);

            // Remove from DB
            context.TaskItems.RemoveRange(context.TaskItems);
            await context.SaveChangesAsync();

            // Act 
            var result2 = await service.GetTasksAsync(userId, TaskPriority.High, false, 1, 10);

            // Assert
            result1.Items.Should().HaveCount(1);
            result2.Items.Should().HaveCount(1);
            result2.Items.First().Title.Should().Be("CachedTask");
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnUserTask()
        {
            using var context = DbContextHelper.CreateInMemoryContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var service = new TaskService(context, memoryCache);

            var task = new TaskItem { Title = "Test", UserId = 1 };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var result = await service.GetByIdAsync(1, task.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Test");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddTask()
        {
            using var context = DbContextHelper.CreateInMemoryContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var service = new TaskService(context, memoryCache);

            var dto = new TaskItemDto { Title = "New Task", Description = "Desc", Priority = TaskPriority.Low };

            var result = await service.CreateAsync(1, dto);

            result.Id.Should().BeGreaterThan(0);
            result.Title.Should().Be("New Task");
            (await context.TaskItems.CountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask()
        {
            using var context = DbContextHelper.CreateInMemoryContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var service = new TaskService(context, memoryCache);

            var task = new TaskItem { Title = "Old", UserId = 1, IsCompleted = false };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var dto = new TaskItemDto { Title = "Updated", IsCompleted = true };
            var result = await service.UpdateAsync(1, task.Id, dto);

            result.Should().BeTrue();
            (await context.TaskItems.FindAsync(task.Id))!.Title.Should().Be("Updated");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTask()
        {
            using var context = DbContextHelper.CreateInMemoryContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var service = new TaskService(context, memoryCache);

            var task = new TaskItem { Title = "To delete", UserId = 1 };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(1, task.Id);

            result.Should().BeTrue();
            (await context.TaskItems.FindAsync(task.Id)).Should().BeNull();
        }
    }
}
