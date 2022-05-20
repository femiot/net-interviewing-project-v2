using Insurance.Core.Interfaces;
using Insurance.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Insurance.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private DbSet<TEntity> _dbSet;
        private DbContext _insuranceContext;

        public GenericRepository(DbContext context)
        {
            _insuranceContext = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() => _dbSet.AsNoTracking().Where(predicate));
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _insuranceContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
