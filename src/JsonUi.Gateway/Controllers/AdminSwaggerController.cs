using JsonUi.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JsonUi.Gateway.Controllers;

[ApiController]
[Route("admin/integrations/{integrationId:guid}/swagger")]
[Authorize(Policy = "Admin")]
public sealed class AdminSwaggerController : ControllerBase
{
    private readonly ISwaggerImportService _swaggerImportService;

    public AdminSwaggerController(ISwaggerImportService swaggerImportService)
    {
        _swaggerImportService = swaggerImportService;
    }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Import(Guid integrationId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = "Swagger file is required" });
        }

        await using var stream = file.OpenReadStream();
        var actions = await _swaggerImportService.ImportAsync(integrationId, stream, cancellationToken);
        return Ok(actions);
    }
}
