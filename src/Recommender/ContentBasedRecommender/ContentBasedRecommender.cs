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

        public async Task<IEnumerable<Book>> GetPredicationAsync(IEnumerable<Book> inputs)
        {
            var exprs = new List<Expression<Func<Book, bool>>>();
            foreach(var input in inputs)
            {
                exprs.Concat(this.CreateOptions(input).ToExpressions());
            }

            var query = this.repository.GetQuery()
                            .WhereExpressions(exprs.AsEnumerable());
            return await Task.FromResult(query);
        }

        private ContentBasedRecommenderOptions CreateOptions(Book book)
        {
            EnsureArg.IsNotNull(book);

            return new ContentBasedRecommenderOptions()
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
        }
    }
}
