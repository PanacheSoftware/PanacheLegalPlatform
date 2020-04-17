using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Database.Core;
using PanacheSoftware.Http;
using System;
using System.Linq;

namespace PanacheSoftware.Database.Persistance
{
    public class PanacheSoftwareUnitOfWork : IPanacheSoftwareUnitOfWork
    {
        public readonly DbContext _context;
        private readonly IUserProvider _userProvider;

        public PanacheSoftwareUnitOfWork(DbContext context, IUserProvider userProvider)
        {
            _context = context;
            _userProvider = userProvider;
        }

        public int Complete()
        {
            //Is this the best place to ensure that last update always gets set?  Maybe it isn't a good idea to loop through the context here as there could be a lot of changes.
            foreach (var updatedEntityEntry in _context.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified || p.State == EntityState.Added))
            {
                var changedEntity = (PanacheSoftwareEntity)updatedEntityEntry.Entity;
                changedEntity.LastUpdateDate = DateTime.Now;
                changedEntity.LastUpdateBy = Guid.Parse(_userProvider.GetUserId());
                if (updatedEntityEntry.State != EntityState.Added) continue;
                changedEntity.CreatedDate = DateTime.Now;
                changedEntity.CreatedBy = Guid.Parse(_userProvider.GetUserId());
                changedEntity.TenantId = Guid.Parse(_userProvider.GetTenantId());
            }
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
