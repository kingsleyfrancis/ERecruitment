using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using ERecruitment.Core.Engine;
using ERecruitment.Services.Logs;
using ERecruitment.Models.Enums;

namespace ERecruitment
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            InitializeApp();
        }


        private static void InitializeApp()
        {
            //Initialize BestEngine
            IAppEngine appEngine = EngineContext.Initialize(false);

            IContainer container = appEngine.ContainerManager.Container;

            var mvcResolver = new AutofacDependencyResolver(container);

            DependencyResolver.SetResolver(mvcResolver);

            // this override is needed because WebAPI is not using DependencyResolver to build controllers 
            //GlobalConfiguration.Configuration.DependencyResolver = apiResolver;
        }


        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var logServ = EngineContext.Current.Resolve<ILogService>();

            const string generalErrorMsg = "A fatal error occured";
               
            Exception exception = Server.GetLastError();

            logServ.InsertLog(LogLevel.Fatal, generalErrorMsg, exception);
        }
    }
}