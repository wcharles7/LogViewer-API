namespace Application.DTOs.Log;

public class LogEntryDto
{
    public int             LogEntryId    { get; set; }
    public DateTimeOffset  Timestamp     { get; set; }
    public string          LevelName     { get; set; } = "";
    public string?         Source        { get; set; }
    public string          Message       { get; set; } = "";
    public string?         Exception     { get; set; }
    public string?         Metadata      { get; set; }
    public int?            EventId       { get; set; }
    public string?         CorrelationId { get; set; }
    public string?         UserId        { get; set; }
    public string?         RequestPath   { get; set; }
    public string?         HttpMethod    { get; set; }
    public int?            StatusCode    { get; set; }
    public long?           DurationMs    { get; set; }
    public string?         IpAddress     { get; set; }
    public string?         MachineName   { get; set; }
    public string?         Environment   { get; set; }
    public string?         ApplicationName { get; set; }
}

public class LogQueryRequest
{
    public string?  LevelName     { get; set; }
    public string?  Source        { get; set; }
    public string?  CorrelationId { get; set; }
    public string?  UserId        { get; set; }
    public string?  Search        { get; set; }
    public int?     StatusCode    { get; set; }
    public string?  HttpMethod    { get; set; }
    public DateTime? DateFrom     { get; set; }
    public DateTime? DateTo       { get; set; }
    public int      Page          { get; set; } = 1;
    public int      PageSize      { get; set; } = 50;
}

public class LogPagedResponse
{
    public int                  Total    { get; set; }
    public int                  Page     { get; set; }
    public int                  PageSize { get; set; }
    public int                  Pages    { get; set; }
    public List<LogEntryDto>    Data     { get; set; } = new();
}

public class LogStatsResponse
{
    public int ErrorCount       { get; set; }
    public int WarningCount     { get; set; }
    public int InfoCount        { get; set; }
    public int TotalCount       { get; set; }
    public List<string> Sources { get; set; } = new();
    public List<LogLevelCount> Last24h { get; set; } = new();
}

public class LogLevelCount
{
    public string Hour      { get; set; } = "";
    public int    Errors    { get; set; }
    public int    Warnings  { get; set; }
}
