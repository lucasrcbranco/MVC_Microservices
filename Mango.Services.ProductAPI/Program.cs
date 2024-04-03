using Microsoft.EntityFrameworkCore;
using ProductAPI;
using ProductAPI.Data;
using ProductAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configuring AutoMapper with DependencyInjection
var mapper = AutoMapperConfiguration.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddSwaggerConfigurations();
builder.AddAppAuthenticationAndAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
ExecutePendingMigrations();
app.Run();

void ExecutePendingMigrations()
{
    if (app is null) return;

    using var scope = app.Services.CreateScope();
    var _dbContext = scope.ServiceProvider.GetService<AppDbContext>();

    if (_dbContext is null) return;

    if (_dbContext.Database.GetPendingMigrations().Any())
    {
        _dbContext.Database.Migrate();
    }
}