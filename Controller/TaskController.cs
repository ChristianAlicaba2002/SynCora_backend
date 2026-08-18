using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.ITask;
using syncora_server.Utils;

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
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Tasks retrieved successfully.", tasks);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTasks()
    {
        var tasks = await _taskService.GetMyTasksAsync();
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Tasks retrieved successfully.", tasks);
        return Ok(response);
    }

    [EnableRateLimiting("write")]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(dto);
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status201Created, "Task created successfully.", task);
            return Ok(response);
        }
        catch (Exception e)
        {
            var response = ApiResponse<object>.FailedResponse(StatusCodes.Status400BadRequest, e.Message);
            return BadRequest(response);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Task retrieved successfully.", task);
            return Ok(response);
            
        }
        catch (TaskExceptions.TaskNotFound e)
        {
            var response = ApiResponse<object>.FailedResponse(e.StatusCode, e.Message);
            return NotFound(response);
        }
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
