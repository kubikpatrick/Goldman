using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Goldman.Models.Identity;

namespace Goldman.Models.Groups;

[PrimaryKey(nameof(Id))]
public sealed class Group
{
    public Group()
    {
        
    }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public string Id { get; set; }
    
    [Required]
    public string Name { get; set; }

    [Required]
    public string UserId { get; set; }
    
    public List<User> Users { get; set; } = [];
}