using System;
using ERecruitment.Core.Configurations;

namespace ERecruitment.Core.Engine
{
    /// <summary>
    ///     Classes implementing this interface can serve as a portal for the
    ///     various services composing the Best engine. Edit functionality, modules
    ///     and implementations access most Best functionality through this
    ///     interface.
    /// </summary>
    public interface IAppEngine
    {
        ContainerManager ContainerManager { get; }

        /// <summary>
        ///     Initialize components and plugins in the Best environment.
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize(SiteConfig config);

        T Resolve<T>() where T : class;

        T ResolveWithKey<T>(string key) where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}