namespace BookRec.Infrastructure.EntityFramework.Repositories
{
    using System.Threading.Tasks;
    using BookRec.Infrastructure.EntityFramework.Models;

    public interface IBookRepository : IBaseRepository<Book>
    {
        Task<Book> GetByTitleAsync(string title);
    }
}
