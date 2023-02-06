using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB {
    public static class Extensions {
        public static WebApplicationBuilder AddMongo(this WebApplicationBuilder builder) {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            builder.Services.AddSingleton(serviceProvider => {
                ConfigurationManager configuration = builder.Configuration;
                ServiceSettings serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                MongoDbSettings mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return builder;
        }

        public static WebApplicationBuilder AddMongoRepository<T>(this WebApplicationBuilder builder, string collectionName) 
            where T : IEntity 
        {
            builder.Services.AddSingleton<IRepository<T>>(serviceProvider => {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, "items");
            });

            return builder;
        }
    }
}