using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ERecruitment.Core.Common;
using ERecruitment.Core.Engine;
using ERecruitment.Services.Accounts;
using ERecruitment.Services.Answers;
using ERecruitment.Services.Events;
using ERecruitment.Services.Logs;
using ERecruitment.Services.Questions;
using ERecruitment.Services.TestDetails;
using ERecruitment.Services.Tests;
using ERecruitment.Services.Time;

namespace ERecruitment.Services.Registrars
{
    public class ServiceRegistrar : IDependencyRegistrar
    {
        public int Order { get { return 3; } }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //Register event consumers
            List<Type> consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();

            foreach (Type consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        bool isMatch = type.IsGenericType &&
                                       ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }

            builder.RegisterType<EventPublisher>()
                .As<IEventPublisher>()
                .SingleInstance();

            builder
                .RegisterType<SubscriptionService>()
                .As<ISubscriptionService>()
                .SingleInstance();

            builder.RegisterType<LogService>()
                .As<ILogService>()
                .InstancePerRequest();

            builder
                .RegisterType<Clock>()
                .As<IClock>()
                .InstancePerRequest();

            builder.RegisterType<QuestionService>()
                .As<IQuestionService>()
                .InstancePerRequest();

            builder.RegisterType<TestService>()
                .As<ITestService>()
                .InstancePerRequest();

            builder.RegisterType<AccountService>()
                .As<IAccountService>()
                .InstancePerRequest();

            builder.RegisterType<AnswerService>()
                .As<IAnswerService>()
                .InstancePerRequest();

            builder.RegisterType<TestDetailService>()
                .As<ITestDetailService>()
                .InstancePerRequest();
        }
    }
}
