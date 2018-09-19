using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using ERecruitment.Core.Caching;
using ERecruitment.Core.Common;
using ERecruitment.Core.Data;
using ERecruitment.Core.Engine;
using ERecruitment.Core.Fakes;
using ERecruitment.Core.IO;
using ERecruitment.Data;
using ERecruitment.Patterns.DataContext;
using ERecruitment.Patterns.Repositories;
using ERecruitment.Patterns.UnitOfWork;

namespace ERecruitment.Common.Registrars
{
    public class CoreRegistrars : IDependencyRegistrar
    {
        public int Order
        {
            get { return 1; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null
                    ? (new HttpContextWrapper(HttpContext.Current) as HttpContextBase)
                    : (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>()
                .As<IWebHelper>()
                .InstancePerLifetimeScope();

            //Unit of Work;
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>().As<IUnitOfWorkAsync>()
                .InstancePerLifetimeScope();

            //Repository
            builder.RegisterGeneric(typeof (Repository<>))
                .As(typeof (IRepository<>))
                .As(typeof (IRepositoryAsync<>))
                .InstancePerLifetimeScope();


            builder.RegisterType<DataContext>().As<IDataContextAsync>()
                .As<IDataContext>().InstancePerLifetimeScope();

            //cache manager
            builder.RegisterType<MemoryCacheManager>()
                .As<ICacheManager>()
                .Named<ICacheManager>("Memory_Cache")
                .SingleInstance();

            builder.RegisterType<PerRequestCacheManager>()
                .As<ICacheManager>()
                .Named<ICacheManager>("Per_Request_Cache")
                .InstancePerLifetimeScope();

            //controllers
            //mvc
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

           
            //Data settings
            builder.RegisterType<DataSettingsManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<FileSystemStorageProvider>()
                .As<IStorageProvider>()
                .InstancePerLifetimeScope();

           
        }
    }
}