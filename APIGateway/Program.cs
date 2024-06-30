using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
  .AddJsonFile("ocelot.json")
  .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true)
  .AddEnvironmentVariables();

builder.Services.AddOcelot();
var app = builder.Build();
app.Use(async (context, next) =>
{
  Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
  await next();
  Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
});
await app.UseOcelot();

app.Run();