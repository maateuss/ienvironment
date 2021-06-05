using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace iEnvironment.Domain.Enums
{
    public enum UserRole
    {
        [Description("admin")]
        Admin,
        [Description("user")]
        User,
        [Description("broker")]
        Broker,
        [Description("guest")]
        Guest
    }
}

