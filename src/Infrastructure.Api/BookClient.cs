namespace BookRec.Infrastructure.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using BookRec.Common;
    using BookRec.Infrastructure.EntityFramework.Models;
    using EnsureThat;
    using Newtonsoft.Json.Linq;

    public class BookClient : IBookClient
    {
        private readonly IHttpClientFactory clientFactory;

        public BookClient(IHttpClientFactory clientFactory)
        {
            EnsureArg.IsNotNull(clientFactory);
            this.clientFactory = clientFactory;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(string query)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q={query}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = this.clientFactory.CreateClient();

            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Query {query} cannot be found");
            }

            var text = await response.Content.ReadAsStringAsync();
            var json = JToken.Parse(text);
            var result = new List<Book>();
            foreach (var res in json["items"])
            {
                var categories= res.GetValuesByPath<string>("volumeInfo.categories[0]");
                var authors = res.GetValuesByPath<object>("volumeInfo.authors[0]");
                var book = new Book()
                {
                    Title = res.GetValueByPath<string>("volumeInfo.title"),
                    Subtitle = res.GetValueByPath<string>("volumeInfo.subtitle"),
                    Authors = authors != null ? string.Join(";", authors.ToArray()) : string.Empty,
                    Publisher = res.GetValueByPath<string>("volumeInfo.publisher"),
                    PublishedDate = res.GetValueByPath<DateTime>("volumeInfo.publishedDate"),
                    PageCount = res.GetValueByPath<int>("volumeInfo.pageCount"),
                    Categories = categories != null ? string.Join(";", categories.ToArray()) : string.Empty,
                    MaturityRating = res.GetValueByPath<string>("volumeInfo.maturityRating"),
                    ImageLink = res.GetValueByPath<string>("volumeInfo.imageLinks.thumbnail"),
                    ContainsImageBubbles = res.GetValueByPath<string>("volumeInfo.title"),
                    LanguageCode = res.GetValueByPath<string>("volumeInfo.language"),
                    PrintType = res.GetValueByPath<string>("volumeInfo.printType"),
                    PreviewLink = res.GetValueByPath<string>("volumeInfo.previewLink"),
                    Country = res.GetValueByPath<string>("accessInfo.country"),
                    PublicDomain = res.GetValueByPath<bool>("accessInfo.publicDomain"),
                };

                result.Add(book);
            }

            return result;
        }
    }
}
