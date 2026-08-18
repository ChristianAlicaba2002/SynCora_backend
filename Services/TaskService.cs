using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.ICurrentUser;
using syncora_server.Interface.ITask;
using syncora_server.Models;

namespace syncora_server.Services;

public class TaskService(ITaskRepository taskRepository, ICurrentUserService currentUser) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ICurrentUserService _currentUser = currentUser;

    private Guid GetAuthenticatedUserId()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        return _currentUser.UserId.Value;
    }

    private static string GetFullName(User? user)
    {
        if (user is null) return string.Empty;

        return string.Join(" ", new[] { user.FirstName, user.MiddleName, user.LastName }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
    }

    private static TaskResponseDTO MapToResponse(Tasks task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        Priority = task.Priority,
        CreatedAt = task.CreatedAt,
        DueDate = task.DueDate,
        UserId = task.UserId,
        CreatedByFullName = GetFullName(task.User),
        ImageUrl = task.User?.ImageUrl
    };

    public async Task<TaskResponseDTO> CreateTaskAsync(CreateTaskDTO _createTaskDTO)
    {
        var userId = GetAuthenticatedUserId();
        var created = await _taskRepository.CreateAsync(_createTaskDTO, userId);
        return MapToResponse(created);
    }

    public async Task<List<TaskResponseDTO>> GetMyTasksAsync()
    {
        var userId = GetAuthenticatedUserId();
        var tasks = await _taskRepository.GetAllByUserIdAsync(userId);
        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<TaskResponseDTO?> GetTaskByIdAsync(Guid taskId)
    {
        var userId = GetAuthenticatedUserId();
        var task = await _taskRepository.GetByIdAsync(taskId, userId) ?? throw new TaskExceptions.TaskNotFound("Task not found.", StatusCodes.Status404NotFound);
        return task is null ? null : MapToResponse(task);
    }

    public async Task<TaskResponseDTO?> UpdateTaskAsync(Guid taskId, UpdateTaskDTO _updateTaskDTO)
    {
        var userId = GetAuthenticatedUserId();
        var task = await _taskRepository.UpdateAsync(taskId, userId, _updateTaskDTO);
        return task is null ? null : MapToResponse(task);
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var userId = GetAuthenticatedUserId();
        return await _taskRepository.DeleteAsync(taskId, userId);
    }

    public async Task<List<TaskResponseDTO>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        if(tasks is null)
        {
            throw new Exception("No tasks found.");
        }
        return [.. tasks.Select(MapToResponse)];
    }
}
