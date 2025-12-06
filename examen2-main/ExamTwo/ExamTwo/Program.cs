using ExamTwo.Controllers;
using ExamTwo.Repositories;
using ExamTwo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddSingleton<ICoffeeRepository, CoffeeRepository>();
builder.Services.AddSingleton<IChangeRepository, ChangeRepository>();

// Register services
builder.Services.AddScoped<ICoffeeMachineService, CoffeeMachineService>();

builder.Services.AddSingleton<Database>();

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
