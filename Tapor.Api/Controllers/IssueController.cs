using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Tapor.DB;
using Tapor.Services;
using Tapor.Shared.Dtos;

namespace Tapor.Api.Controllers;

/// <summary>
/// Хотелки
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class IssueController: ControllerBase
{
    private readonly ILogger _logger;

    public IssueController(ILogger logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Создание хотелки
    /// </summary>
    /// <remarks>
    /// {
    /// "id": 0,
    /// "assignee": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    /// "reporter": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    /// "summary": "супер задача",
    /// "description": "большое описание",
    /// "type": 1,
    /// "status": 2,
    /// "priority": 3,
    /// "size": 4,
    /// "estimatedTime": 0,
    /// "dueDate": "2023-01-29T15:47:00.645Z"
    /// }
    /// </remarks>
    /// <param name="dto">модель issue</param>
    /// <returns>идентификатор созданного пожелания</returns>
    [HttpPost]
    public IActionResult Create([FromBody]IssueDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var currentUserId = Guid.NewGuid();
        var repo = new IssueRepository(_logger);
        var service = new IssueService(repo);
        var issueId = service.Create(dto, currentUserId);
        
        // возвращаем на клиент
        return Ok(issueId);
    }
    
    /// <summary>
    /// Детализация пожелания
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <response code="200">возвращает модель пожелания</response>
    /// <response code="404">ишшью не найдено</response>
    [HttpGet]
    [Produces("application/json")]
    public IActionResult Details([FromQuery]long id)
    {
        if (id != 5) return NotFound();
        
        var res = new IssueDto
        {
            Id = 5,
            Reporter = Guid.Empty,
            Status = 1,
            Summary = "Почистить кофе машину",
            Description = "Очень нужная и серьезная задача, необходимо почистить кофе машину",
            DueDate = DateTime.Now,
            EstimatedTime = 1.5m
        };
        return Ok(res);
    }

    [HttpPost]
    [Consumes("image/jpeg")]
    [Produces("application/json")]
    public IActionResult UploadPhoto(IFormFile img)
    {
        return Ok();
    }
}