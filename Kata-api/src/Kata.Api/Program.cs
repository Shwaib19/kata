using Kata.Api.Application;
using Kata.Application;
using Kata.infrastructure;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SharedKernel;
using SharedKernel.Messaging;

const string clientCorsPolicy = "KataClient";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options => options.AddPolicy(clientCorsPolicy, policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddScoped<ISender, KataSender>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);

WebApplication app = builder.Build();

// The schema is brought up to date at startup so that `docker compose up` yields a working
// application with no extra step. A deployed system would run migrations as a separate job,
// before the new version starts serving: here a single instance owns the database.
await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<KataDbContext>().Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseCors(clientCorsPolicy);

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
   .WithTags("Health")
   .WithSummary("Liveness probe");
app.MapEndpoints();

app.Run();
