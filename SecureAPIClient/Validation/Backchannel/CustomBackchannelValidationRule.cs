using System;
using System.Collections.Generic;
using System.Text;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Security.Validation;
using Glasswall.Platform.Cryptography.Certificates.Backchannel.Validation;
using Glasswall.Providers.Logging.Microsoft;

namespace SecureAPIClient.Validation.Backchannel
{
    public class CustomBackchannelValidationRule : BackchannelValidationRule
    {
        private readonly IGWLogger<CustomBackchannelValidationRule> _logger;
        public CustomBackchannelValidationRule(IGWLogger<CustomBackchannelValidationRule> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            this._logger = logger;
        }

        protected override bool ValidateInternal(BackchannelCertificateValidationContext context)
        {
            this._logger.LogInformation("Executing backchannel custom validation rule.");
            context.Validated();
            return true;
        }
    }
}
