using ProjectManagement.Application.Common;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepo;
    private readonly ITaskRepository _taskRepo;

    public ProjectService(IProjectRepository projectRepo, ITaskRepository taskRepo)
    {
        _projectRepo = projectRepo;
        _taskRepo = taskRepo;
    }

    public async Task<PagedResult<ProjectDto>> SearchAsync(ProjectStatus? status, int page, int pageSize)
    {
        var (items, total) = await _projectRepo.SearchAsync(status, page, pageSize);
        return new PagedResult<ProjectDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(Guid id)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project == null) return Result.Failure<ProjectDto>("Project not found.");
        return Result.Success(MapToDto(project));
    }

    public async Task<Result<ProjectSummaryDto>> GetSummaryAsync(Guid id)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(id);
        if (project == null) return Result.Failure<ProjectSummaryDto>("Project not found.");
        return Result.Success(new ProjectSummaryDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            TotalTasks = project.Tasks.Count,
            CompletedTasks = project.Tasks.Count(t => t.IsCompleted)
        });
    }

    public async Task<IEnumerable<TaskItemDto>> GetTasksByProjectAsync(Guid projectId)
    {
        var tasks = await _taskRepo.GetByProjectIdAsync(projectId);
        return tasks.OrderBy(t => t.Order).Select(MapTaskToDto);
    }

    public async Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            Status = ProjectStatus.Draft
        };
        await _projectRepo.AddAsync(project);
        return Result.Success(MapToDto(project));
    }

    public async Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project == null) return Result.Failure<ProjectDto>("Project not found.");
        project.Name = dto.Name;
        project.Description = dto.Description;
        await _projectRepo.UpdateAsync(project);
        return Result.Success(MapToDto(project));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project == null) return Result.Failure("Project not found.");
        await _projectRepo.DeleteAsync(project);
        return Result.Success();
    }

    public async Task<Result> ActivateAsync(Guid id)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(id);
        if (project == null) return Result.Failure("Project not found.");
        if (!project.Tasks.Any())
            return Result.Failure("A project can only be activated if it has at least one task.");
        project.Status = ProjectStatus.Active;
        await _projectRepo.UpdateAsync(project);
        return Result.Success();
    }

    public async Task<Result> CompleteAsync(Guid id)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(id);
        if (project == null) return Result.Failure("Project not found.");
        if (project.Tasks.Any(t => !t.IsCompleted))
            return Result.Failure("A project can only be completed if all its tasks are completed.");
        project.Status = ProjectStatus.Completed;
        await _projectRepo.UpdateAsync(project);
        return Result.Success();
    }

    private static ProjectDto MapToDto(Project p) => new()
    {
        Id = p.Id, Name = p.Name, Description = p.Description, Status = p.Status
    };

    private static TaskItemDto MapTaskToDto(TaskItem t) => new()
    {
        Id = t.Id, ProjectId = t.ProjectId, Title = t.Title,
        Priority = t.Priority, Order = t.Order, IsCompleted = t.IsCompleted
    };
}
