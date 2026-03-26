using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Interfaces;

public interface IProjectRepository
{
    Task<(IEnumerable<Project> Items, int TotalCount)> SearchAsync(ProjectStatus? status, int page, int pageSize);
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByIdWithTasksAsync(Guid id);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}
