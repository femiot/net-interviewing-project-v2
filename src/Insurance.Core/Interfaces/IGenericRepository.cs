using Insurance.Shared.Entities;
using System.Linq.Expressions;

namespace Insurance.Core.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>?> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task InsertAsync(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}