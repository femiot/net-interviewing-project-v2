using Insurance.Shared.Entities;
using System.Linq.Expressions;

namespace Insurance.Core.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> FirstOrDefaultAsync();
        TEntity? FirstOrDefault();
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>?> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetListAsync();
        IQueryable<TEntity> AsQueryable();
        IQueryable<TEntity> Include(string[] properties);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync();
        Task InsertAsync(TEntity entity);
        Task InsertRangeAsync(List<TEntity> entities);
        void Delete(TEntity entityToDelete);
        void Delete(IEnumerable<TEntity> entitiesToDelete);
        void Update(TEntity entityToUpdate);
    }
}