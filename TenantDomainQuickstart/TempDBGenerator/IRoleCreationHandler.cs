namespace TempDBGenerator
{
    using Glasswall.Domain.Tenancy;

    public interface IRoleCreationHandler
    {
        Role CreateSuperUserRole();
        Role CreateFileReleaserRole();
        Role CreateThreatIntelligenceViewerRole();
        Role CreatePolicyAuthorRole();
    }
}
