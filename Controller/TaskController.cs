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
public class TaskController(ITaskService taskService, ILogger<TaskController> logger) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;
    private readonly ILogger<TaskController> _logger = logger;
    [HttpGet("all")]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        _logger.LogInformation("Tasks retrieved successfully.");
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Tasks retrieved successfully.", tasks);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTasks()
    {
        var tasks = await _taskService.GetMyTasksAsync();
        _logger.LogInformation("Tasks retrieved successfully.");
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
            _logger.LogInformation("Task created successfully.");
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status201Created, "Task created successfully.", task);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError("Task creation failed: {ErrorMessage}", e.Message);
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
            _logger.LogInformation("Task retrieved successfully.");
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Task retrieved successfully.", task);
            return Ok(response);
            
        }
        catch (TaskExceptions.TaskNotFound e)
        {
            _logger.LogError("Task not found: {ErrorMessage}", e.Message);
            var response = ApiResponse<object>.FailedResponse(e.StatusCode, e.Message);
            return NotFound(response);
        }
    }

    [HttpPatch("{id:guid}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDTO dto)
    {
        try
        {
            var task = await _taskService.UpdateTaskAsync(id, dto);
            _logger.LogInformation("Task updated successfully.");
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Task updated successfully.", task);
            return Ok(response);
            
        }
        catch (TaskExceptions.TaskNotFound e)
        {
            _logger.LogError("Task not found: {ErrorMessage}", e.Message);
            var response = ApiResponse<object>.FailedResponse(e.StatusCode, e.Message);
            return NotFound(response);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        try
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            _logger.LogInformation("Task deleted successfully.");
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Task deleted successfully.", deleted);
            return Ok(response);
        }
        catch (TaskExceptions.TaskNotFound e)
        {
            _logger.LogError("Task not found: {ErrorMessage}", e.Message);
            var response = ApiResponse<object>.FailedResponse(e.StatusCode, e.Message);
            return NotFound(response);
        }
    }
}
