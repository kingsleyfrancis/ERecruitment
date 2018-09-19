using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ERecruitment.Core.Common;
using ERecruitment.Core.Engine;
using ERecruitment.Data.DataLoaders;

namespace ERecruitment.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        protected override void Seed(DataContext context)
        {
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            IEnumerable<Type> startUpTaskTypes = typeFinder.FindClassesOfType<IMigrationDataLoader>();

            List<IMigrationDataLoader> startUpTasks = startUpTaskTypes
                .Select(startUpTaskType => (IMigrationDataLoader)
                    Activator.CreateInstance(startUpTaskType))
                .ToList();

            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();

            foreach (IMigrationDataLoader startUpTask in startUpTasks)
                startUpTask.Load(context);
        }
    }
}