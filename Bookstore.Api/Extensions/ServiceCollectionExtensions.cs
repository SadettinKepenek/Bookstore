using Book.Application.Repositories;
using Book.Infrastructure.Persistent.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Order.Application.Repositories;
using Order.Infrastructure.Persistent.EntityFrameworkCore;

namespace Bookstore.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBookDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Book");
        
        services.AddDbContext<BookDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBookUnitOfWork, BookDbContext>(x =>
            x.GetRequiredService<BookDbContext>());

        return services;
    }
    public static IServiceCollection AddOrderDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Order");
        
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IOrderUnitOfWork, OrderDbContext>(x =>
            x.GetRequiredService<OrderDbContext>());

        return services;
    }
}