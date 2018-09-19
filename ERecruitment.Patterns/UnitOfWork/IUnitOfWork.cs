using System;
using System.Data;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.Repositories;

namespace ERecruitment.Patterns.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();
    }
}