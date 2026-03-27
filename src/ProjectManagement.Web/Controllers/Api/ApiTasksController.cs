using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;

namespace ProjectManagement.Web.Controllers.Api;

[ApiController]
[Route("api/tasks")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ApiTasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public ApiTasksController(ITaskService taskService) => _taskService = taskService;

    [HttpPost("{projectId}")]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateTaskDto dto)
    {
        var result = await _taskService.CreateAsync(projectId, dto);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return CreatedAtAction(null, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var result = await _taskService.UpdateAsync(id, dto);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _taskService.DeleteAsync(id);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return NoContent();
    }

    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _taskService.CompleteAsync(id);
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok();
    }

    [HttpPatch("{id}/reorder")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderTaskDto dto)
    {
        var result = await _taskService.ReorderAsync(id, dto.Direction);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }
}
