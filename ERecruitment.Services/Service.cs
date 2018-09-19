using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.Repositories;

namespace ERecruitment.Services
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity :
        class, IEntity, new()
    {
        #region Private Fields

        protected readonly IRepositoryAsync<TEntity> Repository;
        //private readonly IEventPublisher _eventPublisher;

        #endregion Private Fields

        #region Constructor

        protected Service(IRepositoryAsync<TEntity> repository)
        {
            Repository = repository;
            //_eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        }

        #endregion Constructor

        public virtual TEntity Find(params object[] keyValues)
        {
            return Repository.Find(keyValues);
        }

        public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
        {
            return Repository.SelectQuery(query, parameters).AsQueryable();
        }

        public virtual void Insert(TEntity entity)
        {
            Repository.Insert(entity);
            //_eventPublisher.EntityInserted(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            IList<TEntity> enumerable = entities as IList<TEntity> ?? entities.ToList();
            Repository.InsertRange(enumerable);
            //_eventPublisher.EntitiesInserted(enumerable);
        }

        public virtual void InsertOrUpdateGraph(TEntity entity)
        {
            //bool shouldUpdate = entity.ObjectState == ObjectState.Modified;
            Repository.InsertOrUpdateGraph(entity);
/*
            if (shouldUpdate)
            {
                _eventPublisher.EntityUpdated(entity);
            }
            else
            {
                _eventPublisher.EntityInserted(entity);
            }*/
        }

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            IList<TEntity> enumerable = entities as IList<TEntity> ?? entities.ToList();
            Repository.InsertGraphRange(enumerable);
            //_eventPublisher.EntitiesInserted(enumerable);
        }

        public virtual void Update(TEntity entity)
        {
            Repository.Update(entity);
            //_eventPublisher.EntityUpdated(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entity = Find(id);

            if (entity != null)
            {
                Delete(entity);
                //_eventPublisher.EntityDeleted(entity);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            Repository.Delete(entity);
            //_eventPublisher.EntityDeleted(entity);
        }

        public IQueryFluent<TEntity> Query()
        {
            return Repository.Query();
        }

        public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return Repository.Query(queryObject);
        }

        public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return Repository.Query(query);
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await Repository.FindAsync(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await Repository.FindAsync(cancellationToken, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }

        //IF 04/08/2014 - Before: return await DeleteAsync(cancellationToken, keyValues);
        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await Repository.DeleteAsync(cancellationToken, keyValues);
        }

        public IQueryable<TEntity> Queryable()
        {
            return Repository.Queryable();
        }
    }
}