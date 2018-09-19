using System.Data.Entity;
using ERecruitment.Patterns.DataContext;
using ERecruitment.Patterns.Infrastructure;

namespace ERecruitment.Data
{
    public interface IFakeDbContext : IDataContextAsync
    {
        DbSet<T> Set<T>() where T : class;

        void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : class, IObjectState, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new();
    }
}