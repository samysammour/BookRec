namespace BookRec.Infrastructure.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BookRec.Common;
    using BookRec.Infrastructure.EntityFramework.Context;
    using BookRec.Infrastructure.EntityFramework.Models;
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;

    public class BookRepository : IBookRepository
    {
        private readonly BookRecContext dbContext;

        public BookRepository(BookRecContext dbContext)
        {
            EnsureArg.IsNotNull(dbContext, nameof(dbContext));

            this.dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            EnsureArg.IsNotNull<Guid>(id);
            var entity = await this.dbContext.Books.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (entity != null)
            {
                this.dbContext.Remove(entity);
                await this.dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
            => await this.dbContext.Books.ToListAsync();

        public async Task<Book> GetByIdAsync(Guid id)
        {
            EnsureArg.IsNotNull<Guid>(id);
            return await this.dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Book> InsertAsync(Book model)
        {
            EnsureArg.IsNotNull<Book>(model);
            this.dbContext.Add(model);
            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
            return model;
        }

        public async Task<Book> UpdateAsync(Book model)
        {
            EnsureArg.IsNotNull<Book>(model);
            var entity = this.dbContext.Books.Find(model.Id);
            if (entity == null)
            {
                return null;
            }

            entity.Title = model.Title;
            entity.Subtitle = model.Subtitle;
            entity.Authors = model.Authors;
            entity.Publisher = model.Publisher;
            entity.PublishedDate = model.PublishedDate;
            entity.PageCount = model.PageCount;
            entity.Categories = model.Categories;
            entity.MaturityRating = model.MaturityRating;
            entity.ImageLink = model.ImageLink;
            entity.ContainsImageBubbles = model.ContainsImageBubbles;
            entity.LanguageCode = model.LanguageCode;
            entity.PrintType = model.PrintType;
            entity.PreviewLink = model.PreviewLink;
            entity.Country = model.Country;
            entity.PublicDomain = model.PublicDomain;

            this.dbContext.Entry(model).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync().ConfigureAwait(false);
            return model;
        }
    }
}
