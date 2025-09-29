using eCommerce.OrdersService.Application.RepositoryContracts;
using eCommerce.OrdersService.Infrastructure.MappingProfiles;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace eCommerce.OrdersService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(cfg => { }, typeof(OrderItemMappingProfile));

        services.Configure<CollectionOptions>(config.GetSection("Mongo:Collections"));
        services.Configure<MongoOptions>(config.GetSection("Mongo"));

        var user = config["Mongo:User"];
        var pass = config["Mongo:Pass"];
        var host = config["Mongo:Host"];
        var port = config["Mongo:Port"];
        var connectionString = $"mongodb://{user}:{pass}@{host}:{port}";

        var mongoClient = new MongoClient(connectionString);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped<IMongoDatabase>(sp =>
        {
            return mongoClient.GetDatabase(config["Mongo:Database"]);
        });

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };

        ConventionRegistry.Register(
            "CamelCase",
            conventionPack,
            t => true);


        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}
