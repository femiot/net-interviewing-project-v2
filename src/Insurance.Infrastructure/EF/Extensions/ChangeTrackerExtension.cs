using Insurance.Shared.Constants;
using Insurance.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Insurance.Infrastructure.EF.Extensions
{
    public static class ChangeTrackerExtension
    {
        public static void TrackEntityDataChanges(this ChangeTracker changeTracker, IHttpContextAccessor contextAccessor)
        {
            contextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderConstants.CurrentUserId, out var userId);

            var modified = changeTracker.Entries<BaseEntity>().Where(x => x.State == EntityState.Added
            || x.State == EntityState.Modified);

            foreach (var item in modified)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        item.Entity.DateCreated = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss");
                        item.Entity.CreatedByUserId = userId.ToString();
                        break;
                    case EntityState.Modified:
                        item.Entity.DateModified = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss");
                        item.Entity.LastUpdatedByUserId = userId.ToString();
                        break;
                }

            }
        }
    }
}
