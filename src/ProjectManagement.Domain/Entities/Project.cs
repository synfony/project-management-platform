using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
