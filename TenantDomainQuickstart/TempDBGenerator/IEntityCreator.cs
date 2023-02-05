namespace TempDBGenerator
{
    using Glasswall.Domain.Tenancy;

    public interface IEntityCreator
    {
        Tenant CreateTenantEntity(string name, string displayName);
        User CreateUserEntity(string displayName);
        Role CreateRoleEntity(string displayName, string description);
        Group CreateGroupEntity(string name, string displayName);
        Permission CreatePermissionEntity(string displayName, string description);
        Product CreateProductEntity(string name, string displayName);
        GroupUser CreateGroupUserEntity(string displayName, Group group, User user);
        GroupRole CreateGroupRoleEntity(string displayName, Group group, Role role);
        RolePermission CreateRolePermissionEntity(string displayName, Role role, Permission permission);
        TenantUser CreateTenantUserEntity(string displayName, Tenant tenant, User user);
    }
}