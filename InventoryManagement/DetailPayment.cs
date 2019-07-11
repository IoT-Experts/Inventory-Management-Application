using InventoryManagement.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagement
{
    public partial class DetailPayment : Form
    {
        //public decimal totalPayment { get; set; }

        public DetailPaymentInfo detailPaymentDone { get; set; }

        public DetailPayment()
        {
            InitializeComponent();
        }

        private void btnDoneDetailPayment_Click(object sender, EventArgs e)
        {
            //totalPayment = numTotal.Value;
            detailPaymentDone = new DetailPaymentInfo();
            detailPaymentDone.ThousendCount = (int)numThouSend.Value;
            detailPaymentDone.FiveHundredCount = (int)numFiveHundred.Value;
            detailPaymentDone.OneHundredCount = (int)numHundred.Value;
            detailPaymentDone.FiftyCount = (int)numFifty.Value;
            detailPaymentDone.TwentyCount = (int)numTwenty.Value;
            detailPaymentDone.TenCount = (int)numTen.Value;
            detailPaymentDone.FiveCount = (int)numFive.Value;
            detailPaymentDone.TwoCount = (int)numTwo.Value;
            detailPaymentDone.OneCount = (int)numOne.Value;
            detailPaymentDone.TotalPayment = (int)numTotal.Value;

            this.DialogResult = DialogResult.OK;
        }

        private void amount_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal totalThousend = numThouSend.Value * 1000;
                decimal totalFiveHundred = numFiveHundred.Value * 500;
                decimal totalHundred = numHundred.Value * 100;
                decimal totalFifty = numFifty.Value * 50;
                decimal totalTwenty = numTwenty.Value * 20;
                decimal totalTen = numTen.Value * 10;
                decimal totalFive = numFive.Value * 5;
                decimal totalTwo = numTwo.Value * 2;
                decimal totalOne = numOne.Value * 1;
                numTotal.Value = totalThousend + totalFiveHundred + totalHundred+ totalFifty+ totalTwenty+ totalTen+ totalFive+ totalTwo+ totalOne;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
