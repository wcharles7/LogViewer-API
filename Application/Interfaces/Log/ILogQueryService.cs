using Application.DTOs.Log;

namespace Application.Interfaces.Log;

public interface ILogQueryService
{
    Task<LogPagedResponse> GetLogsAsync(LogQueryRequest request);
    Task<LogEntryDto?>     GetLogByIdAsync(int id);
    Task<LogStatsResponse> GetStatsAsync();
    Task<List<string>>     GetSourcesAsync();
}
