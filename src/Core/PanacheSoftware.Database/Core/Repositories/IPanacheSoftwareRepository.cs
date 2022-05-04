using PanacheSoftware.Core.Domain.Core;
using PanacheSoftware.Database.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PanacheSoftware.Database.Core.Repositories
{
    public interface IPanacheSoftwareRepository<TEntity> where TEntity : class
    {
        TEntity Get(Guid id);
        IEnumerable<TEntity> GetAll(bool readOnly);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool readOnly);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate, bool readOnly);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        bool Any();

        Task<PaginatedList<TEntity>> GetPaginatedListAsync(Pagination pagination, int pageSize);
    }
}
