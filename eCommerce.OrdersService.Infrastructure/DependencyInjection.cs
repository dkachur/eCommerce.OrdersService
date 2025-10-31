using eCommerce.OrdersService.Application.RepositoryContracts;
using eCommerce.OrdersService.Application.ServiceContracts;
using eCommerce.OrdersService.Infrastructure.Cache;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Products;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Products.Config;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Users;
using eCommerce.OrdersService.Infrastructure.ExternalServices.Users.Config;
using eCommerce.OrdersService.Infrastructure.MappingProfiles;
using eCommerce.OrdersService.Infrastructure.Messaging.ConnectionManagers;
using eCommerce.OrdersService.Infrastructure.Messaging.Consumers;
using eCommerce.OrdersService.Infrastructure.Messaging.DTO;
using eCommerce.OrdersService.Infrastructure.Messaging.Handlers;
using eCommerce.OrdersService.Infrastructure.Messaging.HostedServices;
using eCommerce.OrdersService.Infrastructure.Messaging.Interfaces;
using eCommerce.OrdersService.Infrastructure.Messaging.Options;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Repositories;
using eCommerce.OrdersService.Infrastructure.Policies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Polly;
using StackExchange.Redis;

namespace eCommerce.OrdersService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(cfg => { }, typeof(OrderItemMappingProfile));

        services.AddMongo(config);
        services.AddRedis(config);
        services.AddUsersServiceClient(config);
        services.AddProductsServiceClient(config);
        services.AddRabbitMq(config);

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
            opt.Host = config["APIGATEWAY_HOST"]!;
            opt.Port = config["APIGATEWAY_PORT"]!;
        });

        services.AddKeyedSingleton<IAsyncPolicy<HttpResponseMessage>>(
            "UsersPolicy",
            (sp, _) =>
            {
                var logger = sp.GetRequiredService<ILogger<UsersServiceClient>>();
                var policyBuilder = new HttpPolicyBuilder(logger, nameof(UsersServiceClient));

                return policyBuilder
                    .WithRetry()
                    //.WithCircuitBreaker()
                    .WithTimeout()
                    .WithBulkhead()
                    .Build();
            });

        services.AddHttpClient<UsersServiceClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<UsersServiceOptions>>().Value;
            client.BaseAddress = new Uri($"http://{options.Host}:{options.Port}");
        })
        .AddPolicyHandler((sp, _) => sp.GetRequiredKeyedService<IAsyncPolicy<HttpResponseMessage>>("UsersPolicy"));

        services.AddScoped<IUsersServiceClient, CachedUsersServiceClient>(sp =>
        {
            var inner = sp.GetRequiredService<UsersServiceClient>();
            var cache = sp.GetRequiredService<ICacheService>();
            var logger = sp.GetRequiredService<ILogger<CachedUsersServiceClient>>();

            return new(inner, cache, logger);
        });

        return services;
    }

    private static IServiceCollection AddProductsServiceClient(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ProductsServiceOptions>(opt =>
        {
            opt.Host = config["APIGATEWAY_HOST"]!;
            opt.Port = config["APIGATEWAY_PORT"]!;
        });

        services.AddKeyedSingleton<IAsyncPolicy<HttpResponseMessage>>(
            "ProductsPolicy",
            (sp, obj) =>
            {
                var logger = sp.GetRequiredService<ILogger<ProductsServiceClient>>();
                var policyBuilder = new HttpPolicyBuilder(logger, nameof(ProductsServiceClient));

                return policyBuilder
                    .WithRetry(initialDelaySeconds: 1.1)
                    //.WithCircuitBreaker()
                    .WithTimeout()
                    .WithBulkhead()
                    .Build();
            });

        services.AddHttpClient<ProductsServiceClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ProductsServiceOptions>>().Value;
            client.BaseAddress = new Uri($"http://{options.Host}:{options.Port}");
        })
        .AddPolicyHandler((sp, _) => sp.GetRequiredKeyedService<IAsyncPolicy<HttpResponseMessage>>("ProductsPolicy"));

        services.AddScoped<IProductsServiceClient, CachedProductsServiceClient>(sp =>
        {
            var inner = sp.GetRequiredService<ProductsServiceClient>();
            var cache = sp.GetRequiredService<ICacheService>();
            var logger = sp.GetRequiredService<ILogger<CachedProductsServiceClient>>();

            return new(inner, cache, logger);
        });

        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
    {
        var host = config["REDIS_HOST"];
        var port = config["REDIS_PORT"];

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect($"{host}:{port}"));

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }

    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration config)
    {
        var options = new RabbitMqOptions()
        {
            Host = config["RABBITMQ_HOST"] ?? "localhost",
            Port = Convert.ToInt32(config["RABBITMQ_PORT"]),
            Username = config["RABBITMQ_USER"] ?? "guest",
            Password = config["RABBITMQ_PASS"] ?? "password",
            ProductsExchange = config["RABBITMQ_PRODUCTS_EXCHANGE"] ?? "products.exchange",
            ProductNameUpdatedRoutingKey = config["RABBITMQ_PRODUCT_NAME_UPDATED_ROUTING_KEY"] ?? "product.name.updated",
        };

        services.Configure<RabbitMqOptions>((opt) =>
        {
            opt.Host = options.Host;
            opt.Port = options.Port;
            opt.Username = options.Username;
            opt.Password = options.Password;
            opt.ProductsExchange = options.ProductsExchange;
            opt.ProductNameUpdatedRoutingKey = options.ProductNameUpdatedRoutingKey;
        });

        services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();
        services.AddHostedService<RabbitMqConnectionHostedService>();
        services.AddTransient<IMessageHandler<ProductNameUpdatedMessage>, ProductNameUpdatedHandler>();
        services.AddSingleton<IMessageConsumer<ProductNameUpdatedMessage>, RabbitMqProductNameUpdatedConsumer>();
        services.AddHostedService<RabbitMqConsumersHostedService>();

        return services;
}
}
