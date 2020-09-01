using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4jClient;
using System;

namespace GenericRepository.Graph
{
    public static class Setup
    {
        public static IServiceCollection AddGraphDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Neo4jConfig>(configuration.GetSection("Neo4jConfig"));

            services.AddScoped(typeof(IGraphClient), provider =>
            {
                var options = provider.GetService<IOptions<Neo4jConfig>>();
                var client = new BoltGraphClient(new Uri(options.Value.uri), username: options.Value.username, password: options.Value.password);

                client.Connect();

                return client;
            });

            return services;
        }

        private class Neo4jConfig
        {
            public string uri { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }
    }
}
