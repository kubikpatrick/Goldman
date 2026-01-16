using System.ComponentModel.DataAnnotations;


using Microsoft.AspNetCore.Identity;

using Goldman.Models.Devices;
using Goldman.Models.Groups;

namespace Goldman.Models.Identity;

public sealed class User : IdentityUser
{
    public User()
    {
        
    }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public List<Device> Devices { get; set; }
    
    public List<RefreshToken> RefreshTokens { get; set; }
    
    public List<Group> Groups { get; set; } = [];
}