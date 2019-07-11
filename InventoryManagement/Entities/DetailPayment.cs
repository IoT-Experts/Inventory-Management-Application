using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Entities
{
    /// <summary>
    /// Will hold the srdsr's regular payment history
    /// </summary>
    public class DetailPaymentInfo : SRDSRDue
    {
        public string PaymentId { get; set; }
        public string CompanyId { get; set; }
        [DisplayName("Payment Date")]
        public DateTime PaymentDate { get; set; }
        [DisplayName("One Thousend Count")]
        public int ThousendCount { get; set; }
        [DisplayName("Five Hundred Count")]
        public int FiveHundredCount { get; set; }
        [DisplayName("One Hundred Count")]
        public int OneHundredCount { get; set; }
        [DisplayName("Fifty Count")]
        public int FiftyCount { get; set; }
        [DisplayName("Twenty Count")]
        public int TwentyCount { get; set; }
        [DisplayName("Ten Count")]
        public int TenCount { get; set; }
        [DisplayName("Five Count")]
        public int FiveCount { get; set; }
        [DisplayName("Two Count")]
        public int TwoCount { get; set; }
        [DisplayName("One Count")]
        public int OneCount { get; set; }
        [DisplayName("Total Payment (Taka)")]
        public decimal TotalPayment { get; set; }

        public DetailPaymentInfo()
        {
            ThousendCount = 0;
            FiveHundredCount = 0;
            OneHundredCount = 0;
            FiftyCount = 0;
            TwentyCount = 0;
            TenCount = 0;
            FiveCount = 0;
            TwoCount = 0;
            OneCount = 0;
            TotalPayment = 0;
        }
    }
}
