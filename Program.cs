using Application.Interfaces.Log;
using DotNetEnv;
using Infrastructure.Services.Log;
using LogViewer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString =
    builder.Configuration.GetConnectionString("DB_CONNECTION_STRING");

builder.Services.AddDbContext<LogViewerDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

builder.Services.AddScoped<ILogQueryService, LogQueryService>();

// CORS pour React
builder.Services.AddCors(options =>
    options.AddPolicy("LogViewer", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("LogViewer");
app.MapControllers();
app.Run();