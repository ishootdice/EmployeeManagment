using System;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagment.Security
{ 
    public class ManageAdminRolesAndClaimsRequirement : IAuthorizationRequirement
    {
        public ManageAdminRolesAndClaimsRequirement()
        {
        }

    }
}

