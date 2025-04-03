using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Allow Angular frontend origin
            .AllowAnyMethod()                     // Allow GET, POST, etc.
            .AllowAnyHeader();                    // Allow any headers (e.g., Authorization)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAngularApp"); // Apply CORS policy before routing
app.UseAuthorization();
app.MapControllers();

app.Run();