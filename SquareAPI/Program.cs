using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using SquareAPI.ExceptionHandling;
using SquareCore.Interfaces.Repository;
using SquareCore.Interfaces.Services;
using SquareCore.Services;
using SquareInfrastructure.Data;
using SquareInfrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMetrics();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation() 
            .AddConsoleExporter();
    });

builder.Services.AddDbContext<SquareDataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddScoped<IPointRepository, PointRepository>();

builder.Services.AddScoped<IPointService, PointService>();
builder.Services.AddScoped<ISquareService, SquareService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var environment = app.Environment;

if (!environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<SquareDataContext>();
        db.Database.Migrate();
    }
}
app.Run();