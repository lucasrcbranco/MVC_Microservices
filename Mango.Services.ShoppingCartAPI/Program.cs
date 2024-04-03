using Mango.Services.ShoppingCartAPI.Services;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Mango.Services.ShoppingCartAPI.Utility;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Extensions;
using ShoppingCartAPI.Integration;
using ShoppingCartAPI.Utility;

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

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IMessageBusService, MessageBusService>();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

builder.Services
    .AddHttpClient(nameof(SD.ProductAPIHttpClientURL), url => url.BaseAddress = new Uri(builder.Configuration["ServicesUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

builder.Services
    .AddHttpClient(nameof(SD.CouponAPIHttpClientURL), url => url.BaseAddress = new Uri(builder.Configuration["ServicesUrls:CouponAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

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