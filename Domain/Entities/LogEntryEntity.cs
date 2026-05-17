namespace Domain.Entities;

public class LogEntryEntity
{
    public int             LogEntryId    { get; set; }
    public DateTimeOffset  Timestamp     { get; set; } = DateTimeOffset.UtcNow;
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
