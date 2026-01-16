using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using Goldman.Api.Data;

namespace Goldman.Api.Hubs;

[Authorize]
public sealed class LocationHub : Hub
{
    private readonly ApplicationDbContext _context;
    
    private readonly ILogger<LocationHub> _logger;
    
    public LocationHub(ApplicationDbContext context, ILogger<LocationHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var groups = await _context.Groups.Where(g => g.Users.Any(u => u.Id == Context.UserIdentifier)).AsNoTracking().ToListAsync();
        if (groups.Count > 0)
        {
            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id);
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
        {
            _logger.LogError("{message}", exception.Message);
        }
        
        var groups = await _context.Groups.Where(g => g.Users.Any(u => u.Id == Context.UserIdentifier)).ToListAsync();
        if (groups.Count > 0)
        {
            foreach (var group in groups)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Id);
            }
        }
    }
}