using SympliSeoTracker.API.Middleware;
using SympliSeoTracker.Domain.Interfaces;
using SympliSeoTracker.Infrastructure.Caching;
using SympliSeoTracker.Infrastructure.Logging;
using SympliSeoTracker.Infrastructure.Services;
using MediatR;
using SympliSeoTracker.Application.Features.Search.Queries;
using FluentValidation.AspNetCore;
using SympliSeoTracker.Application.Validators;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Application layer with MediatR
//builder.Services.AddApplication();

// Register services
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISearchServiceFactory, SearchServiceFactory>();
builder.Services.AddScoped<GoogleSearchService>();
builder.Services.AddScoped<BingSearchService>(); // Add Bing search service

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SearchRequestHandler).Assembly));

// Add memory cache
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // React default port
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Then in the app configuration section:

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<SearchRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
//app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();


app.Run();
