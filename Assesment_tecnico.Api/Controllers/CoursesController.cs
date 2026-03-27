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
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] string? q, [FromQuery] CourseStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Courses.Where(c => !c.IsDeleted).AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(c => c.Title.Contains(q));
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var totalItems = await query.CountAsync();
            var courses = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PagedResult<Course>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = courses
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourseWithLesson([FromBody] CreateCourseWithLessonModel model)
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = model.CourseTitle,
                Status = CourseStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var lesson = new Lesson
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                Title = model.FirstLessonTitle,
                Order = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Courses.Add(course);
            _context.Lessons.Add(lesson);
            
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseModel model)
        {
            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null || existingCourse.IsDeleted) return NotFound();

            existingCourse.Title = model.Title;
            existingCourse.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null || course.IsDeleted) return NotFound();

            course.IsDeleted = true;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> PublishCourse(Guid id)
        {
            var course = await _context.Courses.Include(c => c.Lessons)
                                               .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (course == null) return NotFound();

            if (!course.Lessons.Any(l => !l.IsDeleted))
            {
                return BadRequest(new { Message = "Course must have at least one active lesson to be published." });
            }

            course.Status = CourseStatus.Published;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Course published" });
        }

        [HttpPatch("{id}/unpublish")]
        public async Task<IActionResult> UnpublishCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null || course.IsDeleted) return NotFound();

            course.Status = CourseStatus.Draft;
            course.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Course unpublished" });
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetCourseSummary(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (course == null) return NotFound();

            return Ok(new
            {
                course.Id,
                course.Title,
                course.Status,
                TotalLessons = course.Lessons.Count(l => !l.IsDeleted),
                LastModified = course.UpdatedAt
            });
        }
    }
}
