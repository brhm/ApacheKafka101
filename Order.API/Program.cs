using Order.API.Extensions;
using Order.API.Services;
using Shared.Events.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// First create a TOPIC or QUEUE => move it BusExtension class
//builder.Services.AddSingleton<IBus, Bus>(sp =>
//{
//    var logger = sp.GetRequiredService<ILogger<Bus>>();
//    var bus = new Bus(builder.Configuration, logger);
//    bus.CreateTopicOrQueue([BusConstants.OrderCreatedEventTopicName]);

//    return bus;
//});
builder.Services.AddSingleton<IBus, Bus>();
builder.Services.AddScoped<OrderService>();// Defined as scope due to DB transactions.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// create topic
await app.CreateTopicOrQueuesAsync();

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
