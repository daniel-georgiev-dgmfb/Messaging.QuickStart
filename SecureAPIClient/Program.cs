using System;
using System.Threading;
using Glasswall.Common.Configuration;
using Glasswall.Kernel.Configuration;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Security.Validation;
using Glasswall.Kernel.Web;
using Glasswall.Platform.Cryptography.Initialisation;
using Glasswall.Platform.Web.Api.Client;
using Glasswall.Platform.Web.Tokens.Contexts;
using Glasswall.Providers.Logging.Console;
using Glasswall.Providers.Logging.Microsoft;
using Glasswall.Providers.Microsoft.DependencyInjection;
using SecureAPIClient.Validation.Backchannel;

namespace SecureAPIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var resolver = new MicrosoftDependencyInjection();
            
            //Add all services API uses
            Glasswall.Platform.Web.Api.Client.Extensions.HttpClientExtensions.AddApiClient(resolver);

            //Add certificate validation
            CertificateValidationExtensions.AddBackchannelCertificateValidation(resolver);
            //add configuration
            resolver.RegisterType<IConfiguration, EnvironmentVariableConfiguration>(Lifetime.Singleton);
            //add a custom validation rule to validate invalid certificate
            resolver.RegisterType<IBackchannelCertificateValidationRule, CustomBackchannelValidationRule>(Lifetime.Transient);

            //Add logging
            LoggingExtensions.AddLogging(resolver);
            ConsoleLoggingExtensions.AddConsole(resolver);
            
            

            //For test purposes only
            resolver.RegisterType<IService, Service>(Lifetime.Transient);
            
           
            resolver.Initialise();

            //Do work
            var logger = resolver.Resolve<IGWLogger<Program>>();
            var service = resolver.Resolve<IService>();
            //Resource endpoint
            var endpoint = new Endpoint("https://localhost:5001/api/tenant/9629FA3E-0E12-4942-9288-127E9A0B6B31");
            //CAS client credencials
            var credentials = new ClientSecretTokenContext("service", "Glasswall", new Endpoint("https://cas.wotsits.filetrust.io/Connect/Token"));
            //request context
            var request = new RequestContext(endpoint, credentials);

            var response = service.GetResource(request, CancellationToken.None)
                .GetAwaiter()
                .GetResult();
            logger.LogInformation("Response message: {0}", response);
            Console.ReadLine();
        }
    }
}