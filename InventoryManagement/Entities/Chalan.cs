using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Entities
{
    public class Chalan : Item
    {
        public string ChalanId { get; set; }
        [DisplayName("Chalan Number")]
        public string ChalanNo { get; set; }
        [DisplayName("Stock Entry Count")]
        public int EntryCount { get; set; }
        [DisplayName("Chalan Date")]
        public DateTime ChalanDate { get; set; }
        [DisplayName("Box Entered")]
        public int EntryBoxCount { get; set; }
        [DisplayName("Extra Quantity Entered")]
        public int EntryExtraCount { get; set; }

        public void CalculateBoxesFromTotalChalan()
        {
            if (EntryCount != 0 && CountPerBox != 0)
            {
                EntryBoxCount = EntryCount / CountPerBox;
                EntryExtraCount = EntryCount % CountPerBox;
            }
        }
    }
}
