using Microsoft.EntityFrameworkCore;
using PartsInfoWebApi.Services;
using PartsInfoWebApi.Interfaces;
using AutoMapper;
using PartsInfoWebApi.Infrastructure;
using PartsInfoWebApi.Infrastructure.DIExtensions;
using Serilog;
using Serilog.Events;
using PartsInfoWebApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IThreeLetterCodeRepository, ThreeLetterCodeRepository>();
builder.Services.AddScoped<IThreeLetterCodeService, ThreeLetterCodeService>();

builder.Services.AddAutoMapper(typeof(Program)); // Assuming you have an AutoMapper profile setup

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            builder.WithOrigins("http://localhost:49864")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

app.Run();
