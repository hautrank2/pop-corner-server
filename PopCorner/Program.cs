using Microsoft.EntityFrameworkCore;
using PopCorner.Data;

// 1. Register services
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<PopCornerDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PopCornerConnectionString")));

// 2. Build app
var app = builder.Build();

// 3. Add middlware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
