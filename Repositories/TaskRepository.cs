using Microsoft.EntityFrameworkCore;
using syncora_server.Data;
using syncora_server.DTOs;
using syncora_server.Interface.ITask;
using syncora_server.Models;

namespace syncora_server.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Tasks> CreateAsync(CreateTaskDTO _createTaskDTO, Guid userId)
    {
        var task = new Tasks
        {
            Id = Guid.NewGuid(),
            Title = _createTaskDTO.Title,
            Description = _createTaskDTO.Description,
            Status = _createTaskDTO.Status,
            Priority = _createTaskDTO.Priority,
            DueDate = _createTaskDTO.DueDate,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<List<Tasks>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.Tasks
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Tasks?> GetByIdAsync(Guid taskId, Guid userId)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
    }

    public async Task<Tasks?> UpdateAsync(Guid taskId, Guid userId, UpdateTaskDTO _updateTaskDTO)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task is null) return null;

        task.Title = _updateTaskDTO.Title ?? task.Title;
        task.Description = _updateTaskDTO.Description ?? task.Description;
        task.Status = _updateTaskDTO.Status ?? task.Status;
        task.Priority = _updateTaskDTO.Priority ?? task.Priority;
        task.DueDate = _updateTaskDTO.DueDate ?? task.DueDate;

        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(Guid taskId, Guid userId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task is null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Tasks>> GetAllAsync()
    {
        return await _context.Tasks
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
