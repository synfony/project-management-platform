using Assesment_tecnico.Api.Data;
using Assesment_tecnico.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assesment_tecnico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLessonsByCourse(Guid courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId && !l.IsDeleted)
                .OrderBy(l => l.Order)
                .ToListAsync();
            return Ok(lessons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLesson(Guid id)
        {
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
            if (lesson == null) return NotFound();
            return Ok(lesson);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonModel model)
        {
            var maxOrder = await _context.Lessons
                .Where(l => l.CourseId == model.CourseId && !l.IsDeleted)
                .MaxAsync(l => (int?)l.Order) ?? 0;

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                CourseId = model.CourseId,
                Title = model.Title,
                Order = maxOrder + 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, lesson);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(Guid id, [FromBody] UpdateLessonModel model)
        {
            var existingLesson = await _context.Lessons.FindAsync(id);
            if (existingLesson == null || existingLesson.IsDeleted) return NotFound();

            var orderExists = await _context.Lessons.AnyAsync(l => l.CourseId == existingLesson.CourseId && l.Order == model.Order && l.Id != id && !l.IsDeleted);
            if (orderExists)
            {
                return BadRequest(new { Message = $"Order {model.Order} is already in use for this course." });
            }

            existingLesson.Title = model.Title;
            existingLesson.Order = model.Order;
            existingLesson.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(Guid id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            lesson.IsDeleted = true;
            lesson.UpdatedAt = DateTime.UtcNow;
            
            var remainingLessons = await _context.Lessons
                .Where(l => l.CourseId == lesson.CourseId && !l.IsDeleted && l.Id != id) // Exclude the deleted lesson
                .OrderBy(l => l.Order)
                .ToListAsync();
            
            for (int i = 0; i < remainingLessons.Count; i++)
            {
                remainingLessons[i].Order = i + 1;
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/reorder")]
        public async Task<IActionResult> ReorderLesson(Guid id, [FromQuery] string direction)
        {
            var lessonToMove = await _context.Lessons.FindAsync(id);
            if (lessonToMove == null || lessonToMove.IsDeleted) return NotFound();

            var lessons = await _context.Lessons
                .Where(l => l.CourseId == lessonToMove.CourseId && !l.IsDeleted)
                .OrderBy(l => l.Order)
                .ToListAsync();

            var oldIndex = lessons.FindIndex(l => l.Id == id);
            if (oldIndex == -1) return NotFound();

            int newIndex = oldIndex;
            if (direction == "up" && oldIndex > 0)
            {
                newIndex = oldIndex - 1;
            }
            else if (direction == "down" && oldIndex < lessons.Count - 1)
            {
                newIndex = oldIndex + 1;
            }
            else
            {
                return BadRequest("Cannot move in that direction");
            }

            lessons.RemoveAt(oldIndex);
            lessons.Insert(newIndex, lessonToMove);

            for (int i = 0; i < lessons.Count; i++)
            {
                lessons[i].Order = i + 1;
            }

            await _context.SaveChangesAsync();
            return Ok(lessons);
        }
    }
}
