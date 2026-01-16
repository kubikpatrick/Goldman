using Microsoft.EntityFrameworkCore;

using Goldman.Api.Data;
using Goldman.Models.Groups;
using Goldman.Models.Identity;

namespace Goldman.Api.Services.Managers;

public sealed class GroupManager
{
    private readonly ApplicationDbContext _context;
    
    public GroupManager(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Group>> GetAsync(string userId)
    {
        var groups = await _context.Groups 
            .Include(g => g.Users)
            .ThenInclude(u => u.Devices.Where(d => d.IsPrimary))
            .Where(g => g.Users.Any(u => u.Id == userId))
            .ToListAsync();

        return groups;
    }

    public async Task CreateAsync(Group group)
    {
        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Group group)
    {
        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
    }

    public async Task AddMemberAsync(Group group, User user)
    {
        
    }

    public async Task RemoveMemberAsync(Group group, User user)
    {

    }
}