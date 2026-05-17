using Application.DTOs.Log;
using Application.Interfaces.Log;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Log;

[ApiController]
[Route("logs")]
public class LogEntryController : ControllerBase
{
    private readonly ILogQueryService _logQueryService;

    public LogEntryController(ILogQueryService logQueryService)
    {
        _logQueryService = logQueryService;
    }

    /// <summary>
    /// Get paginated logs with filters
    /// GET /logs/get?levelName=Error&source=MonCashPaymentController&page=1&pageSize=50
    /// </summary>
    [HttpGet("get")]
    public async Task<IActionResult> GetLogs([FromQuery] LogQueryRequest request)
    {
        var result = await _logQueryService.GetLogsAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Get a single log entry by ID
    /// GET /logs/get/42
    /// </summary>
    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetLogById(int id)
    {
        var log = await _logQueryService.GetLogByIdAsync(id);
        if (log is null)
            return NotFound(new { message = $"Log entry {id} not found" });

        return Ok(log);
    }

    /// <summary>
    /// Get aggregated stats (counts, sources, last 24h chart)
    /// GET /logs/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _logQueryService.GetStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Get distinct sources for filter dropdown
    /// GET /logs/sources
    /// </summary>
    [HttpGet("sources")]
    public async Task<IActionResult> GetSources()
    {
        var sources = await _logQueryService.GetSourcesAsync();
        return Ok(sources);
    }
}
