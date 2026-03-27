using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assesment_tecnico.Shared.Models
{
    // --- Main Entities ---

    public enum CourseStatus { Draft, Published }

    public class Course
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public CourseStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }

    public class Lesson
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public Course Course { get; set; } = null!;
    }

    // --- Auth DTOs ---

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // --- Course DTOs ---

    public class CreateCourseWithLessonModel
    {
        [Required]
        public string CourseTitle { get; set; } = string.Empty;

        [Required]
        public string FirstLessonTitle { get; set; } = string.Empty;
    }

    public class UpdateCourseModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
    }

    // --- Lesson DTOs ---

    public class CreateLessonModel
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
    }

    public class UpdateLessonModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int Order { get; set; }
    }
    
    // --- API Result DTOs ---
    
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
