namespace InventoryManagement
{
    partial class Report
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.orderReportViewerContainer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.itemOrderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.itemOrderBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // orderReportViewerContainer
            // 
            this.orderReportViewerContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "OrderReport";
            reportDataSource1.Value = this.itemOrderBindingSource;
            this.orderReportViewerContainer.LocalReport.DataSources.Add(reportDataSource1);
            this.orderReportViewerContainer.LocalReport.ReportEmbeddedResource = "InventoryManagement.OrderReportViewer.rdlc";
            this.orderReportViewerContainer.Location = new System.Drawing.Point(0, 0);
            this.orderReportViewerContainer.Name = "orderReportViewerContainer";
            this.orderReportViewerContainer.Size = new System.Drawing.Size(1056, 561);
            this.orderReportViewerContainer.TabIndex = 0;
            // 
            // itemOrderBindingSource
            // 
            this.itemOrderBindingSource.DataSource = typeof(InventoryManagement.Entities.ItemOrder);
            // 
            // Report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 561);
            this.Controls.Add(this.orderReportViewerContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Report_Load);
            ((System.ComponentModel.ISupportInitialize)(this.itemOrderBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer orderReportViewerContainer;
        private System.Windows.Forms.BindingSource itemOrderBindingSource;
    }
}