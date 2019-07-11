using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Entities
{
    public class Company
    {
        public string companyId { get; set; }
        [DisplayName("Company Name")]
        public string companyName { get; set; }
    }
}
