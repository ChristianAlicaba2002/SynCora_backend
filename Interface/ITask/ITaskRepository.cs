using syncora_server.DTOs;
using syncora_server.Models;

namespace syncora_server.Interface.ITask;

public interface ITaskRepository
{
    Task<List<Tasks>> GetAllAsync();
    Task<Tasks> CreateAsync(CreateTaskDTO _createTaskDTO, Guid userId);
    Task<List<Tasks>> GetAllByUserIdAsync(Guid userId);
    Task<Tasks?> GetByIdAsync(Guid taskId, Guid userId);
    Task<Tasks?> UpdateAsync(Guid taskId, Guid userId, UpdateTaskDTO _updateTaskDTO);
    Task<bool> DeleteAsync(Guid taskId, Guid userId);
}
