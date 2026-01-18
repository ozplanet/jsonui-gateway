using JsonUi.Gateway.Logging;
using JsonUi.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JsonUi.Gateway.Controllers;

[ApiController]
[Route("admin/logging")]
[Authorize(Policy = "Admin")]
public sealed class AdminLoggingController : ControllerBase
{
    private readonly ILoggingService _loggingService;

    public AdminLoggingController(ILoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    [HttpGet]
    public ActionResult<LoggingOptions> Get() => _loggingService.GetOptions();

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] LoggingOptions options, CancellationToken cancellationToken)
    {
        await _loggingService.UpdateAsync(options, cancellationToken);
        return NoContent();
    }
}
