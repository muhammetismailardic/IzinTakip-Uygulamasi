using IzinTakip.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IzinTakip.Core.DataAccess
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<ICollection<T>> GetListWithIncludesAsync(Expression<Func<T, bool>> predicate = null, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes);
        Task<T> GetWithInludesAsync(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes);
    }
}
