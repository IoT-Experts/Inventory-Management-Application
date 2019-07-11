using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using InventoryManagement.Enums;

namespace InventoryManagement.Utilities
{
    public class CommonUtils
    {
        public static bool IsGuid(string strValue)
        {
            try
            {
                if (string.IsNullOrEmpty(strValue)) return false;

                Guid value = new Guid(strValue);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    //public static class ItemsExtensions
    //{
    //    public static string ToDescriptionString(this Items val)
    //    {
    //        DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
    //        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    //    }
    //}
}