using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;

namespace ProjectManagement.Web.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    public IActionResult Create(Guid projectId)
    {
        ViewBag.ProjectId = projectId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskDto dto)
    {
        var result = await _taskService.CreateAsync(projectId, dto);
        if (!result.IsSuccess) { TempData["Error"] = result.Error; }
        return RedirectToAction("Details", "Projects", new { id = projectId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id, Guid projectId)
    {
        await _taskService.DeleteAsync(id);
        return RedirectToAction("Details", "Projects", new { id = projectId });
    }

    [HttpPost]
    public async Task<IActionResult> Complete(Guid id, Guid projectId)
    {
        await _taskService.CompleteAsync(id);
        return RedirectToAction("Details", "Projects", new { id = projectId });
    }

    [HttpPost]
    public async Task<IActionResult> Reorder(Guid id, string direction, Guid projectId)
    {
        await _taskService.ReorderAsync(id, direction);
        return RedirectToAction("Details", "Projects", new { id = projectId });
    }
}
