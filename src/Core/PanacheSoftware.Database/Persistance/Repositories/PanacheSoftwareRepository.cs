using Microsoft.EntityFrameworkCore;
using PanacheSoftware.Database.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PanacheSoftware.Database.Repositories
{
    public class PanacheSoftwareRepository<TEntity> : IPanacheSoftwareRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        private readonly DbSet<TEntity> _entities;

        public PanacheSoftwareRepository(DbContext context)
        {
            Context = context;
            _entities = Context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool readOnly)
        { 
            if (readOnly)
                return _entities.AsNoTracking().Where(predicate);

            return _entities.Where(predicate);
        }

        public TEntity Get(int id)
        {
            return _entities.Find(id);
        }

        public TEntity Get(Guid id)
        {
            return _entities.Find(id);
        }

        public IEnumerable<TEntity> GetAll(bool readOnly)
        {
            if (readOnly)
                return _entities.AsNoTracking().ToList();

            return _entities.ToList();
        }

        public void Remove(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _entities.RemoveRange(entities);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate, bool readOnly)
        {
            if (readOnly)
                return _entities.AsNoTracking().SingleOrDefault(predicate);

            return _entities.SingleOrDefault(predicate);
        }

        public bool Any()
        {
            return _entities.Any();
        }
    }
}
