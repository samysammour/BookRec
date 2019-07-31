namespace BookRec.Recommender
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

            return await this.repository.GetQuery()
                .Where(x => inputs.All(i => x.Id != i.Id))
                .Where(options.AndExpressions())
                .Where(options.OrExpressions())
                .Take(1000)
                .AsEnumerable()
                .Select(entry =>
                {
                    var predication = new PredicationModel { Book = entry, Score = 0.0 };
                    predication.CalculateWeight(options);
                    return predication;
                })
                .OrderByDescending(x => x.Score)
                .Take(10)
                .AsQueryable()
                .ToSafeListAsync();
        }
    }
}
