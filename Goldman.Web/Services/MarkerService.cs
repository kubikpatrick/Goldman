using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models;
using Community.Blazor.MapLibre.Models.Marker;

using Goldman.Models;

namespace Goldman.Web.Services;

public sealed class MarkerService
{
    private readonly MapLibre _map;
    private HashSet<string> _ids;
    
    public MarkerService(MapLibre map)
    {
        _map = map;
        _ids = [];
    }
    
    public async Task AddMarkerAsync(string id, Location location, MarkerOptions options)
    {
        await _map.AddMarker(options, new LngLat(location.Longitude, location.Latitude), new Guid(id));
        
        _ids.Add(id);
    }
    
    public async Task RemoveMarkerAsync(string id)
    {
        await _map.RemoveMarker(new Guid(id));
        
        _ids.Remove(id);
    }

    public async Task MoveMarkerAsync(string id, Location location)
    {
        await _map.MoveMarker(new Guid(id), new LngLat(location.Longitude, location.Latitude));
    }

    public bool Exists(string id)
    {
        return _ids.Contains(id);
    }
}