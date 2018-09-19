using System.Collections.Generic;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Services.Events.Models;

namespace ERecruitment.Services.Events
{
    public static class EventPublisherExtensions
    {
        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : IEntity
        {
            eventPublisher.Publish(new EntityInserted<T>(entity));
        }

        public static void EntitiesInserted<T>(this IEventPublisher eventPublisher,
            IList<T> entities) where T : IEntity
        {
            foreach (var entity in entities)
            {
                eventPublisher.Publish(new EntityInserted<T>(entity));
            }
        }

        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : IEntity
        {
            eventPublisher.Publish(new EntityUpdated<T>(entity));
        }

        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : IEntity
        {
            eventPublisher.Publish(new EntityDeleted<T>(entity));
        }
    }
}