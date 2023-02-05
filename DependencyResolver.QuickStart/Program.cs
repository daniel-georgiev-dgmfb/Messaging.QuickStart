using System;
using Glasswall.Providers.Microsoft.DependencyInjection;
using Glasswall.Providers.Logging.Microsoft;
using Glasswall.Providers.Logging.Console;
using Glasswall.Providers.Logging.Debug;
using Glasswall.Kernel.Configuration;
using Glasswall.Common.Configuration;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Providers.DI.Unity;
using Glasswall.Kernel.Logging;

namespace DependencyResolver.QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create an instance of dependency resolver
            IGWLogger<Program> logger;
            var resolver = new MicrosoftDependencyInjection();
            var unityResolver = new UnityDependencyResolver();
            //register with MS resolver
            resolver.UseLoging()
                .AddConsole()
                .AddDebug()
                .RegisterType<IConfiguration, EnvironmentVariableConfiguration>(Lifetime.Singleton);//register configration for Enviroment variable
            //register services
            resolver.RegisterType<IService, Service>(Glasswall.Kernel.DependencyResolver.Lifetime.Transient);
            resolver.RegisterType<IRule, Rule1>(Lifetime.Transient);
            resolver.RegisterType<IRule, Rule2>(Lifetime.Transient);
            resolver.Initialise().GetAwaiter().GetResult();
            logger = resolver.Resolve<IGWLogger<Program>>();
            //register with MS resolver
            unityResolver.UseLoging()
                .AddConsole()
                .AddDebug()
                .RegisterType<IConfiguration, EnvironmentVariableConfiguration>(Lifetime.Singleton);//register configration for Enviroment variable
            //register services
            unityResolver.RegisterType<IService, Service>(Glasswall.Kernel.DependencyResolver.Lifetime.Transient);
            unityResolver.RegisterType<IRule, Rule1>(Lifetime.Transient);
            unityResolver.RegisterType<IRule, Rule2>(Lifetime.Transient);

            // resolver the service from DI
            try
            {
                var serviceFromMSresolver = resolver.Resolve<IService>();
                logger.LogInformation("MS resolver built service.");
            }
            catch(Exception e)
            {
                logger.LogError(e, "MS resolver failed to resolve service.");
            }

            try
            {
                var serviceFromUnity = unityResolver.Resolve<IService>();
                logger.LogInformation("Unity resolver built service.");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unity resolver failed to resolve service.");
            }
            
            Console.ReadLine();
        }
    }
}
