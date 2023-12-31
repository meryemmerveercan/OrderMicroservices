using MassTransit;
using Notification.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var rabbitmqString = builder.Configuration.GetConnectionString("RabbitMQ") ?? throw new InvalidOperationException("RabbitMQ not found.");

builder.Services.AddMassTransit(config =>
{
	config.AddConsumer<OrderCreatedEventConsumer>();
	config.UsingRabbitMq((ctx, cfg) =>
	{
		cfg.Host(rabbitmqString, hostConfig =>
		{
			hostConfig.Username("iuppvhyz");
			hostConfig.Password("iBReTgl07FWofHlh2l_Dc83sXnLypy3K");
		});
		cfg.ReceiveEndpoint("order-created-queue", e =>
		{
			e.ConfigureConsumer<OrderCreatedEventConsumer>(ctx);
		});
	});
});
builder.Services.AddMassTransitHostedService();

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

app.Run();
