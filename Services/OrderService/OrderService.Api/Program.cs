using DotNetEnv;
using OrderService.Application.Services;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure;
using CoreLib.HttpLogic;
using OrderService.Api.Consumers;
using MassTransit;


Env.Load();
var builder = WebApplication.CreateBuilder(args);
var connectionString = $"Host={Env.GetString("DB_HOST")};Port={Env.GetString("DB_PORT")};Database={Env.GetString("DB_NAME")};Username={Env.GetString("DB_USER")};Password={Env.GetString("DB_PASSWORD")}";
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddScoped<IOrderService, OrderService.Application.Services.OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddControllers();
builder.Services.AddHttpRequestService();

builder.Services.AddMassTransit(x =>
{
    // Регистрируем consumers
    x.AddConsumer<ReserveProductCommandConsumer>();
    x.AddConsumer<CreateOrderCommandConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        //TODO env

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.MapControllers();
app.Run();

