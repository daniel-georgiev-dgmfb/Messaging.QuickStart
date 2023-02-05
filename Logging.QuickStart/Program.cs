using System;
using Glasswall.Providers.Microsoft.DependencyInjection;
using Glasswall.Providers.Logging.Microsoft;
using Glasswall.Providers.Logging.Console;
using Glasswall.Providers.Logging.Debug;
using Glasswall.Kernel.Configuration;
using Glasswall.Common.Configuration;
using Glasswall.Kernel.DependencyResolver;

namespace Logging.QuickStart
{
    class Program
    {
        //Prerequisite:
        //Glasswall.Providers.Microsoft.DependencyInjection package
        //Glasswall.Providers.Logging.Console package
        //Glasswall.Providers.Logging.Debug package
        //Glasswall.Common.Configuration package
        //"environmentVariables"
        //"DEBUGLOGLEVEL": "TRACE",
        //"LOGLEVEL": "DEBUG"

    static void Main(string[] args)
        {
            //Create an instance of dependency resolver
            //NOTE: This functionallity does not work with Unity
            var resolver = new MicrosoftDependencyInjection();
            //register logger services.
            //Add condole logger
            //Add debug logger
            resolver
                .UseLoging()
                .AddConsole()
                .AddDebug()
                .RegisterType<IConfiguration, EnvironmentVariableConfiguration>(Lifetime.Singleton);//register configration for Enviroment variable
            //register services
            resolver.RegisterType<IService, Service>(Glasswall.Kernel.DependencyResolver.Lifetime.Transient);
            resolver.Initialise().GetAwaiter().GetResult();
            // resolver the service from DI
            var service = resolver.Resolve<IService>();
            //invoke method that logs
            service.DoWork("Message to log")
                .GetAwaiter()
                .GetResult();
            //Console.WriteLine("Press enter to terminate.");
            Console.ReadLine();
        }
    }
}