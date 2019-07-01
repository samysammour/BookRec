namespace BookRec.Tools.Migration
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure.Api;
    using Infrastructure.EntityFramework;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();

            var config = builder.Build();
            var services = new ServiceCollection();
            var serviceProvider = services
                .AddDatabase()
                .AddClients()
                .AddRepositories()
                .AddHttpClient()
                .BuildServiceProvider();

            Console.WriteLine("Start!!!");

            var query = string.Empty;
            while(!query.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                query = Console.ReadLine();
                await BookMigration.StartAsync(query, serviceProvider).ConfigureAwait(false);
            }
        }
    }
}
