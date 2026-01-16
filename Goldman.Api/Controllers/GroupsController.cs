using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Goldman.Api.Data;
using Goldman.Http.Requests;
using Goldman.Models.Groups;
using Goldman.Models.Identity;

namespace Goldman.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class GroupsController : AuthorizeControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    
    public GroupsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<Group>>> Index()
    {
        var groups = await _context.Groups
            .Include(g => g.Users.Where(u => u.Id != CurrentUserId))
            .ThenInclude(u => u.Devices.Where(d => d.IsPrimary))
            .Where(g => g.Users.Any(u => u.Id == CurrentUserId))
            .ToListAsync();
        
        return groups;
    }

    [HttpPost]
    public async Task<ActionResult<Group>> Create([FromBody] CreateGroupRequest request)
    {
        var group = new Group
        {
            Name = request.Name,
            UserId = CurrentUserId
        };
        
        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();
        
        return group;
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] string id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group is null || group.UserId != CurrentUserId)
        {
            return NotFound();
        }
        
        _context.Groups.Remove(group);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpPost("{id:guid}/users")]
    public async Task<ActionResult> AddUser([FromRoute] string id, [FromBody] AddUserRequest request)
    {
        var group = await _context.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == id);
        if (group is null || group.UserId != CurrentUserId)
        {
            return NotFound();
        }
        
        bool exists = group.Users.Exists(u => u.Id == request.UserId);
        if (exists)
        {
            return Conflict("User already exists in the group.");
        }
        
        var user = await _userManager.FindByIdAsync(request.UserId);
        group.Users.Add(user);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpDelete("{groupId:alpha}/users/{userId:alpha}")]
    public async Task<ActionResult> RemoveUser([FromRoute] string groupId, [FromRoute] string userId)
    {
        var group = await _context.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == groupId);
        if (group is null || group.UserId != CurrentUserId)
        {
            return NotFound();
        }
        
        bool exists = group.Users.Exists(u => u.Id == userId);
        if (!exists)
        {
            return NotFound();
        }
        
        var userToRemove = group.Users.FirstOrDefault(u => u.Id == userId);
        if (userToRemove is not null)
        {
            group.Users.Remove(userToRemove);
        }

        if (group.Users.Count <= 1)
        {
            _context.Groups.Remove(group);
        }
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
}