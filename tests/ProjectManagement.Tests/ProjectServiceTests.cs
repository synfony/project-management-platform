using Moq;
using FluentAssertions;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepo = new();
    private readonly Mock<ITaskRepository> _taskRepo = new();

    private ProjectService CreateService() => new(_projectRepo.Object, _taskRepo.Object);

    [Fact]
    public async Task ActivateProject_WithTasks_ShouldSucceed()
    {
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId,
            Status = ProjectStatus.Draft,
            Tasks = new List<TaskItem> { new() { Title = "Task 1" } }
        };
        _projectRepo.Setup(r => r.GetByIdWithTasksAsync(projectId)).ReturnsAsync(project);

        var service = CreateService();
        var result = await service.ActivateAsync(projectId);

        result.IsSuccess.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.Active);
    }

    [Fact]
    public async Task ActivateProject_WithoutTasks_ShouldFail()
    {
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, Status = ProjectStatus.Draft, Tasks = new List<TaskItem>() };
        _projectRepo.Setup(r => r.GetByIdWithTasksAsync(projectId)).ReturnsAsync(project);

        var service = CreateService();
        var result = await service.ActivateAsync(projectId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("at least one task");
    }

    [Fact]
    public async Task CompleteProject_WithPendingTasks_ShouldFail()
    {
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId,
            Status = ProjectStatus.Active,
            Tasks = new List<TaskItem>
            {
                new() { Title = "Task 1", IsCompleted = true },
                new() { Title = "Task 2", IsCompleted = false }
            }
        };
        _projectRepo.Setup(r => r.GetByIdWithTasksAsync(projectId)).ReturnsAsync(project);

        var service = CreateService();
        var result = await service.CompleteAsync(projectId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("all its tasks are completed");
    }

    [Fact]
    public async Task DeleteProject_ShouldBeDelete()
    {
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId };
        _projectRepo.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _projectRepo.Setup(r => r.DeleteAsync(project)).Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.DeleteAsync(projectId);

        result.IsSuccess.Should().BeTrue();
        _projectRepo.Verify(r => r.DeleteAsync(project), Times.Once);
    }
}
