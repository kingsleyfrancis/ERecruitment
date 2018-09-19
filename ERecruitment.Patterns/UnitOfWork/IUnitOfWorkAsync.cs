using System.Threading;
using System.Threading.Tasks;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.Repositories;

namespace ERecruitment.Patterns.UnitOfWork
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IEntity;
    }
}