using System;
using System.Collections.Generic;
using Glasswall.Kernel.Data.ORM;

namespace Provider.EntityFramework.Migration
{
    public class DbCustomConfiguration : IDbCustomConfiguration
    {
        public DbCustomConfiguration(Func<IEnumerable<Type>> modelsFactory, Func<IEnumerable<IDbMapper>> mapperFartory, string modelKey)
        {
            this.ModelsFactory = modelsFactory;
            this.Seeders = new List<ISeeder>();
            this.ModelMappers = mapperFartory;
            this.ModelKey = modelKey;
        }
        public ICollection<ISeeder> Seeders { get; }

        public Func<IEnumerable<Type>> ModelsFactory { get; }
        
        public string Schema => throw new NotImplementedException();

        public string ModelKey { get; }

        public Func<IEnumerable<IDbMapper>> ModelMappers { get; private set; }
    }
}