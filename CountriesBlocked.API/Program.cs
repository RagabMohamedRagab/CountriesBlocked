using CountriesBlocked.Application.Dtos;
using CountriesBlocked.Application.ThirdPartyClients;
using CountriesBlocked.Infrastructure.BackgroundServices;
using CountriesBlocked.Infrastructure.IManger;
using CountriesBlocked.Infrastructure.Manger;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1",new OpenApiInfo {
        Title="Countries Blocked API",
        Version="v1",
        Description="An example API using Swagger in .NET 9",
    });

});


builder.Services.AddMemoryCache();

builder.Services.Configure<IpApiConfig>(
    builder.Configuration.GetSection("IpApiConfig"));

builder.Services.AddHttpClient("IpApiConfig",client => {
    client.BaseAddress=new Uri(builder.Configuration["IpApiConfig:Url"]!);
});
builder.Services.AddHostedService<CountryBlockCleanupJob>();

builder.Services.AddScoped<IBlockedCountriesStore,BlockedCountriesStore>();
builder.Services.AddScoped<IBlockedAttemptsStore,BlockedAttemptsStore>();
builder.Services.AddScoped<ILocationService,LocationService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json","My API V1");
        c.RoutePrefix=string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
