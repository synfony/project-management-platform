using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.DTOs;

public class TaskItemDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public int Order { get; set; }
    public bool IsCompleted { get; set; }
}

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public int Order { get; set; }
}

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; }
}

public class ReorderTaskDto
{
    public string Direction { get; set; } = string.Empty; // "up" or "down"
}
