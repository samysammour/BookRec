using System.Collections.Generic;
using System.Threading.Tasks;
using BookRec.Infrastructure.EntityFramework.Models;

namespace BookRec.Infrastructure.EntityFramework.Repositories
{
    public class BookRepository : IBookRepository
    {
        //private readonly DbContext dbContext;

        //public BookRepository()
        //{
        //    this.dbContext = dbContext;
        //}
        public Task<bool> DeleteAsync(int id) => throw new System.NotImplementedException();
        public Task<IEnumerable<Book>> GetAllAsync() => throw new System.NotImplementedException();
        public Task<Book> GetByIdAsync(int id) => throw new System.NotImplementedException();
        public Task<Book> InsertAsync(Book model) => throw new System.NotImplementedException();
        public Task<Book> UpdateAsync(Book model) => throw new System.NotImplementedException();
    }
}
