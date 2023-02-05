using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Security.SecretManagement;
using Glasswall.Platform.Cryptography.Secrets;
using Glasswall.Platform.Cryptography.Stores.Azure;
using Glasswall.Providers.Logging.Microsoft;
using Glasswall.Providers.Logging.Console;
using Provider.EntityFramework.Migration.Configuration;

namespace Glasswall.FileTrust.API.Dependencies
{
    internal class Registration
    {
        public static void Register(IDependencyResolver resolver)
        {
            resolver.AddLogging()
                .AddConsole();
            
            resolver.RegisterType<ISecretStore, AzureSecretVault>(Lifetime.Transient);
            resolver.RegisterType<ISecretManager, SecretManager>(Lifetime.Transient);
            resolver.RegisterType<Glasswall.Kernel.Configuration.IConfiguration, CustomConfiguration>(Lifetime.Transient);
        }
    }
}