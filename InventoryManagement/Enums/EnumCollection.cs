using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace InventoryManagement.Enums
{
    public enum SavingState
    {
        None=0,
        Success=1,
        Failed=2,
        DuplicateExists=3
    }

    public enum SRType
    {
        SR = 0,
        DSR = 1,
        Individual = 2
    }

    public enum RoleType
    {
        Administrator = 1,
        User = 2
    }

    public enum BillingTerm {
        Daily,
        Monthly,
        Yearly
    }

    public enum PurchesType
    {
        Sold = 1,
        Rental = 2
    }

    //public enum Items
    //{
    //    [Description("Plastic Dispenser")]
    //    Plastic_Dispenser = 1001,
    //    [Description("Hot & Cold Normal")]
    //    Hot_And_Cold_Normal = 1002,
    //    [Description("Hot & Cold Exclusive")]
    //    Hot_And_Cold_Exclusive = 1003,
    //    [Description("Aquiva Drinking Water")]
    //    Aquiva_Drinking_Water = 1004
    //}
}
