namespace BookRec.Infrastructure.EntityFramework.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using BookRec.Recommender;

    public class PredicationModel
    {
        public Book Book { get; set; }

        public double Score { get; set; }

        public void CalculateWeight(ContentBasedRecommenderOptions options)
            => this.Score = options.HotFactorsSatisfactions(this.Book) + options.WarmFactorsSatisfactions(this.Book);
    }
}
