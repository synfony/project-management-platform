using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;
using ProjectManagement.Infrastructure.Data;

namespace ProjectManagement.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;

    public ProjectRepository(AppDbContext db) => _db = db;

    public async Task<(IEnumerable<Project> Items, int TotalCount)> SearchAsync(ProjectStatus? status, int page, int pageSize)
    {
        var query = _db.Projects.AsQueryable();
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Project?> GetByIdAsync(Guid id) =>
        await _db.Projects.FindAsync(id);

    public async Task<Project?> GetByIdWithTasksAsync(Guid id) =>
        await _db.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Project project)
    {
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _db.Projects.Update(project);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
    }
}
