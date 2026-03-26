using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using ProjectManagement.Infrastructure.Data;

namespace ProjectManagement.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId) =>
        await _db.Tasks.Where(t => t.ProjectId == projectId).OrderBy(t => t.Order).ToListAsync();

    public async Task<TaskItem?> GetByIdAsync(Guid id) =>
        await _db.Tasks.FindAsync(id);

    public async Task<bool> ExistsOrderInProjectAsync(Guid projectId, int order, Guid? excludeId = null)
    {
        var query = _db.Tasks.Where(t => t.ProjectId == projectId && t.Order == order);
        if (excludeId.HasValue) query = query.Where(t => t.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task AddAsync(TaskItem task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }
}
