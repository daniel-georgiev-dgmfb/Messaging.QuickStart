namespace TempDBGenerator
{
    using Glasswall.Domain.Tenancy;

    public class RoleCreationHandler : IRoleCreationHandler
    {
        private readonly IEntityCreator _entityCreator;
        private readonly Permission _tiViewPermission;
        private readonly Permission _inboundPolicyViewPermission;
        private readonly Permission _inboundPolicyEditPermission;
        private readonly Permission _inboundPolicyDeletePermission;
        private readonly Permission _filePreviewViewPermission;

        private readonly Product _filePreviewProduct;
        private readonly Product _threatIntelligenceProduct;
        private readonly Product _inboundPolicyProduct;


        public RoleCreationHandler(IEntityCreator entityCreator)
        {
            _entityCreator = entityCreator;

            _filePreviewProduct = _entityCreator.CreateProductEntity("FilePreview", "Glasswall File Preview");
            _threatIntelligenceProduct = _entityCreator.CreateProductEntity("ThreatIntelligence", "Threat Intelligence Product");
            _inboundPolicyProduct = _entityCreator.CreateProductEntity("Inbound Policy", "Glasswall Inbound Policy");

            _tiViewPermission = entityCreator.CreatePermissionEntity("View", "Permissions to view the Threat Intelligence");
            _inboundPolicyViewPermission = _entityCreator.CreatePermissionEntity("View", "Permissions to view the Inbound Policy");
            _inboundPolicyEditPermission = _entityCreator.CreatePermissionEntity("Edit", "Permissions to edit the Inbound Policy");
            _inboundPolicyDeletePermission = _entityCreator.CreatePermissionEntity("Delete", "Permissions to delete the Inbound Policy");
            _filePreviewViewPermission = _entityCreator.CreatePermissionEntity("View", "Permissions to view the File Preview Requests");

            _tiViewPermission.Product = _threatIntelligenceProduct;
            _inboundPolicyViewPermission.Product = _inboundPolicyProduct;
            _inboundPolicyDeletePermission.Product = _inboundPolicyProduct;
            _inboundPolicyEditPermission.Product = _inboundPolicyProduct;
            _filePreviewViewPermission.Product = _filePreviewProduct;
        }

        public Role CreateSuperUserRole()
        {
            var superUserRole = _entityCreator.CreateRoleEntity("Super User", "Can do anything on the platform");

            var superUserEditPolicyRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Super User Edit Policy Role Permission",
                superUserRole,
                _inboundPolicyEditPermission);

            var superUserDeletePolicyRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Super User Delete Policy Role Permission",
                superUserRole,
                _inboundPolicyDeletePermission);

            var superUserViewPolicyRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Super User View Policy Role Permission",
                superUserRole,
                _inboundPolicyViewPermission);

            var superUserFilePreviewViewRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Super User File Preview View Role Permission",
                superUserRole,
                _filePreviewViewPermission);

            var superUserTiViewRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Super User Threat Intelligence View Role Permission",
                superUserRole,
                _tiViewPermission);

            superUserRole.RolePermissions.Add(superUserTiViewRolePermission);
            superUserRole.RolePermissions.Add(superUserDeletePolicyRolePermission);
            superUserRole.RolePermissions.Add(superUserEditPolicyRolePermission);
            superUserRole.RolePermissions.Add(superUserViewPolicyRolePermission);
            superUserRole.RolePermissions.Add(superUserFilePreviewViewRolePermission);

            return superUserRole;
        }

        public Role CreateFileReleaserRole()
        {
            var fileReleaserRole = _entityCreator.CreateRoleEntity("File Releaser", "Can view and action File Release Requests");

            var fileReleaserViewRolePermission = _entityCreator.CreateRolePermissionEntity(
                "File Releaser Role View Permission",
                fileReleaserRole,
                _filePreviewViewPermission);

            fileReleaserRole.RolePermissions.Add(fileReleaserViewRolePermission);

            return fileReleaserRole;
        }

        public Role CreateThreatIntelligenceViewerRole()
        {
            var threatIntelligenceViewerRole = _entityCreator.CreateRoleEntity("Threat Intelligence Viewer", "Can view the threat intelligence reports");

            var tiViewerViewRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Threat Intelligence Viewer Role View Permissions", 
                threatIntelligenceViewerRole,
                _tiViewPermission);

            threatIntelligenceViewerRole.RolePermissions.Add(tiViewerViewRolePermission);

            return threatIntelligenceViewerRole;
        }

        public Role CreatePolicyAuthorRole()
        {
            var policyAuthorRole = _entityCreator.CreateRoleEntity("Policy Author", "Can View, Edit, and Delete Inbound SMTP Policies");

            var policyAuthorDeleteRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Policy Author Role Delete Permission",
                policyAuthorRole,
                _inboundPolicyDeletePermission);

            var policyAuthorViewRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Policy Author Role View Permission",
                policyAuthorRole,
                _filePreviewViewPermission);

            var policyAuthorEditRolePermission = _entityCreator.CreateRolePermissionEntity(
                "Policy Author Role Edit Permission",
                policyAuthorRole,
                _inboundPolicyEditPermission);

            policyAuthorRole.RolePermissions.Add(policyAuthorEditRolePermission);
            policyAuthorRole.RolePermissions.Add(policyAuthorDeleteRolePermission);
            policyAuthorRole.RolePermissions.Add(policyAuthorViewRolePermission);

            return policyAuthorRole;
        }
    }
}
