using Goldman.Api.Hubs;
using Goldman.Api.Services.Managers;
using Goldman.Http.Requests;
using Goldman.Models;
using Goldman.Models.Devices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Goldman.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class DevicesController : AuthorizeControllerBase
{
    private readonly DeviceManager _deviceManager;
    
    private readonly IHubContext<LocationHub> _hub;
    
    public DevicesController(DeviceManager deviceManager, IHubContext<LocationHub> hub)
    {
        _deviceManager = deviceManager;
        _hub = hub;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Device>>> Index()
    {
        var devices = await _deviceManager.FindForUserAsync(CurrentUserId);

        return devices;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Device>> Get([FromRoute] string id)
    {
        var device = await _deviceManager.FindByIdAsync(id, CurrentUserId);
        if (device is null)
        {
            return NotFound();
        }
        
        return device;
    }
    
    [HttpPost]
    public async Task<ActionResult<Device>> Create([FromBody] CreateDeviceRequest request)
    {
        var device = new Device
        {
            Name = request.Name,
            Type = request.Type,
            UserId = CurrentUserId
        };
        
        var result = await _deviceManager.CreateAsync(device);
        if (!result.Succeeded)
        {
            return Problem(result.Message);
        }

        return device;
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] string id)
    {
        var device = await _deviceManager.FindByIdAsync(id, CurrentUserId);
        if (device is null)
        {
            return NotFound();
        }
        
        var result = await _deviceManager.DeleteAsync(device);
        if (!result.Succeeded)
        {
            return Problem(result.Message);
        }
        
        return Ok();
    }
    
    [HttpPatch("{id:guid}/location")]
    public async Task<ActionResult> Location([FromRoute] string id, [FromBody] Location location)
    {
        var device = await _deviceManager.FindByIdAsync(id, CurrentUserId);
        if (device is null)
        {
            return NotFound();
        }
        
        var result = await _deviceManager.UpdateLocationAsync(device, location);
        if (!result.Succeeded)
        {
            return Problem(result.Message);
        }
        
        return Ok();
    }
}