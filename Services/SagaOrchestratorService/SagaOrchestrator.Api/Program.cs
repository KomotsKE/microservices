using MassTransit;
using SagaOrchestratorService.Logic.Sagas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    // Регистрация Saga State Machine
    x.AddSagaStateMachine<CreateOrderSaga, CreateOrderSagaState>()
        .InMemoryRepository();

    // Транспорт
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Автоконфигурация эндпоинтов для саги
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.Run();

