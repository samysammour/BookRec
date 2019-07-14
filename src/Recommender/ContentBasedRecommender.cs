namespace BookRec.Recommender
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Infrastructure.EntityFramework.Repositories;
    using EnsureThat;

    public class ContentBasedRecommender
    {
        private readonly IBookRepository repository;
        private readonly ContentBasedRecommenderOptions options;

        public ContentBasedRecommender(IBookRepository repository, ContentBasedRecommenderOptions options)
        {
            EnsureArg.IsNotNull(repository);
            EnsureArg.IsNotNull(options);

            this.repository = repository;
            this.options = options;
        }

        public async Task<IEnumerable<Book>> GetPredicationAsync(IEnumerable<Book> inputs)
        {
            return await this.repository.GetAllAsync().ConfigureAwait(false);
        }
    }
}
