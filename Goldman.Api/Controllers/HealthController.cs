using Microsoft.AspNetCore.Mvc;

namespace Goldman.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class HealthController : ControllerBase
{
    public HealthController()
    {
        
    }
    
    [HttpGet]
    public ActionResult Index() => Ok();
}