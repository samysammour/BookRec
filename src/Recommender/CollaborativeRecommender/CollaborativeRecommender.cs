namespace BookRec.Recommender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BookRec.Common;
    using BookRec.Infrastructure.EntityFramework.Extensions;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Infrastructure.EntityFramework.Repositories;
    using BookRec.Recommender.Models;
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;

    public class CollaborativeRecommender : ICollaborativeRecommender
    {
        private readonly IUserBookRepository repository;

        public CollaborativeRecommender(IUserBookRepository repository)
        {
            EnsureArg.IsNotNull(repository);

            this.repository = repository;
        }

        public async Task<List<PredicationModel>> GetPredicationsByBooksAsync(List<Book> inputs, string username)
        {
            var ids = inputs.Select(x => x.Id).Distinct();
            var similarBooks = await
                (from userBook in this.repository.DbContext.UserBooks.Include(x => x.Book)
                join book in this.repository.DbContext.Books on userBook.BookId equals book.Id
                where (from subUserBook in this.repository.DbContext.UserBooks
                       join subBook in this.repository.DbContext.Books on subUserBook.BookId equals subBook.Id
                       where ids.Any(id => id == subUserBook.BookId) && subUserBook.Username != username
                       select subUserBook.Username).Distinct().Contains(userBook.Username)
                        &&
                        !ids.Contains(userBook.BookId)
                select userBook).ToSafeListAsync();

            var helper = new CollaborativeRecommenderHelper(similarBooks);

            return similarBooks.DistinctBy(x => x.BookId).Select(userBook =>
            {
                var group = helper.FindGroup(userBook.BookId);
                var item = new IterationModel()
                {
                    Book = userBook.Book,
                    Iteration = group.Count(),
                    Rating = helper.CalculateRating(group)
                };

                return item;
            }).Select(model => new PredicationModel()
            {
                Book = model.Book,
                Score = helper.CalculateScore(model)
            }).OrderByDescending(x => x.Score).Take(10).ToList();
        }
    }
}
