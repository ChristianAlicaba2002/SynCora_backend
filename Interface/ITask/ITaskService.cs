using syncora_server.DTOs;

namespace syncora_server.Interface.ITask;

public interface ITaskService
{
    Task<List<TaskResponseDTO>> GetAllTasksAsync();
    Task<TaskResponseDTO> CreateTaskAsync(CreateTaskDTO dto);
    Task<List<TaskResponseDTO>> GetMyTasksAsync();
    Task<TaskResponseDTO?> GetTaskByIdAsync(Guid taskId);
    Task<TaskResponseDTO?> UpdateTaskAsync(Guid taskId, UpdateTaskDTO dto);
    Task<bool> DeleteTaskAsync(Guid taskId);
}
