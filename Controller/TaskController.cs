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

    [HttpGet("all")]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(new { message = "Tasks retrieved successfully.", status = StatusCodes.Status200OK, data = tasks });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTasks()
    {
        var tasks = await _taskService.GetMyTasksAsync();
        return Ok(new { message = "Tasks retrieved successfully.", status = StatusCodes.Status200OK, data = tasks });
    }

    [EnableRateLimiting("write")]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new { message = "Task created successfully.", status = StatusCodes.Status201Created, data = task });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, status = StatusCodes.Status400BadRequest });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task is null)
            return NotFound(new { message = "Task not found.", status = StatusCodes.Status404NotFound });

        return Ok(new { message = "Task retrieved successfully.", status = StatusCodes.Status200OK, data = task });
    }

    [HttpPatch("{id:guid}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDTO dto)
    {
        var task = await _taskService.UpdateTaskAsync(id, dto);
        if (task is null)
            return NotFound(new { message = "Task not found.", status = StatusCodes.Status404NotFound });

        return Ok(new { message = "Task updated successfully.", status = StatusCodes.Status200OK, data = task });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var deleted = await _taskService.DeleteTaskAsync(id);
        if (!deleted)
            return NotFound(new { message = "Task not found.", status = StatusCodes.Status404NotFound });

        return Ok(new { message = "Task deleted successfully.", status = StatusCodes.Status200OK });
    }
}
