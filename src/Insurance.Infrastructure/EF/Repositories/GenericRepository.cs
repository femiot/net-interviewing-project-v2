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

        public async Task<TEntity?> FirstOrDefaultAsync()
        {
            return await _dbSet.FirstOrDefaultAsync();
        }

        public TEntity? FirstOrDefault()
        {
            return _dbSet.FirstOrDefault();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<List<TEntity>> GetListAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<TEntity> Include(string[] properties)
        {
           var queryable = _dbSet.AsQueryable();
            foreach (var property in properties)
                queryable = queryable.Include(property);

            return queryable;
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public async Task InsertRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_insuranceContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Delete(IEnumerable<TEntity> entitiesToDelete)
        {
            if (_insuranceContext.Entry(entitiesToDelete).State == EntityState.Detached)
            {
                _dbSet.AttachRange(entitiesToDelete);
            }
            _dbSet.RemoveRange(entitiesToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _insuranceContext.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
