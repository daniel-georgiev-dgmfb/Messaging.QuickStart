using System.ComponentModel;

namespace TempDBGenerator.Seeding
{
    internal enum Permissions
    {
        // Inbound Policy Catalogue Roles
        [Description("Approve Publish of Inbound Policy Catalogue")]
        ApprovePublishInboundPolicyCatalogue = 1,
        [Description("View Inbound Policy Catalogue")]
        ViewInboundPolicyCatalogue,
        [Description("Edit Inbound Policy Catalogue")]
        EditInboundPolicyCatalogue,
        [Description("Request Publish of Inbound Policy Catalogue")]
        RequestPublishInboundPolicyCatalogue,

        // Outbound Policy Catalogue Roles
        [Description("Approve Publish of Outbound Policy Catalogue")]
        ApprovePublishOutboundPolicyCatalogue = 10,
        [Description("View Outbound Policy Catalogue")]
        ViewOutboundPolicyCatalogue,
        [Description("Edit Outbound Policy Catalogue")]
        EditOutboundPolicyCatalogue,
        [Description("Request Publish of Outbound Policy Catalogue")]
        RequestPublishOutboundPolicyCatalogue,

        // File Roles
        [Description("Approve File Release Request")]
        ApproveFileReleaseRequest = 20,
        [Description("Request File Release")]
        RequestFileRelease,
        [Description("Export Retained File")]
        ExportRetainedFile,
        [Description("Delete Retained File")]
        DeleteRetainedFile,

        // Account Administration Roles
        [Description("Account Administration")]
        AccountAdministration = 30,

        // System Configuration
        [Description("View System Configuration")]
        ViewSystemConfiguration = 40,
        [Description("Edit System Configuration")]
        EditSystemConfiguration,

        // Reporting
        [Description("Create Reports")]
        CreateReports = 50
    }
}