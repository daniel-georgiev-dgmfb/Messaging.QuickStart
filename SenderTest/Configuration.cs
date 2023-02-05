using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Glasswall.Kernel.Configuration;

namespace SenderTest
{
    internal class Configuration : IConfiguration
    {
        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T GetValue<T>(string key)
        {
            return (T)Environment.GetEnvironmentVariables()[key];
        }

        public void SetValue<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
