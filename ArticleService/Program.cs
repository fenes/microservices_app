using Microsoft.EntityFrameworkCore;
using ArticleService.Data;
using ArticleService.DataAccessLayer;
using ArticleService.Services;
using ArticleService.Models.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ArticleContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IConnectionMultiplexer>(
  ConnectionMultiplexer.Connect(builder.Configuration["Redis:Configuration"]));
builder.Services.AddScoped<ICacheService<ArticleDto>, RedisCacheService<ArticleDto>>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the IArticleService service to the container
builder.Services.AddScoped<IArticleService, ArticleService.DataAccessLayer.ArticleService>();

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<ArticleContext>();
  dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();