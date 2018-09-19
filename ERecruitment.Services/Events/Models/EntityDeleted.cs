
using ERecruitment.Patterns.Infrastructure;

namespace ERecruitment.Services.Events.Models
{
    /// <summary>
    /// A container for passing entities that have been deleted. 
    /// This is not used for entities that are deleted logicaly via a bit column.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityDeleted<T> where T : IEntity
    {
        public EntityDeleted(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }
}
