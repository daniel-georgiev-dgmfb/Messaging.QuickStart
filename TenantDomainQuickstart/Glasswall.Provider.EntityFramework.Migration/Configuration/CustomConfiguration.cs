using System;
using System.Collections.Generic;
using Glasswall.Kernel.Configuration;

namespace Provider.EntityFramework.Migration.Configuration
{
    public class CustomConfiguration : IConfiguration
    {
        private readonly IDictionary<string, string> _innerConfiguration;
        public CustomConfiguration()
        {
            this._innerConfiguration = new Dictionary<string, string>
            {
                { "ASPNETCORE_CATALOG_CONNECTION_STRING", "Server=localhost\\SQLEXPRESS;Database=TenantDomain7;Trusted_Connection=True;" },
                { "ASPNETCORE_CASKEY", "wotsitscaskey" },
                { "ASPNETCORE_VAULTDNS", "https://gw-certs.vault.azure.net/" },
                { "ASPNETCORE_AUDIENCE", "https://glasswall/portal/policycatalogues.com" },
                { "ASPNETCORE_VAULT_SECRET", "rRMhyiIBKFmLPrOeeEz4BkWpergE6v+iPx/RPKRUn60=" },
                { "ASPNETCORE_ENVIRONMENT", "Development"},
                { "ASPNETCORE_VAULT_CLIENTID", "a7c02408-ca87-4ff1-9d86-b5b8c63db2af"},
                { "LOGLEVEL", "Debug" },
                { "MINLOGLEVEL", "Debug" }
            };
        }
        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T GetValue<T>(string key)
        {
            return (T)Convert.ChangeType(this._innerConfiguration[key], typeof(T));
        }

        public void SetValue<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}