using System;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagment.Security
{
    public class CustomEmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public CustomEmailConfirmationTokenProviderOptions()
        {
        }
    }
}

