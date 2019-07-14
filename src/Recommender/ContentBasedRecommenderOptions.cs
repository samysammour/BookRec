namespace BookRec.Recommender
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using BookRec.Infrastructure.EntityFramework.Models;

    public class ContentBasedRecommenderOptions
    {
        public IEnumerable<Expression<Func<Book, bool>>> HotFactors { get; set; }

        public IEnumerable<Expression<Func<Book, bool>>> WarmFactors { get; set; }

        public IEnumerable<Expression<Func<Book, bool>>> ColdFactors { get; set; }
    }
}
