using Autofac;
using ERecruitment.Core.Common;

namespace ERecruitment.Core.Engine
{
    public interface IDependencyRegistrar
    {
        int Order { get; }
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);
    }
}