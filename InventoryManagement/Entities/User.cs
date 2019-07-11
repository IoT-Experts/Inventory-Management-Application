using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InventoryManagement.Enums;
using System.ComponentModel;

namespace InventoryManagement.Entities
{
    public class User
    {
        public string userID { get; set; }
        [DisplayName("User Name")]
        public string userName { get; set; }
        [DisplayName("Password")]
        public string password { get; set; }
        public DateTime creationDate { get; set; }
        public RoleType roleType { get; set; }
        public bool active { get; set; }

    }
}
