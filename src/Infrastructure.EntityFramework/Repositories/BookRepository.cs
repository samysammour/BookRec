namespace BookRec.Infrastructure.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BookRec.Infrastructure.EntityFramework.Context;
    using BookRec.Infrastructure.EntityFramework.Models;
    using EFCore.BulkExtensions;
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;

    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(BookRecContext dbContext)
            : base(dbContext)
        {
            EnsureArg.IsNotNull(dbContext);
        }

        public IQueryable<Book> GetQuery() => this.DbContext.Books;

        public async Task<Book> GetByTitleAsync(string title)
        {
            EnsureArg.IsNotNull<string>(title);
            return await this.DbContext.Books.FirstOrDefaultAsync(x => x.Title == title);
        }
    }
}
