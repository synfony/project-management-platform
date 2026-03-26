using ProjectManagement.Application.Common;
using ProjectManagement.Application.DTOs;

namespace ProjectManagement.Application.Interfaces;

public interface ITaskService
{
    Task<Result<TaskItemDto>> CreateAsync(Guid projectId, CreateTaskDto dto);
    Task<Result<TaskItemDto>> UpdateAsync(Guid id, UpdateTaskDto dto);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> CompleteAsync(Guid id);
    Task<Result> ReorderAsync(Guid id, string direction);
}
