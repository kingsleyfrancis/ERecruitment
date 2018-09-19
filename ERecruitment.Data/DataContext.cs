using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ERecruitment.Core.Common;
using ERecruitment.Models.Accounts;
using ERecruitment.Models.Identities;
using ERecruitment.Patterns.DataContext;
using ERecruitment.Patterns.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERecruitment.Data
{
    public class DataContext :
        IdentityDbContext<Account, UserRole, int, AccountUserLogin, AccountUserRole, AccountUserClaim>,
        IDataContextAsync
    {
        #region Private Fields

        private bool _disposed;

        #endregion Private Fields

        public DataContext()
            //: base("JobateaseProductionConString")
            : base("RecruitConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            //Database.SetInitializer(new JobHubInitializer());
        }

        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.
        /// </exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.
        /// </exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.
        /// </exception>
        /// <seealso cref="DbContext.SaveChanges" />
        /// <returns>The number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            try
            {
                SyncObjectsStatePreCommit();
                int changes = base.SaveChanges();
                SyncObjectsStatePostCommit();

                return changes;
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                IEnumerable<string> errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                string fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                string exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                CommonHelper.LogFatalErrors(ex, exceptionMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
                CommonHelper.LogFatalErrors(ex);
                throw;
            }
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.
        /// </exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.
        /// </exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.
        /// </exception>
        /// <seealso cref="DbContext.SaveChangesAsync" />
        /// <returns>
        ///     A task that represents the asynchronous save operation.  The
        ///     <see cref="Task.Result">Task.Result</see> contains the number of
        ///     objects written to the underlying database.
        /// </returns>
        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                return await SaveChangesAsync(CancellationToken.None);
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                IEnumerable<string> errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                string fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                string exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

#pragma warning disable 4014
                CommonHelper.LogFatalErrors(ex, exceptionMessage);
#pragma warning restore 4014

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
#pragma warning disable 4014
                CommonHelper.LogFatalErrors(ex);
#pragma warning restore 4014
                return 0;
            }
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.
        /// </exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.
        /// </exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.
        /// </exception>
        /// <seealso cref="DbContext.SaveChangesAsync" />
        /// <returns>
        ///     A task that represents the asynchronous save operation.  The
        ///     <see cref="Task.Result">Task.Result</see> contains the number of
        ///     objects written to the underlying database.
        /// </returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                SyncObjectsStatePreCommit();
                int changesAsync = await base.SaveChangesAsync(cancellationToken);
                SyncObjectsStatePostCommit();

                return changesAsync;
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                IEnumerable<string> errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                string fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                string exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

#pragma warning disable 4014
                CommonHelper.LogFatalErrors(ex, exceptionMessage);
#pragma warning restore 4014

                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
            catch (Exception ex)
            {
#pragma warning disable 4014
                CommonHelper.LogFatalErrors(ex);
#pragma warning restore 4014
                return 0;
            }
        }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState
        {
            Entry(entity).State = StateHelper.ConvertState(entity.ObjectState);
        }

        public void SyncObjectsStatePostCommit()
        {
            IEnumerable<DbEntityEntry> entries = ChangeTracker.Entries();
            foreach (DbEntityEntry dbEntityEntry in ChangeTracker.Entries())
            {
                ((IObjectState) dbEntityEntry.Entity).ObjectState =
                    StateHelper.ConvertState(dbEntityEntry.State);
            }
        }

        public static DataContext Create()
        {
            return new DataContext();
        }

        private void SyncObjectsStatePreCommit()
        {
            IEnumerable<DbEntityEntry> entries = ChangeTracker.Entries();
            foreach (DbEntityEntry dbEntityEntry in entries)
            {
                var entity = ((IObjectState) dbEntityEntry.Entity);
                ObjectState orignalState = entity.ObjectState;
                dbEntityEntry.State = StateHelper.ConvertState(orignalState);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //dynamically load all configuration
            //System.Type configType = typeof(LanguageMap);   //any of your configuration classes here
            //var typesToRegister = Assembly.GetAssembly(configType).GetTypes()

            IEnumerable<Type> typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !String.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
                               type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>));

            foreach (Type type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
            //...or do it manually below. For example,
            //modelBuilder.Configurations.Add(new LanguageMap());


            //write sql code in output window during debugging
#if DEBUG
            Database.Log = s => Debug.WriteLine(s);
#endif


            base.OnModelCreating(modelBuilder);
        }


        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // free other managed objects that implement
                    // IDisposable only
                }

                // release any unmanaged objects
                // set object references to null

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}