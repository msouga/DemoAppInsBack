using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using ms_productos_service.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(new AppDbCommandInterceptor(builder.Services.BuildServiceProvider().GetRequiredService<TelemetryClient>())));

// Registrar controladores
builder.Services.AddControllers();
// Add Application Insights services
builder.Services.AddApplicationInsightsTelemetry();
// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services and define a policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Add Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

// Middleware de redirección HTTPS (opcional en dev)
// Use CORS middleware with the defined policy
app.UseCors("AllowAngularOrigins");

// Mapear rutas a los controladores (incluye tu ProductosController)
app.MapControllers();

app.Run();