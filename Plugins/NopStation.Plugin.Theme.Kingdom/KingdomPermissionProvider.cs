using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace NopStation.Plugin.Theme.Kingdom;

public class KingdomPermissionProvider : IPermissionProvider
{
    public static readonly PermissionRecord ManageKingdom = new PermissionRecord { Name = "Kingdom theme. Manage NopStation Kingdom theme", SystemName = "ManageNopStationKingdom", Category = "NopStation" };

    public virtual IEnumerable<PermissionRecord> GetPermissions()
    {
        return new[]
        {
            ManageKingdom
        };
    }

    public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
    {
        return new HashSet<(string, PermissionRecord[])>
        {
            (
                NopCustomerDefaults.AdministratorsRoleName,
                new[]
                {
                    ManageKingdom
                }
            )
        };
    }
}
