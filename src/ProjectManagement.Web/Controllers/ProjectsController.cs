using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService) => _projectService = projectService;

    public async Task<IActionResult> Index(ProjectStatus? status, int page = 1)
    {
        var result = await _projectService.SearchAsync(status, page, 10);
        ViewBag.CurrentStatus = status;
        ViewBag.CurrentPage = page;
        return View(result);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var summary = await _projectService.GetSummaryAsync(id);
        if (!summary.IsSuccess) return NotFound();
        var tasks = await _projectService.GetTasksByProjectAsync(id);
        ViewBag.Tasks = tasks;
        return View(summary.Value);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        var result = await _projectService.CreateAsync(dto);
        if (!result.IsSuccess) { ModelState.AddModelError("", result.Error); return View(dto); }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _projectService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();
        var dto = new UpdateProjectDto { Name = result.Value!.Name, Description = result.Value.Description };
        ViewBag.ProjectId = id;
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateProjectDto dto)
    {
        var result = await _projectService.UpdateAsync(id, dto);
        if (!result.IsSuccess) { ModelState.AddModelError("", result.Error); ViewBag.ProjectId = id; return View(dto); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _projectService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _projectService.ActivateAsync(id);
        if (!result.IsSuccess) TempData["Error"] = result.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _projectService.CompleteAsync(id);
        if (!result.IsSuccess) TempData["Error"] = result.Error;
        return RedirectToAction(nameof(Details), new { id });
    }
}
