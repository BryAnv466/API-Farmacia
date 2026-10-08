using System.Text.Json.Serialization;
using FarmaciaApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(GetConnectionString(builder.Configuration)));

// CORS: permite que la página web consuma la API
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Crea las tablas y los datos iniciales si no existen
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();
app.Run();

// Render entrega DATABASE_URL como postgres://user:pass@host:port/db
static string GetConnectionString(IConfiguration cfg)
{
    var url = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (string.IsNullOrEmpty(url)) return cfg.GetConnectionString("Default")!;
    var uri = new Uri(url);
    var user = uri.UserInfo.Split(':', 2);
    return $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};Database={uri.AbsolutePath.TrimStart('/')};" +
           $"Username={Uri.UnescapeDataString(user[0])};Password={Uri.UnescapeDataString(user[1])};" +
           "SSL Mode=Prefer;Trust Server Certificate=true";
}
