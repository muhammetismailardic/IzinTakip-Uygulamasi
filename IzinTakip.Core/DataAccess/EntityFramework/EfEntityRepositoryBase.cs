using IzinTakip.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IzinTakip.Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
    where TEntity : class, IEntity, new()
    where TContext : DbContext, new()
    {
        public virtual async Task CreateAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                var addedEntity = context.Entry(entity);
                

                addedEntity.State = EntityState.Added;
                
                await context.SaveChangesAsync();
            }
        }
        public virtual async Task DeleteAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                var deletedEntity = context.Entry(entity);

                deletedEntity.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }
        public virtual async Task<ICollection<TEntity>> GetListWithIncludesAsync(Expression<Func<TEntity, bool>> predicate = null, params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
        {
            using (var context = new TContext())
            {
                IQueryable<TEntity> entityQuery;
                if (predicate == null)
                {
                    entityQuery = context.Set<TEntity>();
                }
                else
                {
                    entityQuery = context.Set<TEntity>().Where(predicate);
                }

                foreach (var include in includes)
                {
                    entityQuery = include(entityQuery);
                }

                return await entityQuery.ToListAsync();
            }
        }
        public virtual async Task<TEntity> GetWithInludesAsync(Expression<Func<TEntity, bool>> predicate, params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
        {
            using (var context = new TContext())
            {
                IQueryable<TEntity> entityQuery = context.Set<TEntity>().Where(predicate);

                foreach (var include in includes)
                {
                    entityQuery = include(entityQuery);
                }

                return await entityQuery.SingleOrDefaultAsync();
            }
        }
        public virtual async Task UpdateAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                var updatedEntity = context.Entry(entity);

                updatedEntity.State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
