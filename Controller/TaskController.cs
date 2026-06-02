using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using syncora_server.DTOs;
using syncora_server.Interface.ITask;

namespace syncora_server.Controller;

[Authorize(Policy = "UserOnly")]
[ApiController]
[Route("api/v1/tasks")]
public class TaskController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpPost("/create")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
    {
        var task = await _taskService.CreateTaskAsync(dto);
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    [HttpGet("/")]
    public async Task<IActionResult> GetMyTasks()
    {
        var tasks = await _taskService.GetMyTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task is null) return NotFound();
        return Ok(task);
    }

    [HttpPut("{id:guid}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDTO dto)
    {
        var task = await _taskService.UpdateTaskAsync(id, dto);
        if (task is null) return NotFound();
        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var deleted = await _taskService.DeleteTaskAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
