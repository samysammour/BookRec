﻿namespace BookRec.Recommender
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BookRec.Infrastructure.EntityFramework.Models;

    public interface IContentBasedRecommender
    {
        Task<List<PredicationModel>> GetPredicationsByBooksAsync(List<Book> inputs);
    }
}
