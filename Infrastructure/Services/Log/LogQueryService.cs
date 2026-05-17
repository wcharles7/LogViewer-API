using Application.DTOs.Log;
using Application.Interfaces.Log;
using Domain.Entities;
using LogViewer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Log;

public class LogQueryService : ILogQueryService
{
    private readonly LogViewerDbContext _context; // ← ton DbContext spécifique

    public LogQueryService(LogViewerDbContext context) // ← pas DbContext générique
    {
        _context = context;
    }

    public async Task<LogPagedResponse> GetLogsAsync(LogQueryRequest request)
    {
        var query = _context.LogEntries.AsQueryable(); // ✅ LogEntries existe maintenant

        if (!string.IsNullOrEmpty(request.LevelName))
            query = query.Where(l => l.LevelName == request.LevelName);

        if (!string.IsNullOrEmpty(request.Source))
            query = query.Where(l => l.Source == request.Source);

        if (!string.IsNullOrEmpty(request.CorrelationId))
            query = query.Where(l => l.CorrelationId == request.CorrelationId);

        if (!string.IsNullOrEmpty(request.UserId))
            query = query.Where(l => l.UserId == request.UserId);

        if (!string.IsNullOrEmpty(request.HttpMethod))
            query = query.Where(l => l.HttpMethod == request.HttpMethod);

        if (request.StatusCode.HasValue)
            query = query.Where(l => l.StatusCode == request.StatusCode);

        if (request.DateFrom.HasValue)
            query = query.Where(l => l.Timestamp >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(l => l.Timestamp <= request.DateTo.Value);

        if (!string.IsNullOrEmpty(request.Search))
        {
            string s = request.Search.ToLower();
            query = query.Where(l =>
                l.Message.ToLower().Contains(s) ||
                (l.Exception != null && l.Exception.ToLower().Contains(s)) ||
                (l.Source    != null && l.Source.ToLower().Contains(s)));
        }

        int total = await query.CountAsync();
        int pages = (int)Math.Ceiling(total / (double)request.PageSize);

        var data = await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => MapToDto(l))
            .ToListAsync();

        return new LogPagedResponse
        {
            Total    = total,
            Page     = request.Page,
            PageSize = request.PageSize,
            Pages    = pages,
            Data     = data
        };
    }

    public async Task<LogEntryDto?> GetLogByIdAsync(int id)
    {
        var log = await _context.LogEntries.FindAsync(id);
        return log is null ? null : MapToDto(log);
    }

    public async Task<LogStatsResponse> GetStatsAsync()
    {
        var total = await _context.LogEntries.CountAsync();
        var errors = await _context.LogEntries.CountAsync(l => l.LevelName == "Error");
        var warnings = await _context.LogEntries.CountAsync(l => l.LevelName == "Warning");
        var info = await _context.LogEntries.CountAsync(l => l.LevelName == "Information");

        var sources = await _context.LogEntries
            .Where(l => l.Source != null)
            .Select(l => l.Source!)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        var since = DateTimeOffset.UtcNow.AddHours(-24);
        var last24h = await _context.LogEntries
            .Where(l => l.Timestamp >= since)
            .GroupBy(l => new { Hour = l.Timestamp.Hour, l.LevelName })
            .Select(g => new { g.Key.Hour, g.Key.LevelName, Count = g.Count() })
            .ToListAsync();

        var hourlyStats = last24h
            .GroupBy(x => x.Hour)
            .Select(g => new LogLevelCount
            {
                Hour     = $"{g.Key:00}:00",
                Errors   = g.Where(x => x.LevelName == "Error").Sum(x => x.Count),
                Warnings = g.Where(x => x.LevelName == "Warning").Sum(x => x.Count)
            })
            .OrderBy(x => x.Hour)
            .ToList();

        return new LogStatsResponse
        {
            TotalCount   = total,
            ErrorCount   = errors,
            WarningCount = warnings,
            InfoCount    = info,
            Sources      = sources,
            Last24h      = hourlyStats
        };
    }

    public async Task<List<string>> GetSourcesAsync()
    {
        return await _context.LogEntries
            .Where(l => l.Source != null)
            .Select(l => l.Source!)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();
    }

    private static LogEntryDto MapToDto(LogEntryEntity l) => new()
    {
        LogEntryId      = l.LogEntryId,
        Timestamp       = l.Timestamp,
        LevelName       = l.LevelName,
        Source          = l.Source,
        Message         = l.Message,
        Exception       = l.Exception,
        Metadata        = l.Metadata,
        EventId         = l.EventId,
        CorrelationId   = l.CorrelationId,
        UserId          = l.UserId,
        RequestPath     = l.RequestPath,
        HttpMethod      = l.HttpMethod,
        StatusCode      = l.StatusCode,
        DurationMs      = l.DurationMs,
        IpAddress       = l.IpAddress,
        MachineName     = l.MachineName,
        Environment     = l.Environment,
        ApplicationName = l.ApplicationName
    };
}