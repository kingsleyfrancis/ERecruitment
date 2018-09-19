using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ERecruitment.Core.Common;
using ERecruitment.Core.Configurations;

namespace ERecruitment.Core.Engine
{
    /// <summary>
    ///     Provides access to the singleton instance of the Best engine.
    /// </summary>
    public class EngineContext
    {
        #region Initialization Methods

        /// <summary>Initializes a static instance of the Best factory.</summary>
        /// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IAppEngine Initialize(bool forceRecreate)
        {
            if (Singleton<IAppEngine>.Instance == null || forceRecreate)
            {
                var config = ConfigurationManager.GetSection("SiteConfig")
                    as SiteConfig;
                Debug.WriteLine("Constructing engine " + DateTime.Now);
                Singleton<IAppEngine>.Instance = CreateEngineInstance(config);
                Debug.WriteLine("Initializing engine " + DateTime.Now);
                Singleton<IAppEngine>.Instance.Initialize(config);
            }
            return Singleton<IAppEngine>.Instance;
        }

        /// <summary>
        ///     Sets the static engine instance to the supplied engine.
        ///     Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IAppEngine engine)
        {
            Singleton<IAppEngine>.Instance = engine;
        }

        /// <summary>
        ///     Creates a factory instance and adds a http application injecting facility.
        /// </summary>
        /// <returns>A new factory</returns>
        public static IAppEngine CreateEngineInstance(SiteConfig config)
        {
            if (config != null && !string.IsNullOrEmpty(config.EngineType))
            {
                Type engineType = Type.GetType(config.EngineType);
                if (engineType == null)
                    throw new ConfigurationErrorsException("The type '" + engineType +
                                                           "' could not be found. Please check the configuration at /configuration/Best/engine[@engineType] or check for missing assemblies.");
                if (!typeof (IAppEngine).IsAssignableFrom(engineType))
                    throw new ConfigurationErrorsException("The type '" + engineType +
                                                           "' doesn't implement 'Best.Core.IAppEngine' and cannot be configured in /configuration/Best/engine[@engineType] for that purpose.");
                return Activator.CreateInstance(engineType) as IAppEngine;
            }

            return new AppEngine();
        }

        #endregion Initialization Methods

        /// <summary>Gets the singleton Best engine used to access Best services.</summary>
        public static IAppEngine Current
        {
            get
            {
                if (Singleton<IAppEngine>.Instance == null)
                {
                    Initialize(false);
                }
                return Singleton<IAppEngine>.Instance;
            }
        }
    }
}