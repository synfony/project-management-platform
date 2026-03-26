using ProjectManagement.Application.Common;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepo;

    public TaskService(ITaskRepository taskRepo)
    {
        _taskRepo = taskRepo;
    }

    public async Task<Result<TaskItemDto>> CreateAsync(Guid projectId, CreateTaskDto dto)
    {
        if (await _taskRepo.ExistsOrderInProjectAsync(projectId, dto.Order))
            return Result.Failure<TaskItemDto>($"Order {dto.Order} already exists in this project.");

        var task = new TaskItem
        {
            ProjectId = projectId,
            Title = dto.Title,
            Priority = dto.Priority,
            Order = dto.Order,
            IsCompleted = false
        };
        await _taskRepo.AddAsync(task);
        return Result.Success(MapToDto(task));
    }

    public async Task<Result<TaskItemDto>> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await _taskRepo.GetByIdAsync(id);
        if (task == null) return Result.Failure<TaskItemDto>("Task not found.");
        task.Title = dto.Title;
        task.Priority = dto.Priority;
        await _taskRepo.UpdateAsync(task);
        return Result.Success(MapToDto(task));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var task = await _taskRepo.GetByIdAsync(id);
        if (task == null) return Result.Failure("Task not found.");
        await _taskRepo.DeleteAsync(task);
        return Result.Success();
    }

    public async Task<Result> CompleteAsync(Guid id)
    {
        var task = await _taskRepo.GetByIdAsync(id);
        if (task == null) return Result.Failure("Task not found.");
        task.IsCompleted = true;
        await _taskRepo.UpdateAsync(task);
        return Result.Success();
    }

    public async Task<Result> ReorderAsync(Guid id, string direction)
    {
        var task = await _taskRepo.GetByIdAsync(id);
        if (task == null) return Result.Failure("Task not found.");

        var allTasks = (await _taskRepo.GetByProjectIdAsync(task.ProjectId))
            .OrderBy(t => t.Order).ToList();

        var index = allTasks.FindIndex(t => t.Id == id);

        if (direction == "up" && index > 0)
        {
            var neighbor = allTasks[index - 1];
            (task.Order, neighbor.Order) = (neighbor.Order, task.Order);
            await _taskRepo.UpdateAsync(task);
            await _taskRepo.UpdateAsync(neighbor);
        }
        else if (direction == "down" && index < allTasks.Count - 1)
        {
            var neighbor = allTasks[index + 1];
            (task.Order, neighbor.Order) = (neighbor.Order, task.Order);
            await _taskRepo.UpdateAsync(task);
            await _taskRepo.UpdateAsync(neighbor);
        }
        else
        {
            return Result.Failure("Cannot reorder in that direction.");
        }

        return Result.Success();
    }

    private static TaskItemDto MapToDto(TaskItem t) => new()
    {
        Id = t.Id, ProjectId = t.ProjectId, Title = t.Title,
        Priority = t.Priority, Order = t.Order, IsCompleted = t.IsCompleted
    };
}
