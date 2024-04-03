using Mango.Services.EmailAPI.Extensions;
using Mango.Services.EmailAPI.Services;
using EmailAPI.Data;
using EmailAPI.Integration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

DbContextOptionsBuilder<AppDbContext> dbContextOptionsBuilder = new();
dbContextOptionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailService(dbContextOptionsBuilder.Options));

builder.Services.AddSingleton<IEmailCartProcessor, EmailCartProcessor>();
builder.Services.AddSingleton<INewUserRegisteredProcessor, NewUserRegisteredProcessor>();
builder.Services.AddControllers();

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