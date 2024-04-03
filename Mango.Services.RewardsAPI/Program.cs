using Mango.Services.RewardAPI.Extensions;
using Mango.Services.RewardAPI.Services;
using Microsoft.EntityFrameworkCore;
using RewardAPI.Data;
using RewardAPI.Integration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

DbContextOptionsBuilder<AppDbContext> dbContextOptionsBuilder = new();
dbContextOptionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new RewardService(dbContextOptionsBuilder.Options));
builder.Services.AddSingleton<IRewardProcessor, RewardProcessor>();
builder.Services.AddScoped<IRewardService, RewardService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
ExecutePendingMigrations();
app.ConfigureServiceBusConsumer();
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