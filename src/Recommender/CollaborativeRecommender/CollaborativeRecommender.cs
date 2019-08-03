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

        public async Task<List<PredicationModel>> GetPredicationsByBooksAsync(List<UserBook> inputs, string username)
        {
            var helper = new CollaborativeRecommenderHelper(inputs);
            var userGroups = await (from userBook in this.repository.DbContext.UserBooks.Include(x => x.Book)
                                    join book in this.repository.DbContext.Books on userBook.BookId equals book.Id
                                    where (from subUserBook in this.repository.DbContext.UserBooks
                                           join subBook in this.repository.DbContext.Books on subUserBook.BookId equals subBook.Id
                                           where helper.Ids.Any(id => id == subUserBook.BookId) && subUserBook.Username != username
                                           select subUserBook.Username).Distinct().Contains(userBook.Username)
                                    group userBook by userBook.Username into userGroup
                                    select new
                                    {
                                                Group = userGroup,
                                                Temperature = helper.CalculateUTM(userGroup),
                                                Nos = helper.CalculateNOS(userGroup)
                                    })
                             .OrderByDescending(x => x.Temperature).Take(500).ToSafeListAsync();

            var outputWeights = userGroups.SelectMany(group =>
            {
                return group.Group.Where(x => !helper.Ids.Contains(x.BookId))
                        .Select(item => new
                        {
                            item.Book,
                            Score = helper.CalculateOPW(group.Temperature, item.Rating),
                            group.Nos
                        });
            });

            return outputWeights.GroupBy(x => x.Book.Id)
                    .Select(group =>
                    {
                        var prediction = new PredicationModel()
                        {
                            Book = group.FirstOrDefault().Book
                        };

                        if (group.Count() == 1)
                        {
                            prediction.Score = group.FirstOrDefault().Score;
                        }
                        else
                        {
                            prediction.Score = (double)(group.Sum(x => x.Nos * x.Score) / group.Count());
                        }

                        return prediction;
                    }).OrderByDescending(x => x.Score).Take(10).ToList();
        }
    }
}
