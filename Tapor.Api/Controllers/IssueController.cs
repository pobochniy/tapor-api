using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tapor.DB;
using Tapor.Services;
using Tapor.Shared.Dtos;

namespace Tapor.Api.Controllers;

/// <summary>
/// Хотелки
/// </summary>
[Route("api/[controller]/[action]")]
public class IssueController: ControllerBase
{
    private readonly IssueService _service;

    public IssueController(IssueService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Создание хотелки
    /// </summary>
    /// <param name="dto">модель issue</param>
    /// <returns>идентификатор созданного пожелания</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody]IssueDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var issueId = await _service.Create(dto, currentUserId, ct);
        
        // возвращаем на клиент
        return Ok(issueId);
    }
    
    /// <summary>
    /// Список пожеланий
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    public IActionResult GetList(CancellationToken ct)
    {
        var res = _service.GetList(ct);
        return Ok(res);
    }
    
    /// <summary>
    /// Детализация пожелания
    /// </summary>
    /// <param name="id">идентификатор</param>
    /// <response code="200">возвращает модель пожелания</response>
    /// <response code="404">ишшью не найдено</response>
    [HttpGet]
    [Produces("application/json")]
    public async Task<IActionResult> Details([FromQuery]long id)
    {
        var res = await _service.Details(id);
        if (res == null) return NotFound();
        
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