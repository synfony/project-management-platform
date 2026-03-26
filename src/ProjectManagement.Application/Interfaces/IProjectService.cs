using ProjectManagement.Application.Common;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.Interfaces;

public interface IProjectService
{
    Task<PagedResult<ProjectDto>> SearchAsync(ProjectStatus? status, int page, int pageSize);
    Task<Result<ProjectDto>> GetByIdAsync(Guid id);
    Task<Result<ProjectSummaryDto>> GetSummaryAsync(Guid id);
    Task<IEnumerable<TaskItemDto>> GetTasksByProjectAsync(Guid projectId);
    Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto);
    Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> ActivateAsync(Guid id);
    Task<Result> CompleteAsync(Guid id);
}
