using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Entities
{
    public class SRDSR
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public SRType Type { get; set; }
        [DisplayName("Phone Number")]
        public string CellNo { get; set; } 
    }

    public class SRDSRDue : SRDSR
    {
        [DisplayName("Due (Taka)")]
        public double Due { get; set; }
    }
}
