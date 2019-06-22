namespace Aan.Infrastructure.EntityFramework
{
    using BookRec.Infrastructure.EntityFramework.Context;
    using BookRec.Infrastructure.EntityFramework.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static partial class ServiceRegistrations
    {
        /// <summary>
        /// Adds Aan Repositories
        /// </summary>
        /// <param name="services">The services</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.TryAddScoped<IBookRepository>(sp =>
            {
                //var context = sp.GetService<AanContext>();
                return new BookRepository();
            });

            return services;
        }

        /// <summary>
        /// Adds Aan database
        /// </summary>
        /// <param name="services">The services</param>
        /// <returns>Service Collection</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            services.AddDbContext<BookRecContext>(options =>
            options.UseSqlServer("Data Source=bookrec.database.windows.net,1433;Initial Catalog=BookRec;User ID=bookrec;Password=SD@!#!2sad"));
            return services;
        }
    }
}
