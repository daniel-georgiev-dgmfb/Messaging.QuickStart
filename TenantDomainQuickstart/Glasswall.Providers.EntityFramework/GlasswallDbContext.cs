//  ----------------------------------------------------------------------- 
//   <copyright file="GlasswallDbContext.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Glasswall.Providers.EntityFramework
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using CustomServices;
    using Filters;
    using Kernel.Data;
    using Kernel.Data.Connection;
    using Kernel.Data.ORM;
    using Kernel.Data.Tenancy;
    using Kernel.Reflection.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Logging;

    public class GlasswallDbContext : DbContext, IDbContext
    {
        private readonly string _connectionString;

        public static readonly Microsoft.Extensions.Logging.LoggerFactory _myLoggerFactory = 
            new LoggerFactory(new[] { 
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() 
            });

        /// <summary>
        ///     Constructor with connection string. Used by derived classes which have own connection string factory
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="identityProvider"></param>
        public GlasswallDbContext(IConnectionStringProvider<SqlConnectionStringBuilder> connectionString,
            IDbCustomConfiguration customConfiguration)
        {
            _connectionString = connectionString.GetConnectionString().ConnectionString;
            CustomConfiguration = customConfiguration;
        }

        public IDbCustomConfiguration CustomConfiguration { get; }

        /// <summary>
        ///     Adds the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        T IDbContext.Add<T>(T item)
        {
            if (!typeof(BaseTenantModel).IsAssignableFrom(typeof(T))) return base.Set<T>().Add(item).Entity;

            var queryBuilder = CustomConfiguration.TenantManager();
            var tenantModel = item as BaseTenantModel;

            queryBuilder.AssignTenantId(tenantModel);

            return base.Set<T>().Add(item).Entity;
        }

        /// <summary>
        ///     Removes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        bool IDbContext.Remove<T>(T item)
        {
            return base.Set<T>().Remove(item) != null;
        }

        /// <summary>
        ///     Sets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> IDbContext.Set<T>()
        {
            return base.Set<T>();
        }

        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        ///     The number of state entries written to the underlying database. This can include
        ///     state entries for entities and/or relationships. Relationship state entries are created for
        ///     many-to-many relationships and relationships where there is no foreign key property
        ///     included in the entity class (often referred to as independent associations).
        /// </returns>
        int IDbContext.SaveChanges()
        {
            Database.EnsureCreated();
            AssignTenantIdToModified();
            return base.SaveChanges();
        }

        async Task<int> IDbContext.SaveChangesAsync()
        {
            Database.EnsureCreated();
            AssignTenantIdToModified();
            return await base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseLoggerFactory(_myLoggerFactory);
            }

            optionsBuilder.ReplaceService<IModelCacheKeyFactory, CustomModelCacheKeyFactory>();
        }

        /// <summary>
        ///     This method is called when the model for a derived context has been initialized, but
        ///     before the model has been locked down and used to initialize the context.  The default
        ///     implementation of this method does nothing, but it can be overridden in a derived class
        ///     such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        ///     Typically, this method is called only once when the first instance of a derived context
        ///     is created.  The model for that context is then cached and is for all further instances of
        ///     the context in the app domain.  This caching can be disabled by setting the ModelCaching
        ///     property on the given ModelBuidler, but note that this can seriously degrade performance.
        ///     More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        ///     classes directly. This method cannot be overidden. Override CreateMethod instead
        /// </remarks>
        protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema(this.CustomConfiguration.Schema ?? "dbo");
            CreateModel(modelBuilder);
        }

        /// <summary>
        ///     Called by sealed OnModelCreating to let derived classes provide their own implemenation
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void CreateModel(ModelBuilder modelBuilder)
        {
            //Either resolve models by assembly scanning or use the ones provided in db configuration
            //This runs onces when the models is being created. If the context is used with different schemas model key needs to be provide. TBD
            var models = CustomConfiguration.ModelsFactory != null
                ? CustomConfiguration.ModelsFactory()
                : ReflectionHelper.GetAllTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && typeof(BaseModel).IsAssignableFrom(t));

            //Register models
            foreach (var m in models)
            {
                if (modelBuilder.Model.FindEntityType(m) != null) continue;

                modelBuilder.Model.AddEntityType(m);

                if (typeof(BaseTenantModel).IsAssignableFrom(m))
                {
                    var method = SetGlobalQueryMethod.MakeGenericMethod(m);
                    method.Invoke(this, new object[] { modelBuilder });
                }
            }

            //apply mappings
            if (CustomConfiguration.ModelMappers != null)
            {
                var mappers = CustomConfiguration.ModelMappers();
                var tasks = mappers.Cast<IDbMapper<ModelBuilder>>()
                    .Select(x => x.Configure(modelBuilder, CustomConfiguration));
                Task.WaitAll(tasks.ToArray());
            }

            //seed the db
            foreach (var s in CustomConfiguration.Seeders.OrderBy(x => x.SeedingOrder).Cast<ISeeder<ModelBuilder>>())
                s.Seed(modelBuilder);
        }

        private static readonly MethodInfo SetGlobalQueryMethod = typeof(GlasswallDbContext).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");

        // This method is called for every loaded entity type in OnModelCreating method.
        // Here type is known through generic parameter and we can use EF Core methods.
        private void SetGlobalQuery<T>(ModelBuilder builder) where T : BaseTenantModel
        {
            builder.Entity<T>().HasKey(e => e.Id);
            builder.Entity<T>().HasQueryFilter(e => e.TenantId == CustomConfiguration.TenantManager().ResolveTenant() && !e.IsDeleted);
        }

        //ToDo: to be review. not sure it's the best interaction. What about the vavigation entities
        //TBD
        private void AssignTenantIdToModified()
        {
            var queryBuilder = CustomConfiguration.TenantManager();
            var modified = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).Select(x => x.Entity);
            foreach (var item in modified)
                if (item is BaseTenantModel)
                    queryBuilder.AssignTenantId((BaseTenantModel) item);
        }
    }
}