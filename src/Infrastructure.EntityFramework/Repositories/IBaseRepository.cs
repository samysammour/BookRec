using BookRec.Infrastructure.EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookRec.Infrastructure.EntityFramework.Repositories
{
    public interface IBaseRepository<T>
        where T : AggregateRoot
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<T> InsertAsync(T model);

        Task<T> UpdateAsync(T model);

        Task<bool> DeleteAsync(int id);
    }
}
