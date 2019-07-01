namespace BookRec.Tools.Migration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Api;
    using Infrastructure.EntityFramework.Repositories;
    using Microsoft.Extensions.DependencyInjection;

    public static class BookMigration
    {
        public static async Task StartAsync(string query, ServiceProvider serviceProvider)
        {
            var client = serviceProvider.GetService<IBookClient>();
            var repositopry = serviceProvider.GetService<IBookRepository>();

            Console.WriteLine($"Get books for {query}");
            var books = await client.GetBooksAsync(query).ConfigureAwait(false);

            if (books != null && books.Any())
            {
                Console.WriteLine($"Book Found: {books.Count()}");
                foreach(var book in books)
                {
                    try
                    {
                        var model = await repositopry.InsertAsync(book).ConfigureAwait(false);
                        if (model == null)
                        {
                            throw new ArgumentException($"Book {book.Title} cannot be found");
                        }

                        Console.WriteLine($"{book.Title} added successfullt into Database");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
