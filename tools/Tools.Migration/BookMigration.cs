namespace BookRec.Tools.Migration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Infrastructure.Api;
    using Infrastructure.EntityFramework.Repositories;
    using Microsoft.Extensions.DependencyInjection;

    public static class BookMigration
    {
        public static async Task StartAsync(ServiceProvider serviceProvider)
        {
            var client = serviceProvider.GetService<IBookClient>();
            var repositopry = serviceProvider.GetService<IBookRepository>();

            foreach(var query in BookCategories.GetSearchQueries())
            {
                Console.WriteLine($"Get books for {query}");
                for(int i = 0; i < 10; i++)
                {
                    var books = await client.GetBooksAsync(HttpUtility.HtmlEncode(query), i).ConfigureAwait(false);
                    if (books != null && books.Any())
                    {
                        Console.WriteLine($"Book Found: {books.Count()}");
                        foreach(var book in books)
                        {
                            try
                            {
                                var model = await repositopry.GetByTitleAsync(book.Title).ConfigureAwait(false);
                                if (model == null)
                                {
                                    model = await repositopry.InsertAsync(book).ConfigureAwait(false);
                                    Console.WriteLine($"{book.Title} added successfullt into Database");
                                }
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
    }
}
