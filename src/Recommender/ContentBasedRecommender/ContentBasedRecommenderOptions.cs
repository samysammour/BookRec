namespace BookRec.Recommender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using BookRec.Common;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Recommender.Helpers;
    using EnsureThat;

    public class ContentBasedRecommenderOptions
    {
        public ContentBasedRecommenderOptions(List<Book> inputs)
        {
            EnsureArg.IsNotNull(inputs);

            this.InitOptions(inputs);
        }

        public IEnumerable<Expression<Func<Book, bool>>> HotFactors { get; set; }

        public IEnumerable<Expression<Func<Book, bool>>> WarmFactors { get; set; }

        public IEnumerable<Expression<Func<Book, bool>>> ColdFactors { get; set; }

        public Expression<Func<Book, bool>> AndExpressions()
        {
            var andEXpressions = this.HotFactors.ToArray().Aggregate((l, r) => l.And(r));
            //var orEXpressions = this.WarmFactors.ToArray().Aggregate((l, r) => l.Or(r));

            return andEXpressions;
            //var invokedExpr = Expression.Invoke(andEXpressions, orEXpressions.Parameters.Cast<Expression>());
            //return Expression.Lambda<Func<Book, bool>>(Expression.Or(orEXpressions.Body, invokedExpr), andEXpressions.Parameters);
        }

        public Expression<Func<Book, bool>> OrExpressions()
        {
            var orEXpressions = this.WarmFactors.ToArray().Aggregate((l, r) => l.Or(r));

            return orEXpressions;
        }

        public double GetHotFactorWeight()
            => (double)this.GetWarmFactorWeight() * 2;

        public double GetWarmFactorWeight()
            => (double)1 / ((this.HotFactors.Count() * 2) + this.WarmFactors.Count());

        public double HotFactorsSatisfactions(Book input)
        {
            var hotFactorWeight = 0.0;
            var weightPerFactor = this.GetHotFactorWeight();
            if (input == default)
            {
                return 0.0;
            }

            foreach (var exp in this.HotFactors)
            {
                Func<Book, bool> predicate = exp.ToPredicate();
                hotFactorWeight = predicate(input) ? hotFactorWeight + weightPerFactor : hotFactorWeight;
            }

            return hotFactorWeight;
        }

        public double WarmFactorsSatisfactions(Book input)
        {
            var warmFactorWeight = 0.0;
            var weightPerFactor = this.GetWarmFactorWeight();
            if (input == default)
            {
                return 0.0;
            }

            foreach (var exp in this.WarmFactors)
            {
                Func<Book, bool> predicate = exp.ToPredicate();
                warmFactorWeight = predicate(input) ? warmFactorWeight + weightPerFactor : warmFactorWeight;
            }

            return warmFactorWeight;
        }

        private void InitOptions(List<Book> books)
        {
            EnsureArg.IsNotNull(books);
            this.HotFactors = new List<Expression<Func<Book, bool>>>()
                {
                    t => books.Any(x => x.Categories == t.Categories),
                    t => books.Any(x => x.MaturityRating == t.MaturityRating),
                    t => books.Any(x => x.LanguageCode == t.LanguageCode),
                    t => books.Any(x => x.Country == t.Country),
                };

            this.WarmFactors = new List<Expression<Func<Book, bool>>>()
                {
                    t => books.Any(x => x.Authors == t.Authors),
                    t => books.Any(x => x.Publisher == t.Publisher),
                    t => books.Any(x => t.PublishedDate.Year < x.PublishedDate.Year + 50 && t.PublishedDate.Year > x.PublishedDate.Year - 50),
                };

            this.ColdFactors = new List<Expression<Func<Book, bool>>>()
                {
                    t => books.Any(x => x.Title == t.Title),
                    t => books.Any(x => x.Subtitle == t.Subtitle),
                    t => books.Any(x => x.PageCount == t.PageCount),
                    t => books.Any(x => x.ImageLink == t.ImageLink),
                    t => books.Any(x => x.ContainsImageBubbles == t.ContainsImageBubbles),
                    t => books.Any(x => x.PrintType == t.PrintType),
                    t => books.Any(x => x.PreviewLink == t.PreviewLink),
                    t => books.Any(x => x.PublicDomain == t.PublicDomain),
                };
        }
    }
}
