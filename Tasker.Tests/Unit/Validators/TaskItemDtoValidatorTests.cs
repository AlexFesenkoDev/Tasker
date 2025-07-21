using FluentValidation.TestHelper;
using Tasker.Models;
using Tasker.Models.Dtos;
using Tasker.Validators;

namespace Tasker.Tests.Unit.Validators
{
    public class TaskItemDtoValidatorTests
    {
        private readonly TaskItemDtoValidator _validator = new();

        [Fact]
        public void Should_Pass_When_ValidDto()
        {
            var dto = new TaskItemDto
            {
                Title = "Test Task",
                Description = "Some description",
                Priority = TaskPriority.Medium
            };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_TitleIsEmpty()
        {
            var dto = new TaskItemDto { Title = "" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Fail_When_TitleTooLong()
        {
            var dto = new TaskItemDto { Title = new string('a', 101) };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Fail_When_DescriptionTooLong()
        {
            var dto = new TaskItemDto
            {
                Title = "Valid",
                Description = new string('x', 1001)
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Fail_When_PriorityIsInvalidEnum()
        {
            var dto = new TaskItemDto
            {
                Title = "Valid",
                Priority = (TaskPriority)999
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Priority);
        }
    }
}
