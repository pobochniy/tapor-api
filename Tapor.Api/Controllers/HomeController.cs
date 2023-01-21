using Microsoft.AspNetCore.Mvc;

namespace Tapor.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController: ControllerBase
{
    [HttpGet("[action]/{id?}")]
    public IActionResult Index(int id)
    {
        return Content($"Hello, controllers {id}");
    }
    
    [HttpGet("[action]")]
    [Route("koko")]
    public IActionResult About(int id)
    {
        return Content($"About");
    }
}