using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HireAI.Infrastructure.GenaricBasies
{
    public interface IRepository<T> where T : class
    {

        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(int id);
        Task<T> InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
