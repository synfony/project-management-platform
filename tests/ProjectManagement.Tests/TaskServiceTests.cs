using Moq;
using FluentAssertions;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Services;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();

    private TaskService CreateService() => new(_taskRepo.Object);

    [Fact]
    public async Task CreateTask_WithDuplicateOrder_ShouldFail()
    {
        var projectId = Guid.NewGuid();
        _taskRepo.Setup(r => r.ExistsOrderInProjectAsync(projectId, 1, null)).ReturnsAsync(true);

        var service = CreateService();
        var result = await service.CreateAsync(projectId, new CreateTaskDto { Title = "Test", Order = 1 });

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Order 1 already exists");
    }
}
