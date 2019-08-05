namespace BookRec.Recommender
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BookRec.Common;
    using BookRec.Infrastructure.EntityFramework.Extensions;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Infrastructure.EntityFramework.Repositories;
    using EnsureThat;

    public class ContentBasedRecommender : IContentBasedRecommender
    {
        private readonly IBookRepository repository;

        public ContentBasedRecommender(IBookRepository repository)
        {
            EnsureArg.IsNotNull(repository);

            this.repository = repository;
        }

        public async Task<List<PredicationModel>> GetPredicationsByBooksAsync(List<Book> inputs)
        {
            if (inputs == null || !inputs.Any())
            {
                return new List<PredicationModel>();
            }

            var options = new ContentBasedRecommenderOptions(inputs);
            var x = await (from book in this.repository.DbContext.Books
                          let weight = options.HotFactorsSatisfaction(book) + options.WarmFactorsSatisfaction(book)
                          where weight >= options.MinScore()
                          where inputs.All(x => x.Id != book.Id)
                          orderby weight descending
                          select new PredicationModel { Book = book, Score = options.CalculateScore(weight) }).Take(10).ToSafeListAsync();

            return x;
        }
    }
}
