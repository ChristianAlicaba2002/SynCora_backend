using syncora_server.Models;

namespace syncora_server.Interface.ITask;

public interface ITaskService
{
    Task<Tasks> CreateTask();
}
