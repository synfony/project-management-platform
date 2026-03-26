using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Domain.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<bool> ExistsOrderInProjectAsync(Guid projectId, int order, Guid? excludeId = null);
    Task AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
}
