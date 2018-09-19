using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using ERecruitment.Core.Common;
using ERecruitment.Core.Configurations;
using ERecruitment.Core.Events;
using ERecruitment.Core.StartUp;

namespace ERecruitment.Core.Engine
{
    public class AppEngine : IAppEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion Fields

        #region Ctor

        /// <summary>
        ///     Creates an instance of the content engine using default settings and configuration.
        /// </summary>
        public AppEngine()
            : this(EventBroker.Instance, new ContainerConfigurer())
        {
        }

        public AppEngine(EventBroker broker, ContainerConfigurer configurer)
        {
            var config = ConfigurationManager.GetSection("SiteConfig") as SiteConfig;
            InitializeContainer(configurer, broker, config);
        }

        #endregion Ctor

        #region Utilities

        private void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            IEnumerable<Type> startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            List<IStartupTask> startUpTasks = startUpTaskTypes
                .Select(startUpTaskType => (IStartupTask)
                    Activator.CreateInstance(startUpTaskType))
                .ToList();
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (IStartupTask startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        private void InitializeContainer(ContainerConfigurer configurer, EventBroker broker,
            SiteConfig config)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, _containerManager, broker, config);
        }

        #endregion Utilities

        #region Methods

        /// <summary>
        ///     Initialize components and plugins in the Best environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(SiteConfig config)
        {
            //startup tasks
            if (!config.IgnoreStartupTasks)
            {
                RunStartupTasks();
            }
        }

        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public T ResolveWithKey<T>(string key) where T : class
        {
            return ContainerManager.Resolve<T>(key);
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion Methods

        #region Properties

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion Properties
    }
}