using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Security.Validation;
using Glasswall.Platform.Cryptography.Certificates.Backchannel.Validation;

namespace Provider.EntityFramework.Migration
{
    public class CustomBackchannelValidationRule : BackchannelValidationRule
    {
        private readonly IGWLogger<CustomBackchannelValidationRule> _logger;

        //public CustomBackchannelValidationRule(IGWLogger<CustomBackchannelValidationRule> logger)
        //{
        //    if (logger == null)
        //        throw new ArgumentNullException(nameof(logger));
        //    this._logger = logger;
        //}

        public CustomBackchannelValidationRule()
        {

        }

        protected override bool ValidateInternal(BackchannelCertificateValidationContext context)
        {
            //this._logger.LogInformation("Executing backchannel custom validation rule.");
            context.Validated();
            return true;
        }
    }
}