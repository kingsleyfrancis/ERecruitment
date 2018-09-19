﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using ERecruitment.Core.Configurations;

namespace ERecruitment.Core.Common
{
    /// <summary>
    ///     Provides information about types in the current web application.
    ///     Optionally this class can look at all assemblies in the bin folder.
    /// </summary>
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        #region Fields

        private bool _binFolderAssembliesLoaded;
        private bool _ensureBinFolderAssembliesLoaded = true;

        #endregion Fields

        #region Ctor

        public WebAppTypeFinder(SiteConfig config)
        {
            _ensureBinFolderAssembliesLoaded = config.DynamicDiscovery;
        }

        #endregion Ctor

        #region Properties

        /// <summary>
        ///     Gets or sets wether assemblies in the bin folder of the web application should be specificly checked for beeing
        ///     loaded on application load.
        ///     This is need in situations where plugins need to be loaded in the AppDomain after the application been reloaded.
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded
        {
            get { return _ensureBinFolderAssembliesLoaded; }
            set { _ensureBinFolderAssembliesLoaded = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Gets a physical disk path of \Bin directory
        /// </summary>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string GetBinDirectory()
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HttpRuntime.BinDirectory;
            }
            //not hosted. For example, run either in unit tests
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public override IList<Assembly> GetAssemblies()
        {
            if (EnsureBinFolderAssembliesLoaded && !_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                string binPath = GetBinDirectory();
                //binPath = _webHelper.MapPath("~/bin");
                LoadMatchingAssemblies(binPath);
            }

            return base.GetAssemblies();
        }

        #endregion Methods
    }
}