using System;
using System.Collections.Generic;
using System.Linq;
using ERecruitment.Core.Common;
using ERecruitment.Core.Configurations;
using ERecruitment.Core.Events;

namespace ERecruitment.Core.Engine
{
    public class ContainerConfigurer
    {
        public virtual void Configure(IAppEngine engine, ContainerManager containerManager,
            EventBroker broker, SiteConfig configuration)
        {
            //other dependencies
            containerManager.AddComponentInstance<SiteConfig>(configuration, "Site.configuration");
            containerManager.AddComponentInstance<IAppEngine>(engine, "Site.engine");
            containerManager.AddComponentInstance<ContainerConfigurer>(this, "Site.containerConfigurer");

            //type finder
            containerManager.AddComponent<ITypeFinder, WebAppTypeFinder>("Site.typeFinder");

            //register dependencies provided by other assemblies
            var typeFinder = containerManager.Resolve<ITypeFinder>();
            containerManager.UpdateContainer(x =>
            {
                IEnumerable<Type> drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = new List<IDependencyRegistrar>();
                foreach (Type drType in drTypes)
                    drInstances.Add((IDependencyRegistrar) Activator.CreateInstance(drType));
                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (IDependencyRegistrar dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(x, typeFinder);
            });

            //event broker
            containerManager.AddComponentInstance(broker);
        }
    }
}