using Scalar.AspNetCore;
using PlaylistApi.EF;
using Microsoft.EntityFrameworkCore;
using PlaylistApi.Core.Entities;


var builder = WebApplication.CreateBuilder(args);

// Honor the port the hosting platform assigns (Render, Cloud Run, etc. set PORT).
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();


builder.Services.AddProblemDetails();

// CORS for browser frontends on other origins (local Vite dev pointed at this API,
// or the deployed site). Origins come from config: "Cors:AllowedOrigins" (comma-separated),
// overridable via the Cors__AllowedOrigins environment variable on the host.
var corsOrigins = (builder.Configuration["Cors:AllowedOrigins"] ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddPersistence(builder.Configuration.GetConnectionString("Default")!);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Outside development, unhandled exceptions are returned as ProblemDetails (500).
    app.UseExceptionHandler();
}

// API docs (Scalar UI at /scalar/v1) are exposed in all environments so the
// hosted instance has a browsable, testable UI.
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.UseStatusCodePages();

app.UseCors();

// HTTPS is terminated by the hosting platform's proxy, so only redirect locally.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Migrate() is relational-only; non-relational providers (e.g. InMemory in tests) use EnsureCreated()
    if (db.Database.IsRelational())
        db.Database.Migrate();
    else
        db.Database.EnsureCreated();

}

app.Run();
