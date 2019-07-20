namespace BookRec.Recommender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Infrastructure.EntityFramework.Repositories;
    using Common;
    using EnsureThat;

    public class ContentBasedRecommender : IContentBasedRecommender
    {
        private readonly IBookRepository repository;

        public ContentBasedRecommender(IBookRepository repository)
        {
            EnsureArg.IsNotNull(repository);

            this.repository = repository;
        }

        public async Task<IEnumerable<Book>> GetPredicationAsync(Book input)
        {
            //var exprs = new List<Expression<Func<Book, bool>>>();
            //foreach(var input in inputs)
            //{
            //    exprs.AddRange(this.CreateOptions(input).ToExpressions());
            //}

            //var query = this.repository.GetQuery()
            //    .Where(x =>
            //        inputs.Any(input => input.Categories == x.Categories) ||
            //        inputs.Any(input => input.MaturityRating == x.MaturityRating) ||
            //        inputs.Any(input => input.LanguageCode == x.LanguageCode) ||
            //        inputs.Any(input => input.Country == x.Country) ||
            //        inputs.Any(input => input.Authors == x.Authors) ||
            //        inputs.Any(input => input.Publisher == x.Publisher) ||
            //        inputs.Any(input => input.PublishedDate.Year + 30 < x.PublishedDate.Year && input.PublishedDate.Year - 30 > x.PublishedDate.Year));

            var query = this.repository.GetQuery()
                .Where(x =>
                    input.Categories == x.Categories ||
                    input.Authors == x.Authors)
                .Select(entry => new { Book = entry, Weight = this.CalculateWeight(entry, input)})
                .OrderBy(x => x.Weight)
                .Take(10).ToList();
                    //{
                    //    double weight = 0;
                    //    if (entry.Categories == input.Categories)
                    //    {
                    //        weight += 0.7;
                    //    }

                    //    if (entry.Authors == input.Authors)
                    //    {
                    //        weight += 0.3;
                    //    }

                    //    return entry;
                    //});
            return await Task.FromResult(query.Select(x => x.Book));
        }

        private ContentBasedRecommenderOptions CreateOptions(Book book)
        {
            EnsureArg.IsNotNull(book);

            var expr = new ContentBasedRecommenderOptions()
            {
                HotFactors = new List<Expression<Func<Book, bool>>>()
                {
                    t => t.Categories == book.Categories,
                    t => t.MaturityRating == book.MaturityRating,
                    t => t.LanguageCode == book.LanguageCode,
                    t => t.Country == book.Country
                },
                WarmFactors = new List<Expression<Func<Book, bool>>>()
                {
                    t => t.Authors == book.Authors,
                    t => t.Publisher == book.Publisher,
                    t => t.PublishedDate.Year < book.PublishedDate.Year + 30 && t.PublishedDate.Year > book.PublishedDate.Year - 30,
                },
                ColdFactors = new List<Expression<Func<Book, bool>>>()
                {
                    t => t.Title == book.Title,
                    t => t.Subtitle == book.Subtitle,
                    t => t.PageCount == book.PageCount,
                    t => t.ImageLink == book.ImageLink,
                    t => t.ContainsImageBubbles == book.ContainsImageBubbles,
                    t => t.PrintType == book.PrintType,
                    t => t.PreviewLink == book.PreviewLink,
                    t => t.PublicDomain == book.PublicDomain
                }
            };

            return expr;
        }

        private double CalculateWeight(Book dbObj, Book input)
        {
            double weight = 0;
            if (dbObj.Categories.Equals(input.Categories, StringComparison.OrdinalIgnoreCase))
            {
                weight += 0.7;
            }

            if (dbObj.Authors.Equals(input.Authors, StringComparison.OrdinalIgnoreCase))
            {
                weight += 0.3;
            }

            return weight;
        }
    }
}
