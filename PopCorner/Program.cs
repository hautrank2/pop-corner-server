using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PopCorner.Data;
using PopCorner.Mappings;
using PopCorner.Models.Common;
using PopCorner.Repositories;
using PopCorner.Repositories.Interfaces;
using PopCorner.Service;
using PopCorner.Service.Interfaces;

var CORS_NAME = "AllowFrontend";

// 1. Register services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_NAME,
       policy =>
       {
           policy
               .WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
       });
});
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddHttpContextAccessor();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Replace this line:
// builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// With this line:
builder.Services.AddDbContext<PopCornerDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PopCornerConnectionString")));

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();

builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfiles));


builder.Services.AddScoped<ICloudnaryService, CloudinaryService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton(sp =>
{
    var s = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(s.CloudName, s.ApiKey, s.ApiSecret);
    var cld = new Cloudinary(account) { Api = { Secure = s.Secure } };
    return cld;
});


// 2. Build app
var app = builder.Build();

// 3. Add middlware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(CORS_NAME);

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
