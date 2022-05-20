using Insurance.Core.Interfaces;
using Insurance.Infrastructure.EF;
using Insurance.Infrastructure.EF.Extensions;
using Insurance.Infrastructure.Repositories;
using Insurance.Shared.Entities;
using Microsoft.AspNetCore.Http;

#nullable disable

namespace MicroFinanceSystem.Infrastructure.UnitOfWork
{
    public class InsuranceUnitOfWork : IInsuranceUnitOfWork
    {
        private InsuranceContext _insuranceContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public Dictionary<Type, object> Repositories
        {
            get { return _repositories; }
            set { Repositories = value; }
        }

        public InsuranceUnitOfWork(InsuranceContext InsuranceContext, IHttpContextAccessor contextAccessor)
        {
            _insuranceContext = InsuranceContext;
            _contextAccessor = contextAccessor;
        }

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IGenericRepository<T>;
            }

            IGenericRepository<T> repo = new GenericRepository<T>(_insuranceContext);
            Repositories.Add(typeof(T), repo);
            return repo;
        }

        public void Save()
        {
            _insuranceContext.ChangeTracker.TrackEntityDataChanges(_contextAccessor);
            _insuranceContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            _insuranceContext.ChangeTracker.TrackEntityDataChanges(_contextAccessor);
            await _insuranceContext.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _insuranceContext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
