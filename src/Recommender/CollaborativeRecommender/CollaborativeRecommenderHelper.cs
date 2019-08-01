namespace BookRec.Recommender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Recommender.Models;
    using EnsureThat;

    public class CollaborativeRecommenderHelper
    {
        public CollaborativeRecommenderHelper(List<UserBook> inputs)
        {
            EnsureArg.IsNotNull(inputs);

            this.PrepareGroups(inputs);
        }

        public IEnumerable<IGrouping<Guid, UserBook>> Groups { get; set; }

        public int MaxIteration { get; set; }

        public IGrouping<Guid, UserBook> FindGroup(Guid bookId)
            => this.Groups.FirstOrDefault(g => g.Key == bookId);

        public int CalculateRating(IGrouping<Guid, UserBook> group)
            => (int)Math.Ceiling((double)group.Sum(x => x.Rating) / group.Count());

        public double CalculateScore(IterationModel model)
            => ((0.75 / this.MaxIteration) * model.Iteration) + (0.05 * model.Rating);

        private void PrepareGroups(List<UserBook> inputs)
        {
            this.Groups = inputs.GroupBy(i => i.BookId);
            this.MaxIteration = this.Groups.Max(x => x.Count());
        }
    }
}
