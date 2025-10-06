using eCommerce.OrdersService.Application.RepositoryContracts;
using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Products;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Products.Config;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Users;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Users.Config;
using eCommerce.OrdersService.Infrastructure.MappingProfiles;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace eCommerce.OrdersService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(cfg => { }, typeof(OrderItemMappingProfile));

        services.AddMongo(config);
        services.AddUsersServiceClient(config);
        services.AddProductsServiceClient(config);

        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }

    private static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration config)
    {
        var mongoOptions = new MongoOptions()
        {
            User = config["MONGO_USER"]!,
            Pass = config["MONGO_PASS"]!,
            Host = config["MONGO_HOST"]!,
            Port = config["MONGO_PORT"]!,
            Database = config["MONGO_DATABASE"]!,
            Collections = new CollectionOptions()
            {
                Orders = config["MONGO_COLLECTION_ORDERS"]!
            }
        };

        services.Configure<MongoOptions>(opt =>
        {
            opt.User = mongoOptions.User;
            opt.Pass = mongoOptions.Pass;
            opt.Host = mongoOptions.Host;
            opt.Port = mongoOptions.Port;
            opt.Database = mongoOptions.Database;
            opt.Collections = mongoOptions.Collections;
        });

        var user = mongoOptions.User;
        var pass = mongoOptions.Pass;
        var host = mongoOptions.Host;
        var port = mongoOptions.Port;
        var connectionString = $"mongodb://{user}:{pass}@{host}:{port}";

        var mongoClient = new MongoClient(connectionString);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped<IMongoDatabase>(_ => mongoClient.GetDatabase(mongoOptions.Database));

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };

        ConventionRegistry.Register(
            "CamelCase",
            conventionPack,
            t => true);

        return services;
    }

    private static IServiceCollection AddUsersServiceClient(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<UsersServiceOptions>(opt =>
        {
            opt.Host = config["USERSERVICE_HOST"]!;
            opt.Port = config["USERSERVICE_PORT"]!;
        });

        services.AddHttpClient<IUsersServiceClient, UsersServiceClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<UsersServiceOptions>>().Value;
            client.BaseAddress = new Uri($"http://{options.Host}:{options.Port}");
        });

        return services;
    }

    private static IServiceCollection AddProductsServiceClient(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ProductsServiceOptions>(opt =>
        {
            opt.Host = config["PRODUCTSERVICE_HOST"]!;
            opt.Port = config["PRODUCTSERVICE_PORT"]!;
        });

        services.AddHttpClient<IProductsServiceClient, ProductsServiceClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ProductsServiceOptions>>().Value;
            client.BaseAddress = new Uri($"http://{options.Host}:{options.Port}");
        });

        return services;
    }
}
