using InventoryManagement.Entities;
using Microsoft.Reporting.WinForms;
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
    public partial class Report : Form
    {
        private string companyName;
        private string srDsrName;
        private string marketName;
        private DateTime orderDate;
        private string paid;
        SRDSRDue previousDue;

        public Report()
        {
            InitializeComponent();
        }

        public Report(string companyName, string srDsrName, string marketName, DateTime orderDate, List<ItemOrder> order, string paid, SRDSRDue previousDue) : this()
        {
            this.companyName = companyName;
            this.srDsrName = srDsrName;
            this.marketName = marketName;
            this.orderDate = orderDate;
            this.paid = paid;
            this.previousDue = previousDue;
            itemOrderBindingSource.DataSource = order;
            UpdateReportParameters();
        }

        private void UpdateReportParameters()
        {
            ReportParameter[] orderParams = new ReportParameter[7];
            orderParams[0] = new ReportParameter("paramCompanyName", this.companyName, false);
            orderParams[1] = new ReportParameter("paramSrDsrName", this.srDsrName, false);
            orderParams[2] = new ReportParameter("paramSrDsrMobileNo", this.previousDue != null ? this.previousDue.CellNo: "N/A", false);
            orderParams[3] = new ReportParameter("paramMarketName", this.marketName, false);
            orderParams[4] = new ReportParameter("paramOrderDate", this.orderDate.ToShortDateString(), false);
            orderParams[5] = new ReportParameter("paramOrderAmountPaid", !string.IsNullOrEmpty(this.paid)? this.paid: "0", false);
            orderParams[6] = new ReportParameter("paramTotalSrDsrDue", this.previousDue != null? this.previousDue.Due.ToString(): "0", false);

            this.orderReportViewerContainer.LocalReport.SetParameters(orderParams);
        }

        private void Report_Load(object sender, EventArgs e)
        {
            this.orderReportViewerContainer.RefreshReport();
        }
    }
}
