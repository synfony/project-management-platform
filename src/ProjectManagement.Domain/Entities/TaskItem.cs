using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public int Order { get; set; }
    public bool IsCompleted { get; set; }
    public Project Project { get; set; } = null!;
}
