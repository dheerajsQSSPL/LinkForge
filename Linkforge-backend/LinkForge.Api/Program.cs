
using LinkForge.Api.Middleware;
using LinkForge.Application.Interfaces;
using LinkForge.Application.Services;
using LinkForge.Infrastructure.Data;
using LinkForge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<LinkForgeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LinkForgeDb")));

// Dependency Injection
builder.Services.AddScoped<IShortLinkRepository, ShortLinkRepository>();
builder.Services.AddScoped<ILinkService, LinkService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - allow the React development server
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS before mapping endpoints
app.UseCors("ReactDev");

// Temporary database health check
app.MapGet("/health/db", async (LinkForgeDbContext dbContext) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync();

    return canConnect
        ? Results.Ok(new { status = "Database connected" })
        : Results.StatusCode(503);
});

app.MapControllers();

app.Run();
