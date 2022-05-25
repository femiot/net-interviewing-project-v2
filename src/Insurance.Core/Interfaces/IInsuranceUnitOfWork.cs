
using Insurance.Core.Interfaces.Entities;

namespace Insurance.Core.Interfaces
{
    public interface IInsuranceUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task SaveAsync();
        void Save();
    }
}