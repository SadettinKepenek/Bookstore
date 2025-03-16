using System.Reflection;
using Asp.Versioning;
using Book.Application.Repositories.Book;
using Book.Application.Services.Book;
using Book.Infrastructure.Persistent.EntityFrameworkCore.Repositories;
using Bookstore.Api.Extensions;
using Bookstore.Api.Middlewares;
using Bookstore.Api.Validators.Book;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Order.Application.Repositories;
using Order.Application.Services;
using Order.Infrastructure.Persistent.EntityFrameworkCore.Repositories;
using Saga.Application;
using Saga.Application.Steps;
using Saga.Application.Steps.BookSteps;
using Saga.Application.Steps.OrderSteps;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(t => t.ToString()));
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddControllers();



builder.Services.AddBookDb(builder.Configuration);
builder.Services.AddOrderDb(builder.Configuration);

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<ProcessValidateAndGetBookStep>();
builder.Services.AddScoped<ProcessInitializeOrderStep>();
builder.Services.AddScoped<ProcessStockStep>();
builder.Services.AddScoped<ProcessCompleteOrderStep>();

builder.Services.AddScoped<IEnumerable<ISagaStep>>(sp => new List<ISagaStep>
{
    sp.GetRequiredService<ProcessValidateAndGetBookStep>(),
    sp.GetRequiredService<ProcessInitializeOrderStep>(),
    sp.GetRequiredService<ProcessStockStep>(),
    sp.GetRequiredService<ProcessCompleteOrderStep>()
});
builder.Services.AddScoped<SagaOrchestrator>();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

await app.RunAsync();
