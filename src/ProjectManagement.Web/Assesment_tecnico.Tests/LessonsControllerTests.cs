using Assesment_tecnico.Api.Controllers;
using Assesment_tecnico.Api.Data;
using Assesment_tecnico.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Assesment_tecnico.Tests
{
    public class LessonsControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public LessonsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private ApplicationDbContext CreateContext() => new ApplicationDbContext(_dbContextOptions);

        [Fact]
        public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
        {
            // Arrange
            var context = CreateContext();
            var courseId = Guid.NewGuid();
            context.Courses.Add(new Course { Id = courseId, Title = "Test Course" });
            await context.SaveChangesAsync();

            var controller = new LessonsController(context);
            var model = new CreateLessonModel { CourseId = courseId, Title = "New Lesson" };

            // Act
            var result = await controller.CreateLesson(model);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var lesson = Assert.IsType<Lesson>(createdAtActionResult.Value);
            Assert.Equal(1, lesson.Order); // First lesson should have order 1
        }

        [Fact]
        public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
        {
            // Arrange
            var context = CreateContext();
            var courseId = Guid.NewGuid();
            var lessonId = Guid.NewGuid();
            context.Courses.Add(new Course { Id = courseId, Title = "Test Course" });
            context.Lessons.Add(new Lesson { Id = lessonId, CourseId = courseId, Title = "Existing Lesson", Order = 1 });
            await context.SaveChangesAsync();

            var controller = new LessonsController(context);
            var model = new UpdateLessonModel { Title = "Updated Lesson", Order = 1 }; // Attempt to use order 1 again

            // Act
            var result = await controller.UpdateLesson(lessonId, model);

            // Assert
            // The logic in UpdateLesson prevents duplicate order, but CreateLesson auto-increments.
            // So we test the UpdateLesson logic here.
            var anotherLessonId = Guid.NewGuid();
            context.Lessons.Add(new Lesson { Id = anotherLessonId, CourseId = courseId, Title = "Another Lesson", Order = 2 });
            await context.SaveChangesAsync();

            var updateController = new LessonsController(context);
            var updateModel = new UpdateLessonModel { Title = "Trying to duplicate order", Order = 1 };
            var updateResult = await updateController.UpdateLesson(anotherLessonId, updateModel);

            Assert.IsType<BadRequestObjectResult>(updateResult);
        }

        [Fact]
        public async Task DeleteLesson_ShouldSoftDeleteAndReorderRemainingLessons()
        {
            // Arrange
            var context = CreateContext();
            var courseId = Guid.NewGuid();
            context.Courses.Add(new Course { Id = courseId, Title = "Test Course" });
            var lesson1 = new Lesson { Id = Guid.NewGuid(), CourseId = courseId, Title = "Lesson 1", Order = 1 };
            var lesson2 = new Lesson { Id = Guid.NewGuid(), CourseId = courseId, Title = "Lesson 2", Order = 2 };
            var lesson3 = new Lesson { Id = Guid.NewGuid(), CourseId = courseId, Title = "Lesson 3", Order = 3 };
            context.Lessons.AddRange(lesson1, lesson2, lesson3);
            await context.SaveChangesAsync();

            var controller = new LessonsController(context);

            // Act
            var result = await controller.DeleteLesson(lesson2.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var deletedLesson = await context.Lessons.IgnoreQueryFilters().SingleOrDefaultAsync(l => l.Id == lesson2.Id);
            Assert.True(deletedLesson.IsDeleted);

            var remainingLessons = await context.Lessons.Where(l => l.CourseId == courseId && !l.IsDeleted).OrderBy(l => l.Order).ToListAsync();
            Assert.Equal(2, remainingLessons.Count);
            Assert.Equal(1, remainingLessons[0].Order);
            Assert.Equal("Lesson 1", remainingLessons[0].Title);
            Assert.Equal(2, remainingLessons[1].Order);
            Assert.Equal("Lesson 3", remainingLessons[1].Title);
        }

        [Fact]
        public async Task ReorderLesson_MoveDown_ShouldUpdateOrderCorrectly()
        {
            // Arrange
            var context = CreateContext();
            var courseId = Guid.NewGuid();
            context.Courses.Add(new Course { Id = courseId, Title = "Test Course" });
            var lesson1 = new Lesson { Id = Guid.NewGuid(), CourseId = courseId, Title = "Lesson 1", Order = 1 };
            var lesson2 = new Lesson { Id = Guid.NewGuid(), CourseId = courseId, Title = "Lesson 2", Order = 2 };
            context.Lessons.AddRange(lesson1, lesson2);
            await context.SaveChangesAsync();

            var controller = new LessonsController(context);

            // Act
            var result = await controller.ReorderLesson(lesson1.Id, "down");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var lessons = await context.Lessons.Where(l => l.CourseId == courseId).OrderBy(l => l.Order).ToListAsync();
            Assert.Equal("Lesson 2", lessons[0].Title);
            Assert.Equal(1, lessons[0].Order);
            Assert.Equal("Lesson 1", lessons[1].Title);
            Assert.Equal(2, lessons[1].Order);
        }

        [Fact]
        public async Task ReorderLesson_MoveUp_ShouldUpdateOrderCorrectly()
        {
            // Arrange
            var context = CreateContext();
            var courseId = Guid.NewGuid();
            context.Courses.Add(new Course { Id = courseId, Title = "Test Course" });
            var lesson1 = new Lesson { Id = Guid.NewGuid(), CourseId = courseId, Title = "Lesson 1", Order = 1 };
            var lesson2 = new Lesson { Id = Guid.NewGuid(), CourseId = courseId, Title = "Lesson 2", Order = 2 };
            context.Lessons.AddRange(lesson1, lesson2);
            await context.SaveChangesAsync();

            var controller = new LessonsController(context);

            // Act
            var result = await controller.ReorderLesson(lesson2.Id, "up");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var lessons = await context.Lessons.Where(l => l.CourseId == courseId).OrderBy(l => l.Order).ToListAsync();
            Assert.Equal("Lesson 2", lessons[0].Title);
            Assert.Equal(1, lessons[0].Order);
            Assert.Equal("Lesson 1", lessons[1].Title);
            Assert.Equal(2, lessons[1].Order);
        }
    }
}
