using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace LogViewer.Infrastructure.Data;

public class LogViewerDbContext : DbContext
{
    public LogViewerDbContext(DbContextOptions<LogViewerDbContext> options) : base(options) { }

    public DbSet<LogEntryEntity> LogEntries => Set<LogEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEntryEntity>(e =>
        {
            e.ToTable("LogEntries");
            e.HasKey(x => x.LogEntryId);
        });
    }
}