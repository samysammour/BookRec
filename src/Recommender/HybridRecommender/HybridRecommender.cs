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

    public class HybridRecommender : IHybridRecommender
    {
        private readonly IContentBasedRecommender contentBasedRecommender;
        private readonly ICollaborativeRecommender collaborativeRecommender;

        public HybridRecommender(IContentBasedRecommender contentBasedRecommender, ICollaborativeRecommender collaborativeRecommender)
        {
            EnsureArg.IsNotNull(contentBasedRecommender);
            EnsureArg.IsNotNull(collaborativeRecommender);

            this.contentBasedRecommender = contentBasedRecommender;
            this.collaborativeRecommender = collaborativeRecommender;
        }

        public async Task<List<PredicationModel>> GetPredicationsByBooksAsync(List<UserBook> inputs, string username)
        {
            var cbfTask = this.contentBasedRecommender.GetPredicationsByBooksAsync(inputs.Select(x => x.Book).ToList());
            var cfTask = this.collaborativeRecommender.GetPredicationsByBooksAsync(inputs, username);

            await Task.WhenAll(cbfTask, cfTask).ConfigureAwait(false);

            var cbfPrediction = cbfTask.Result;
            var cfPrediction = cfTask.Result;
            return cbfPrediction.SafeConcat(cfPrediction).GroupBy(x => x.Book.Id).Select(group =>
            {
                if (group.Count() == 1)
                {
                    return group.FirstOrDefault();
                }

                return new PredicationModel()
                {
                    Book = group.FirstOrDefault().Book,
                    Score = group.Sum(x => x.Score) / group.Count()
                };
            }).DistinctBy(x => x.Book.Id).OrderByDescending(x => x.Score).Take(10).ToList();
        }
    }
}
