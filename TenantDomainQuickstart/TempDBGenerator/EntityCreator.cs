namespace TempDBGenerator
{
    using System;
    using System.Text;
    using Glasswall.Domain.Tenancy;

    public class EntityCreator : IEntityCreator
    {
        public Tenant CreateTenantEntity(string name, string displayName)
        {
            return new Tenant
            {
                Id = Guid.NewGuid(),
                Name = name,
                DisplayName = displayName,
                Active = true,
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                Database = "GW_Domain",
                Shard = "DG-MFB\\SQLEXPRESS_2016",
                Region = "EMEA",
                SignInUser = "admin",
                SignInSecret = "catalog-secret",
                OwnerEmail = $"admin@{name}.com",
                OwnerName = "Admin",
                RowVersion = Encoding.UTF8.GetBytes(displayName)
            };
        }

        public User CreateUserEntity(string displayName)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.MinValue,
                IsDeleted = false,
                LastUpdated = DateTimeOffset.UtcNow,
                TimeZone = "Europe/London",
                Locale = "GB",
                RowVersion = Encoding.UTF8.GetBytes(displayName)
            };
        }

        public Role CreateRoleEntity(string displayName, string description)
        {
            return new Role
            {
                Id = Guid.NewGuid(),
                DisplayName = displayName,
                Description = description,
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                RowVersion = Encoding.UTF8.GetBytes(displayName)
            };
        }

        public Group CreateGroupEntity(string name, string displayName)
        {
            return new Group
            {
                Id = Guid.NewGuid(),
                Name = name,
                DisplayName = displayName,
                Active = true,
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                RowVersion = Encoding.UTF8.GetBytes(displayName)
            };
        }

        public Permission CreatePermissionEntity(string displayName, string description)
        {
            return new Permission
            {
                Id = Guid.NewGuid(),
                DisplayName = displayName,
                Description = description,
                Active = true,
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                EnumKey = 0,
                RowVersion = Encoding.UTF8.GetBytes(displayName)
            };
        }

        public Product CreateProductEntity(string name, string displayName)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                RowVersion = Encoding.UTF8.GetBytes(displayName)
            };
        }

        public GroupUser CreateGroupUserEntity(string displayName, Group group, User user)
        {
            return new GroupUser
            {
                Id = Guid.NewGuid(),
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                RowVersion = Encoding.UTF8.GetBytes(displayName),
                Group = group,
                GroupId = group.Id,
                User = user,
                UserId = user.Id
            };
        }

        public GroupRole CreateGroupRoleEntity(string displayName, Group group, Role role)
        {
            return new GroupRole
            {
                Id = Guid.NewGuid(),
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                RowVersion = Encoding.UTF8.GetBytes(displayName),
                Role = role,
                RoleId = role.Id,
                Group = group,
                GroupId = group.Id
            };
        }

        public RolePermission CreateRolePermissionEntity(string displayName, Role role, Permission permission)
        {
            return new RolePermission
            {
                Id = Guid.NewGuid(),
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                RowVersion = Encoding.UTF8.GetBytes(displayName),
                Permission = permission,
                PermissionId = permission.Id,
                Role = role,
                RoleId = role.Id
            };
        }

        public TenantUser CreateTenantUserEntity(string displayName, Tenant tenant, User user)
        {
            return new TenantUser
            {
                Id = Guid.NewGuid(),
                Active = true,
                IsDeleted = false,
                Created = DateTimeOffset.MinValue,
                LastUpdated = DateTimeOffset.UtcNow,
                RowVersion = Encoding.UTF8.GetBytes(displayName),
                User = user,
                UserId = user.Id,
                Tenant = tenant,
                TenantId = tenant.Id
            };
        }
    }
}