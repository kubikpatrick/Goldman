using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goldman.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UsersController : AuthorizeControllerBase
{
    public UsersController()
    {
        
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}/avatar")]
    [ResponseCache(Duration = 3600)]
    public async Task<ActionResult> Avatar(string userId)
    {
        return PhysicalFile("C:\\Users\\patri\\Desktop\\dog.jpg", "image/jpeg");
    }
}