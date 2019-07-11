using DataAccess;
using InventoryManagement.Entities;
using InventoryManagement.Enums;
using InventoryManagement.Managers;
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
    public partial class Administration : Form
    {
        bool isFromTemplate = false;
        bool isOrderPlaced = false;
        bool isItemReturnedFromOrder = false;
        bool isDamagedItemReturnedFromOrder = false;
        bool isPaymentCalculated = false;
        //bool isChalanFilterIsInAction = false;

        DetailPaymentInfo detailPaymentDone { get; set; }

        public Administration()
        {
            InitializeComponent();
            //LoadCompaniesIntoCompanyCombo();
            DisplayCompanies();
            InitializeTheSrType();
            DisplaySrDsrs();
            //LoadCompaniesIntoCompanyStocCombo();
            DisplayAllMarkets();

            LoadCompaniesIntoCompanyOrderCombo();
            LoadSrDsrIntoSRDSROrderCombo();
            LoadMarketOrderCombo();

            DisplayAllUsers();
        }

        #region Global Operation
        private void Administration_FormClosed(object sender, FormClosedEventArgs e)
        {
            /// open the login window
            this.Owner.Show();
        }

        private void tabAdministrator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabAdministrator.SelectedTab == tabAdministrator.TabPages["tabChalanActivity"]) {
                LoadCompaniesIntoChalanActivityCombo();

                dtChalanActivityToDate.Value = DateTime.Now.Date;
                dtChalanActivityFromDate.Value = DateTime.Now.Date.AddDays(-7);

                btnChalanActivityFilter_Click(null, null);

            }
            else if (tabAdministrator.SelectedTab == tabAdministrator.TabPages["tabSrDsrPaymentHistory"])
            {

                LoadCompaniesIntoSRDSRPaymentHistoryCombo();
                LoadSrDsrIntoSRDSRPaymentHistoryCombo();

                dtDetailPaymentToDate.Value = DateTime.Now.Date;
                dtDetailPaymentFromDate.Value = DateTime.Now.Date.AddDays(-7);

                btnFilterSrDsrPaymentHistory_Click(null, null);
            }
            else if (tabAdministrator.SelectedTab == tabAdministrator.TabPages["tabManageSrDsr"])
            {
                DisplaySrDsrs();
            }
            else if (tabAdministrator.SelectedTab == tabAdministrator.TabPages["tabManageItems"])//your specific tabname
            {
                LoadCompaniesIntoCompanyCombo();
            }
            else if (tabAdministrator.SelectedTab == tabAdministrator.TabPages["tabManageItemStock"])
            {
                LoadCompaniesIntoCompanyStocCombo();
            }
            else if (tabAdministrator.SelectedTab == tabAdministrator.TabPages["tabManageOrder"])
            {
                LoadCompaniesIntoCompanyOrderCombo();
                LoadSrDsrIntoSRDSROrderCombo();
                LoadMarketOrderCombo();
            }
        }
        #endregion

        #region Order Management

        private void comboOrderCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOrderId.Text = string.Empty;
            DisplayOrderTemplate();
        }

        private void comboOrderSRDSR_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOrderId.Text = string.Empty;
            DisplayOrderTemplate();
        }

        private void comboOrderMarket_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOrderId.Text = string.Empty;
            DisplayOrderTemplate();
        }

        private void btnViewOrderTemplate_Click(object sender, EventArgs e)
        {
            txtOrderId.Text = string.Empty;
            DisplayOrderTemplate();
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            txtOrderId.Text = string.Empty;
            /// get the order info of the selected date based on companyid, srid, marketid
            DisplayAllOrders();

        }

        private void dataGridViewOrder_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewOrder.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridViewOrder.SelectedRows[0];
                /// update the order placement info if existing information found.
                txtOrderId.Text = row.Cells["OrderId"].Value.ToString();
            }
        }

        private void dataGridViewOrder_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //DataGridViewCell currentCell = dataGridViewOrder[e.ColumnIndex, e.RowIndex];
            if (dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("OrderCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("ReturnCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("DamageCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("OrderBoxCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("OrderExtraCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("ReturnBoxCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("ReturnExtraCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("DamageBoxCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("DamageExtraCount"))
            {
                int tempValue;
                if (e.FormattedValue == null || !int.TryParse(e.FormattedValue.ToString(), out tempValue))
                {
                    dataGridViewOrder.Rows[e.RowIndex].ErrorText = "value must be numeric";
                    e.Cancel = true;
                }
            }
        }

        private void dataGridViewOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.   
            dataGridViewOrder.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        private void dataGridViewOrder_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("OrderBoxCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("OrderExtraCount"))
            {
                isOrderPlaced = true;
                isItemReturnedFromOrder = false;
                int itemsPerBox = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CountPerBox"].Value;
                dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value = ((int)dataGridViewOrder.Rows[e.RowIndex].Cells["OrderBoxCount"].Value * itemsPerBox) + (int)dataGridViewOrder.Rows[e.RowIndex].Cells["OrderExtraCount"].Value;
            }
            else if (dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("OrderCount"))
            {
                int currentStock = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CurrentStockTotal"].Value;
                int orderCount = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value;

                int itemsPerBox = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CountPerBox"].Value;
                double itemsPrice = (double)dataGridViewOrder.Rows[e.RowIndex].Cells["Price"].Value;

                if (orderCount > currentStock)
                {
                    dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value = dataGridViewOrder.Rows[e.RowIndex].Cells["CurrentStockTotal"].Value;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["OrderBoxCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value / itemsPerBox;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["OrderExtraCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value % itemsPerBox;
                }
                else if (orderCount <= 0)
                {
                    dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value = 0;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["OrderBoxCount"].Value = 0;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["OrderExtraCount"].Value = 0;
                }

                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value = dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value;
                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsBoxCount"].Value = dataGridViewOrder.Rows[e.RowIndex].Cells["OrderBoxCount"].Value;
                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsExtraCount"].Value = dataGridViewOrder.Rows[e.RowIndex].Cells["OrderExtraCount"].Value;

                dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value = 0;
                dataGridViewOrder.Rows[e.RowIndex].Cells["TotalPrice"].Value = Math.Round((int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value * itemsPrice, 2);
                //dataGridViewOrder.Rows[e.RowIndex].Cells["CurrentStockTotal"].Value = (currentStock - (int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value) + (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value;
            }
            else if (dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("ReturnBoxCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("ReturnExtraCount"))
            {
                isItemReturnedFromOrder = true;
                isOrderPlaced = false;
                /// mark the row as it has return item
                dataGridViewOrder.Rows[e.RowIndex].Cells["IsItemReturned"].Value = true;

                int itemsPerBox = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CountPerBox"].Value;
                dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value = ((int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnBoxCount"].Value * itemsPerBox) + (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnExtraCount"].Value;
            }
            else if (dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("ReturnCount"))
            {
                int orderCount = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value;
                int returnCount = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value;
                int damageCount = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value;

                int itemsPerBox = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CountPerBox"].Value;
                //double itemsPrice = (double)dataGridViewOrder.Rows[e.RowIndex].Cells["Price"].Value;

                if (returnCount > Math.Abs(orderCount - damageCount))
                {
                    dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value = Math.Abs(orderCount - damageCount);
                    dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnBoxCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value / itemsPerBox;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnExtraCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value % itemsPerBox;
                }
                else if (returnCount <= 0)
                {
                    dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value = 0;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnBoxCount"].Value = 0;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnExtraCount"].Value = 0;
                }

                CalculateSells(orderCount, itemsPerBox, e);

                //totalReturnedPrice += ((int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value * itemsPrice);

                //int sellsCount = orderCount - (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value;
                //int sellsCount = orderCount - ((int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value + (int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value);
                //if (sellsCount > 0)
                //{
                //    dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value = sellsCount;

                //    dataGridViewOrder.Rows[e.RowIndex].Cells["SellsBoxCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value / itemsPerBox;
                //    dataGridViewOrder.Rows[e.RowIndex].Cells["SellsExtraCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value % itemsPerBox;
                //}
                //else
                //{
                //    dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value = 0;
                //    dataGridViewOrder.Rows[e.RowIndex].Cells["SellsBoxCount"].Value = 0;
                //    dataGridViewOrder.Rows[e.RowIndex].Cells["SellsExtraCount"].Value = 0;
                //}

                //dataGridViewOrder.Rows[e.RowIndex].Cells["TotalPrice"].Value = Math.Round((int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value * itemsPrice, 2);
                //dataGridViewOrder.Rows[e.RowIndex].Cells["CurrentStockTotal"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CurrentStockTotal"].Value + (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value;
            }
            else if (dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("DamageBoxCount") || dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("DamageExtraCount"))
            {
                isDamagedItemReturnedFromOrder = true;
                isOrderPlaced = false;
                /// mark the row as it has return item as damaged
                dataGridViewOrder.Rows[e.RowIndex].Cells["IsDamagedItemReturned"].Value = true;

                int itemsPerBox = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CountPerBox"].Value;
                dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value = ((int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageBoxCount"].Value * itemsPerBox) + (int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageExtraCount"].Value;
            }
            else if (dataGridViewOrder.Columns[e.ColumnIndex].Name.Equals("DamageCount"))
            {
                int orderCount = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["OrderCount"].Value;
                int returnCount = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value;
                int damageCount = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value;

                int itemsPerBox = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["CountPerBox"].Value;

                if (damageCount > Math.Abs(orderCount - returnCount))
                {
                    dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value = Math.Abs(orderCount - returnCount);
                    dataGridViewOrder.Rows[e.RowIndex].Cells["DamageBoxCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value / itemsPerBox;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["DamageExtraCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value % itemsPerBox;
                }
                else if (damageCount <= 0)
                {
                    dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value = 0;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["DamageBoxCount"].Value = 0;
                    dataGridViewOrder.Rows[e.RowIndex].Cells["DamageExtraCount"].Value = 0;
                }

                CalculateSells(orderCount, itemsPerBox, e);
            }

        }

        /// <summary>
        /// Will calculate the total sells
        /// </summary>
        /// <param name="orderCount"></param>
        /// <param name="itemsPerBox"></param>
        /// <param name="e"></param>
        private void CalculateSells(int orderCount, int itemsPerBox, DataGridViewCellEventArgs e)
        {
            double itemsPrice = (double)dataGridViewOrder.Rows[e.RowIndex].Cells["Price"].Value;
            int sellsCount = orderCount - ((int)dataGridViewOrder.Rows[e.RowIndex].Cells["ReturnCount"].Value + (int)dataGridViewOrder.Rows[e.RowIndex].Cells["DamageCount"].Value);
            if (sellsCount > 0)
            {
                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value = sellsCount;

                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsBoxCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value / itemsPerBox;
                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsExtraCount"].Value = (int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value % itemsPerBox;
            }
            else
            {
                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value = 0;
                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsBoxCount"].Value = 0;
                dataGridViewOrder.Rows[e.RowIndex].Cells["SellsExtraCount"].Value = 0;
            }

            dataGridViewOrder.Rows[e.RowIndex].Cells["TotalPrice"].Value = Math.Round((int)dataGridViewOrder.Rows[e.RowIndex].Cells["SellsCount"].Value * itemsPrice, 2);
        }

        private void btnAddDetailPayment_Click(object sender, EventArgs e)
        {
            /// show modal window to add detail payment and return back the result
            DetailPayment detailPayment = new DetailPayment();
            DialogResult response = detailPayment.ShowDialog(this);

            if (response.Equals(DialogResult.OK)) {
                this.detailPaymentDone = detailPayment.detailPaymentDone;
                numBillPaid.Value = this.detailPaymentDone != null ? this.detailPaymentDone.TotalPayment : 0;
            }
        }

        private void btnSaveOrderDetails_Click(object sender, EventArgs e)
        {
            if (comboOrderCompany.SelectedValue != null && comboOrderSRDSR.SelectedValue != null && comboOrderMarket.SelectedValue != null && dtOrderDate.Value != null)
            {
                //if (!isPaymentCalculated) {
                //    btnCalculatePayment_Click(null, null);
                //}

                SRDSRDue calculatedDue = new SRDSRDue();
                calculatedDue.Id = comboOrderSRDSR.SelectedValue.ToString();
                calculatedDue.Due = NullHandler.GetDouble(txtBillDue.Text.Trim());

                if (detailPaymentDone == null) {
                    detailPaymentDone = new DetailPaymentInfo();
                }

                /// save/update the order information
                SavingState saveState = ItemManager.Instance.SaveItemsOrder(dataGridViewOrder, comboOrderCompany.SelectedValue.ToString(), comboOrderSRDSR.SelectedValue.ToString(), comboOrderMarket.SelectedValue.ToString(), dtOrderDate.Value, calculatedDue, isFromTemplate, isItemReturnedFromOrder, isDamagedItemReturnedFromOrder);

                isOrderPlaced = false;
                isItemReturnedFromOrder = false;
                isDamagedItemReturnedFromOrder = false;

                if (saveState.Equals(SavingState.Success))
                {
                    /// if any payment happen actually
                    if (numBillPaid.Value > 0)
                    {
                        detailPaymentDone.Id = comboOrderSRDSR.SelectedValue.ToString();
                        detailPaymentDone.CompanyId = comboOrderCompany.SelectedValue.ToString();
                        detailPaymentDone.PaymentDate = dtOrderDate.Value;
                        detailPaymentDone.TotalPayment = numBillPaid.Value;

                        saveState = DetailPaymentManager.Instance.SaveSrPaymentDetails(detailPaymentDone);
                        detailPaymentDone = null;
                    }

                    DialogResult dialogResult = MessageBox.Show("Would you like to print an order report?", "Report Print", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        btnPrintOrder_Click(null, null);
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //do something else
                    }
                    //DispalyAllOrders();
                    MessageBox.Show("Order has been updated.");
                }
                else if (saveState.Equals(SavingState.DuplicateExists))
                {
                    MessageBox.Show("Order already exist or no order placed yet. Please review the item.");
                }
                else if (saveState.Equals(SavingState.Failed))
                {
                    MessageBox.Show("Failed to update order information.");
                }
            }
            else
            {
                MessageBox.Show("Save Failed. Please select a company, sr/dsr, market first");
            }

            DisplayAllOrders();
            btnSaveOrderDetails.Enabled = false;
            numBillPaid.Enabled = false;
            btnAddDetailPayment.Enabled = false;
        }
        
        private void btnRemoveOrder_Click(object sender, EventArgs e)
        {
            if (!isFromTemplate && !string.IsNullOrEmpty(txtOrderId.Text.Trim()))
            {
                int totalOrderCount = 0;
                int totalSellsCount = 0;
                foreach (DataGridViewRow row in dataGridViewOrder.Rows)
                {
                    totalOrderCount += NullHandler.GetInt32(row.Cells["OrderCount"].Value);
                    totalSellsCount += NullHandler.GetInt32(row.Cells["SellsCount"].Value);
                }

                /// remove the order information from orders based on selected orderId
                ItemManager.Instance.DeleteItemsOrder(txtOrderId.Text.Trim());
                txtOrderId.Text = string.Empty;
                DisplayAllOrders();
            }
            else
            {
                MessageBox.Show("Please select an order or this order has not been placed yet to be reset.");
            }
        }

        private void btnResetSrDsrOrderOfSelectedDate_Click(object sender, EventArgs e)
        {
            if (comboOrderCompany.SelectedValue != null && comboOrderSRDSR.SelectedValue != null && comboOrderMarket.SelectedValue != null && dtOrderDate.Value != null)
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the SR/DSR Order?", "Reset SR/DSR Order", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    /// remove the order information from orders based on selected orderId
                    SavingState deleteState = ItemManager.Instance.DeleteSrDSrOrder(comboOrderCompany.SelectedValue.ToString(), comboOrderSRDSR.SelectedValue.ToString(), comboOrderMarket.SelectedValue.ToString(), dtOrderDate.Value);
                    if (deleteState.Equals(SavingState.Success))
                    {
                        txtOrderId.Text = string.Empty;
                        DisplayOrderTemplate();
                    }
                    else
                    {
                        MessageBox.Show("Remove operation of SR/DSR Order failed or No order placed yet.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            else
            {
                MessageBox.Show("Please select an SR/DSR, company, market and order date properly to remove the order.");
            }
        }

        private void btnPrintOrder_Click(object sender, EventArgs e)
        {
            if (comboOrderCompany.SelectedValue != null && comboOrderSRDSR.SelectedValue != null && comboOrderMarket.SelectedValue != null && dtOrderDate.Value != null)
            {
                List<ItemOrder> order = ItemManager.Instance.GetAllOrders(comboOrderCompany.SelectedValue.ToString(), comboOrderSRDSR.SelectedValue.ToString(), comboOrderMarket.SelectedValue.ToString(), dtOrderDate.Value);
                SRDSRDue srDsrInfoWithDue = SRDSRManager.Instance.GetSrDsrInfoWithDues(comboOrderSRDSR.SelectedValue.ToString());
                /// print the current order
                Report orderReportModal = new Report(comboOrderCompany.Text, comboOrderSRDSR.Text, comboOrderMarket.Text, dtOrderDate.Value, order, numBillPaid.Value.ToString(), srDsrInfoWithDue);
                DialogResult result = orderReportModal.ShowDialog(this);
            }
        }

        private void btnCalculatePayment_Click(object sender, EventArgs e)
        {
            if (comboOrderCompany.SelectedValue != null && comboOrderSRDSR.SelectedValue != null && comboOrderMarket.SelectedValue != null && dtOrderDate.Value != null)
            {
                double totalOrderBill = 0;
                double totalReturnedPrice = 0;
                foreach (DataGridViewRow row in dataGridViewOrder.Rows)
                {
                    totalOrderBill += NullHandler.GetDouble(row.Cells["TotalPrice"].Value);

                    if (NullHandler.GetBoolean(row.Cells["IsItemReturned"].Value))
                    {
                        totalReturnedPrice += (NullHandler.GetInt32(row.Cells["ReturnCount"].Value) * NullHandler.GetDouble(row.Cells["Price"].Value));
                    }

                    if (NullHandler.GetBoolean(row.Cells["IsDamagedItemReturned"].Value))
                    {
                        totalReturnedPrice += (NullHandler.GetInt32(row.Cells["DamageCount"].Value) * NullHandler.GetDouble(row.Cells["Price"].Value));
                    }
                }

                txtTotalOrderBill.Text = Math.Round(totalOrderBill, 2).ToString();

                if (isFromTemplate)
                {
                    numBillPaid.Value = 0;
                    numBillPaid_ValueChanged(null, null);
                }
                else {
                    //numBillPaid.Value = (decimal)totalReturnedPrice;
                    txtItemPriceDeduction.Text = Math.Round(totalReturnedPrice, 2).ToString();
                }

                if (comboOrderCompany.SelectedValue != null && comboOrderSRDSR.SelectedValue != null && dtOrderDate.Value != null) {

                    txtBillPaidOnDate.Text = DetailPaymentManager.Instance.GetTotalPaymentsOf(comboOrderSRDSR.SelectedValue.ToString(), comboOrderCompany.SelectedValue.ToString(), dtOrderDate.Value).ToString();
                }

                isPaymentCalculated = true;
                btnSaveOrderDetails.Enabled = true;
                numBillPaid.Enabled = true;
                btnAddDetailPayment.Enabled = true;
            }
        }

        private void txtItemPriceDeduction_TextChanged(object sender, EventArgs e)
        {
            numBillPaid_ValueChanged(null, null);
        }

        private void numBillPaid_ValueChanged(object sender, EventArgs e)
        {
            SRDSRDue previousDue = null;
            double currentDue = NullHandler.GetDouble(txtTotalOrderBill.Text.Trim()) - ((double)numBillPaid.Value + NullHandler.GetDouble(txtItemPriceDeduction.Text.Trim()));

            if (comboOrderSRDSR.SelectedValue != null)
            {
                previousDue = SRDSRManager.Instance.GetSrDsrDue(comboOrderSRDSR.SelectedValue.ToString());
            }

            if (isFromTemplate)
            {
                /// add any previous dues with the currentDue(just took the last days due)
                /// get the previous days due from db
                if (comboOrderSRDSR.SelectedValue != null)
                {
                    if (previousDue != null)
                        currentDue += previousDue.Due;
                }
                txtBillDue.Text = Math.Round(currentDue, 2).ToString();
            }
            else
            {
                if (previousDue != null)
                {
                    currentDue = previousDue.Due - ((double)numBillPaid.Value + NullHandler.GetDouble(txtItemPriceDeduction.Text.Trim()));
                }

                txtBillDue.Text = Math.Round(currentDue, 2).ToString();
            }
        }

        private void LoadCompaniesIntoCompanyOrderCombo()
        {
            comboOrderCompany.ValueMember = "companyId";
            comboOrderCompany.DisplayMember = "companyName";
            comboOrderCompany.DataSource = CompanyManager.Instance.GetAllCompany();
        }

        private void LoadSrDsrIntoSRDSROrderCombo()
        {
            comboOrderSRDSR.ValueMember = "Id";
            comboOrderSRDSR.DisplayMember = "Name";
            comboOrderSRDSR.DataSource = SRDSRManager.Instance.GetAllSrDsr();
        }

        private void LoadMarketOrderCombo()
        {
            comboOrderMarket.ValueMember = "Id";
            comboOrderMarket.DisplayMember = "Name";
            comboOrderMarket.DataSource = MarketManager.Instance.GetAllMarket();
        }

        private void DisplayOrderTemplate()
        {
            if (comboOrderCompany.SelectedValue != null)
            {
                dataGridViewOrder.AutoGenerateColumns = true;
                dataGridViewOrder.DataSource = ItemManager.Instance.GetOrderTemplate(comboOrderCompany.SelectedValue.ToString());
                dataGridViewOrder.AutoGenerateColumns = false;

                dataGridViewOrder.Columns["ItemName"].DisplayIndex = 0;
                dataGridViewOrder.Columns["ItemName"].ReadOnly = true;
                //dataGridViewOrder.Columns["CurrentStockTotal"].DisplayIndex = 1;
                //dataGridViewOrder.Columns["CurrentStockTotal"].ReadOnly = true;
                dataGridViewOrder.Columns["CurrentStockTotal"].Visible = false;
                dataGridViewOrder.Columns["CurrentDamagedStockTotal"].Visible = false;

                dataGridViewOrder.Columns["DamagedStockBoxCount"].Visible = false;
                dataGridViewOrder.Columns["DamagedStockNotInBoxCount"].Visible = false;

                dataGridViewOrder.Columns["StockBoxCount"].DisplayIndex = 1;
                dataGridViewOrder.Columns["StockBoxCount"].ReadOnly = true;

                dataGridViewOrder.Columns["StockNotInBoxCount"].DisplayIndex = 2;
                dataGridViewOrder.Columns["StockNotInBoxCount"].ReadOnly = true;

                dataGridViewOrder.Columns["CountPerBox"].DisplayIndex = 3;
                dataGridViewOrder.Columns["CountPerBox"].ReadOnly = true;

                dataGridViewOrder.Columns["OrderCount"].Visible = false;
                dataGridViewOrder.Columns["OrderBoxCount"].DisplayIndex = 4;
                dataGridViewOrder.Columns["OrderBoxCount"].ReadOnly = false;
                dataGridViewOrder.Columns["OrderExtraCount"].DisplayIndex = 5;
                dataGridViewOrder.Columns["OrderExtraCount"].ReadOnly = false;


                dataGridViewOrder.Columns["ReturnCount"].Visible = false;
                dataGridViewOrder.Columns["ReturnBoxCount"].DisplayIndex = 6;
                dataGridViewOrder.Columns["ReturnBoxCount"].ReadOnly = true;
                dataGridViewOrder.Columns["ReturnExtraCount"].DisplayIndex = 7;
                dataGridViewOrder.Columns["ReturnExtraCount"].ReadOnly = true;

                dataGridViewOrder.Columns["DamageCount"].Visible = false;
                dataGridViewOrder.Columns["DamageBoxCount"].DisplayIndex = 8;
                dataGridViewOrder.Columns["DamageBoxCount"].ReadOnly = true;
                dataGridViewOrder.Columns["DamageExtraCount"].DisplayIndex = 9;
                dataGridViewOrder.Columns["DamageExtraCount"].ReadOnly = true;


                dataGridViewOrder.Columns["SellsCount"].Visible = false;
                dataGridViewOrder.Columns["SellsBoxCount"].DisplayIndex = 10;
                dataGridViewOrder.Columns["SellsBoxCount"].ReadOnly = true;
                dataGridViewOrder.Columns["SellsExtraCount"].DisplayIndex = 11;
                dataGridViewOrder.Columns["SellsExtraCount"].ReadOnly = true;

                dataGridViewOrder.Columns["Price"].DisplayIndex = 12;
                dataGridViewOrder.Columns["Price"].ReadOnly = true;
                dataGridViewOrder.Columns["TotalPrice"].DisplayIndex = 13;
                dataGridViewOrder.Columns["TotalPrice"].ReadOnly = true;

                dataGridViewOrder.Columns["OrderId"].Visible = false;
                dataGridViewOrder.Columns["SrId"].Visible = false;
                dataGridViewOrder.Columns["MarketId"].Visible = false;
                dataGridViewOrder.Columns["OrderDate"].Visible = false;
                dataGridViewOrder.Columns["StockId"].Visible = false;
                dataGridViewOrder.Columns["TotalQuantityToAddToTheCurrentStock"].Visible = false;
                dataGridViewOrder.Columns["CompanyId"].Visible = false;
                dataGridViewOrder.Columns["ItemId"].Visible = false;

                dataGridViewOrder.Columns["ChalanNo"].Visible = false;
                dataGridViewOrder.Columns["StockEntryDate"].Visible = false;

                dataGridViewOrder.Columns["IsItemReturned"].Visible = false;

                dataGridViewOrder.Columns["IsDamagedItemReturned"].Visible = false;

                isFromTemplate = true;
            }

            txtTotalOrderBill.Text = txtItemPriceDeduction.Text = txtBillPaidOnDate.Text = txtBillDue.Text = 0.ToString();
            numBillPaid.Value = 0;
            isPaymentCalculated = false;

            isOrderPlaced = false;
            isItemReturnedFromOrder = false;

            btnSaveOrderDetails.Enabled = false;
            numBillPaid.Enabled = false;
            btnAddDetailPayment.Enabled = false;

            if (comboOrderSRDSR.SelectedValue != null)
            {
                SRDSRDue previousDue = SRDSRManager.Instance.GetSrDsrDue(comboOrderSRDSR.SelectedValue.ToString());
                if (previousDue != null)
                    txtBillDue.Text = previousDue.Due.ToString();
                else
                    txtBillDue.Text = 0.ToString();
            }
        }

        private void DisplayAllOrders()
        {
            if (comboOrderCompany.SelectedValue != null && comboOrderSRDSR.SelectedValue != null && comboOrderMarket.SelectedValue != null && dtOrderDate.Value != null)
            {
                dataGridViewOrder.AutoGenerateColumns = true;
                dataGridViewOrder.DataSource = ItemManager.Instance.GetAllOrders(comboOrderCompany.SelectedValue.ToString(), comboOrderSRDSR.SelectedValue.ToString(), comboOrderMarket.SelectedValue.ToString(), dtOrderDate.Value);
                dataGridViewOrder.AutoGenerateColumns = false;

                dataGridViewOrder.Columns["ItemName"].DisplayIndex = 0;
                dataGridViewOrder.Columns["ItemName"].ReadOnly = true;
                //dataGridViewOrder.Columns["CurrentStockTotal"].DisplayIndex = 1;
                //dataGridViewOrder.Columns["CurrentStockTotal"].ReadOnly = true;
                dataGridViewOrder.Columns["CurrentStockTotal"].Visible = false;
                dataGridViewOrder.Columns["CurrentDamagedStockTotal"].Visible = false;

                dataGridViewOrder.Columns["DamagedStockBoxCount"].Visible = false;
                dataGridViewOrder.Columns["DamagedStockNotInBoxCount"].Visible = false;

                dataGridViewOrder.Columns["StockBoxCount"].DisplayIndex = 1;
                dataGridViewOrder.Columns["StockBoxCount"].ReadOnly = true;

                dataGridViewOrder.Columns["StockNotInBoxCount"].DisplayIndex = 2;
                dataGridViewOrder.Columns["StockNotInBoxCount"].ReadOnly = true;

                dataGridViewOrder.Columns["CountPerBox"].DisplayIndex = 3;
                dataGridViewOrder.Columns["CountPerBox"].ReadOnly = true;

                dataGridViewOrder.Columns["OrderCount"].Visible = false;
                dataGridViewOrder.Columns["OrderBoxCount"].DisplayIndex = 4;
                dataGridViewOrder.Columns["OrderBoxCount"].ReadOnly = true;
                dataGridViewOrder.Columns["OrderExtraCount"].DisplayIndex = 5;
                dataGridViewOrder.Columns["OrderExtraCount"].ReadOnly = true;


                dataGridViewOrder.Columns["ReturnCount"].Visible = false;
                dataGridViewOrder.Columns["ReturnBoxCount"].DisplayIndex = 6;
                dataGridViewOrder.Columns["ReturnBoxCount"].ReadOnly = false;
                dataGridViewOrder.Columns["ReturnExtraCount"].DisplayIndex = 7;
                dataGridViewOrder.Columns["ReturnExtraCount"].ReadOnly = false;

                dataGridViewOrder.Columns["DamageCount"].Visible = false;
                dataGridViewOrder.Columns["DamageBoxCount"].DisplayIndex = 8;
                dataGridViewOrder.Columns["DamageBoxCount"].ReadOnly = false;
                dataGridViewOrder.Columns["DamageExtraCount"].DisplayIndex = 9;
                dataGridViewOrder.Columns["DamageExtraCount"].ReadOnly = false;

                dataGridViewOrder.Columns["SellsCount"].Visible = false;
                dataGridViewOrder.Columns["SellsBoxCount"].DisplayIndex = 10;
                dataGridViewOrder.Columns["SellsBoxCount"].ReadOnly = true;
                dataGridViewOrder.Columns["SellsExtraCount"].DisplayIndex = 11;
                dataGridViewOrder.Columns["SellsExtraCount"].ReadOnly = true;

                dataGridViewOrder.Columns["Price"].DisplayIndex = 12;
                dataGridViewOrder.Columns["Price"].ReadOnly = true;
                dataGridViewOrder.Columns["TotalPrice"].DisplayIndex = 13;
                dataGridViewOrder.Columns["TotalPrice"].ReadOnly = true;

                dataGridViewOrder.Columns["OrderId"].Visible = false;
                dataGridViewOrder.Columns["SrId"].Visible = false;
                dataGridViewOrder.Columns["MarketId"].Visible = false;
                dataGridViewOrder.Columns["OrderDate"].Visible = false;
                dataGridViewOrder.Columns["StockId"].Visible = false;
                dataGridViewOrder.Columns["TotalQuantityToAddToTheCurrentStock"].Visible = false;
                dataGridViewOrder.Columns["CompanyId"].Visible = false;
                dataGridViewOrder.Columns["ItemId"].Visible = false;

                dataGridViewOrder.Columns["ChalanNo"].Visible = false;
                dataGridViewOrder.Columns["StockEntryDate"].Visible = false;

                dataGridViewOrder.Columns["IsItemReturned"].Visible = false;

                dataGridViewOrder.Columns["IsDamagedItemReturned"].Visible = false;

                isFromTemplate = false;
            }

            txtTotalOrderBill.Text = txtItemPriceDeduction.Text = txtBillPaidOnDate.Text = txtBillDue .Text = 0.ToString();
            numBillPaid.Value = 0;
            isPaymentCalculated = false;

            isOrderPlaced = false;
            isItemReturnedFromOrder = false;

            btnSaveOrderDetails.Enabled = false;
            numBillPaid.Enabled = false;
            btnAddDetailPayment.Enabled = false;

            if (comboOrderSRDSR.SelectedValue != null)
            {
                SRDSRDue previousDue = SRDSRManager.Instance.GetSrDsrDue(comboOrderSRDSR.SelectedValue.ToString());
                if (previousDue != null)
                    txtBillDue.Text = previousDue.Due.ToString();
                else
                    txtBillDue.Text = 0.ToString();
            }
        }
        #endregion

        #region Item Stock Management

        private void btnAddStock_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItemIdInStock.Text.Trim()))
            {
                ItemStock currentStock = new ItemStock();
                currentStock.ItemId = txtItemIdInStock.Text.Trim();
                currentStock.StockId = txtStockId.Text.Trim();
                currentStock.CountPerBox = NullHandler.GetInt32(txtItemPerBoxInStock.Text.Trim());
                
                decimal newBoxToAdd = numStockQuantityInBox.Value;
                decimal newExtraQuantityToAdd = numStockQuantity.Value;
                int totalStockNow = (NullHandler.GetInt32(txtCurrentStockInBox.Text.Trim()) * currentStock.CountPerBox) + NullHandler.GetInt32(txtCurrentStockWithOutBox.Text.Trim());
                
                currentStock.CurrentStockTotal = totalStockNow + Convert.ToInt32((newBoxToAdd * currentStock.CountPerBox) + newExtraQuantityToAdd);
                currentStock.ChalanNo = txtChalanNo.Text.Trim();
                currentStock.StockEntryDate = dateStockEntry.Value;

                SavingState saveState = ItemManager.Instance.SaveStockItem(currentStock);

                if (saveState.Equals(SavingState.Success))
                {
                    /// store the chalan
                    if (!string.IsNullOrEmpty(txtChalanNo.Text.Trim()) && (newBoxToAdd > 0 || newExtraQuantityToAdd > 0))
                    {
                        Chalan chalan = new Chalan();
                        chalan.ChalanNo = currentStock.ChalanNo;
                        chalan.ChalanDate = currentStock.StockEntryDate;
                        chalan.ItemId = currentStock.ItemId;
                        chalan.EntryCount = Convert.ToInt32((newBoxToAdd * currentStock.CountPerBox) + newExtraQuantityToAdd);

                        saveState = ChalanManager.Instance.SaveChalan(chalan);
                    }

                    txtStockItemName.Text = string.Empty;
                    DisplayAllCompanyStock();
                    MessageBox.Show("Items stock has been updated.");
                }
                else if (saveState.Equals(SavingState.DuplicateExists))
                {
                    MessageBox.Show("Items stock already exist. Please review the item.");
                }
                else if (saveState.Equals(SavingState.Failed))
                {
                    MessageBox.Show("Failed to update items stock information.");
                }
            }
            else {
                MessageBox.Show("Please select an item to update the stock.");
            }
        }

        private void btnViewCompanyStock_Click(object sender, EventArgs e)
        {
            DisplayAllCompanyStock();
        }

        //private void btnFilterStockEntry_Click(object sender, EventArgs e)
        //{
        //    isChalanFilterIsInAction = true;

        //    btnAddStock.Enabled = false;
        //    btnDeductFromStock.Enabled = false;
        //    btnRemoveSelectedStock.Enabled = false;
        //}

        private void btnDeductFromStock_Click(object sender, EventArgs e)
        {
            if (comboStockCompany.SelectedValue != null && !string.IsNullOrEmpty(txtItemIdInStock.Text.Trim()) && !string.IsNullOrEmpty(txtStockId.Text.Trim()))
            {
                if (numDeductBox.Value > 0 || numDeductExtraQuantity.Value > 0)
                {
                    int totalQuantityToDeduct = (NullHandler.GetInt32(txtItemPerBoxInStock.Text.Trim()) * NullHandler.GetInt32(numDeductBox.Value)) + NullHandler.GetInt32(numDeductExtraQuantity.Value);
                    int currentStockTotal = NullHandler.GetInt32(txtCurrentStockTotal.Text.Trim());

                    if (totalQuantityToDeduct <= currentStockTotal)
                    {
                        DialogResult dialogResult = MessageBox.Show("Would you really like to deduct the quantity from Item Stock?", "Reset Stock", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.Yes)
                        {
                            totalQuantityToDeduct = currentStockTotal - totalQuantityToDeduct;
                            /// deduct the ammount from stock
                            SavingState state = ItemManager.Instance.DeductItemsStock(totalQuantityToDeduct, txtStockId.Text.Trim());
                            if (state.Equals(SavingState.Success))
                            {
                                DisplayAllCompanyStock();
                                MessageBox.Show("Stock quantity deducted successfully");
                            }
                            else
                            {
                                MessageBox.Show("Stock quantity deduction failed");
                            }
                        }
                        else
                        {
                            /// do nothing
                        }
                    }
                    else
                    {
                        MessageBox.Show("Deduction quantity should not exceed the total quantity availabe in the stock.");
                    }
                }
                else
                {
                    MessageBox.Show("Please add some deduction quantity first.");
                }
            }
            else
            {
                MessageBox.Show("Please select an item from stock or not stock created for that item yet.");
            }
        }

        private void btnResetDamagedStockFromItem_Click(object sender, EventArgs e)
        {
            if ((comboStockCompany.SelectedValue != null && !string.IsNullOrEmpty(txtItemIdInStock.Text.Trim())) || (comboStockCompany.SelectedValue != null && chkResetAllFromCompany.Checked))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to reset the Item's damaged Stock?", "Reset Damaged Stock", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SavingState resetState = SavingState.None;

                    if (chkResetAllFromCompany.Checked)
                    {
                        resetState = ItemManager.Instance.DeductAllDamagedItemsStockOfCompany(comboStockCompany.SelectedValue.ToString());
                    }
                    else resetState = ItemManager.Instance.DeductDamagedItemsFromStock(txtItemIdInStock.Text.Trim());

                    if (resetState.Equals(SavingState.Success))
                    {
                        DisplayAllCompanyStock();
                        MessageBox.Show("Items damaged stock reset successfully");
                    }
                    else
                    {
                        MessageBox.Show("Items damaged stock reset failed. Please contact to the Administrator.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            else
            {
                MessageBox.Show("Please select an item stock to reset damaged stock or try checking reset all.");
            }
        }

        private void btnRemoveSelectedStock_Click(object sender, EventArgs e)
        {
            if ((comboStockCompany.SelectedValue != null && !string.IsNullOrEmpty(txtItemIdInStock.Text.Trim())) || (comboStockCompany.SelectedValue != null && chkResetAllFromCompany.Checked))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to reset the Item Stock?", "Reset Stock", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SavingState deleteState = SavingState.None;

                    if (chkResetAllFromCompany.Checked) {
                        deleteState = ItemManager.Instance.DeleteAllItemsStockOfCompany(comboStockCompany.SelectedValue.ToString());
                    }
                    else deleteState = ItemManager.Instance.DeleteItemsStock(txtItemIdInStock.Text.Trim());

                    if (deleteState.Equals(SavingState.Success))
                    {
                        DisplayAllCompanyStock();
                        MessageBox.Show("Items stock reset successfully");
                    }
                    else
                    {
                        MessageBox.Show("Items stock reset failed. Please contact to the Administrator.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            else
            {
                MessageBox.Show("Please select an item stock to reset.");
            }
        }

        private void comboStockCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtStockItemName.Text = string.Empty;
            DisplayAllCompanyStock();
        }

        private void dataGridStockDetails_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridStockDetails.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridStockDetails.SelectedRows[0];
                txtStockItemName.Text = row.Cells["ItemName"].Value.ToString();

                txtCurrentStockTotal.Text = NullHandler.GetString(row.Cells["CurrentStockTotal"].Value);

                txtCurrentStockInBox.Text = NullHandler.GetString(row.Cells["StockBoxCount"].Value);
                txtCurrentStockWithOutBox.Text = NullHandler.GetString(row.Cells["StockNotInBoxCount"].Value);

                txtCurrentDamagedStockInBox.Text = NullHandler.GetString(row.Cells["DamagedStockBoxCount"].Value);
                txtCurrentDamagedStockWithoutBox.Text = NullHandler.GetString(row.Cells["DamagedStockNotInBoxCount"].Value);

                txtItemPerBoxInStock.Text = NullHandler.GetString(row.Cells["CountPerBox"].Value);
                
                txtItemIdInStock.Text = NullHandler.GetString(row.Cells["ItemId"].Value);
                txtStockId.Text = NullHandler.GetString(row.Cells["StockId"].Value);

                txtChalanNo.Text = NullHandler.GetString(row.Cells["ChalanNo"].Value);
                dateStockEntry.Value = !row.Cells["StockEntryDate"].Value.Equals(DateTime.MinValue)? Convert.ToDateTime(row.Cells["StockEntryDate"].Value) : DateTime.Now.Date;

                chkResetAllFromCompany.Checked = false;
            }
        }

        private void LoadCompaniesIntoCompanyStocCombo()
        {
            comboStockCompany.ValueMember = "companyId";
            comboStockCompany.DisplayMember = "companyName";
            comboStockCompany.DataSource = CompanyManager.Instance.GetAllCompany();
        }

        private void DisplayAllCompanyStock()
        {
            //isChalanFilterIsInAction = false;

            //btnAddStock.Enabled = true;
            //btnDeductFromStock.Enabled = true;
            //btnRemoveSelectedStock.Enabled = true;

            if (!string.IsNullOrEmpty(NullHandler.GetString(comboStockCompany.SelectedValue)))
            {
                dataGridStockDetails.DataSource = ItemManager.Instance.GetAllStockItem(comboStockCompany.SelectedValue.ToString(), txtStockItemName.Text.Trim());
                dataGridStockDetails.Columns["CompanyId"].Visible = false;
                dataGridStockDetails.Columns["ItemId"].Visible = false;
                dataGridStockDetails.Columns["StockId"].Visible = false;
                dataGridStockDetails.Columns["TotalQuantityToAddToTheCurrentStock"].Visible = false;
                //dataGridStockDetails.Columns["CountPerBox"].Visible = false;
                dataGridStockDetails.Columns["Price"].Visible = false;
                dataGridStockDetails.Columns["CurrentStockTotal"].Visible = false;
                dataGridStockDetails.Columns["CurrentDamagedStockTotal"].Visible = false;

                dataGridStockDetails.Columns["ItemName"].DisplayIndex = 0;
                dataGridStockDetails.Columns["CountPerBox"].DisplayIndex = 1;
                dataGridStockDetails.Columns["StockBoxCount"].DisplayIndex = 2;
                dataGridStockDetails.Columns["StockNotInBoxCount"].DisplayIndex = 3;
                dataGridStockDetails.Columns["DamagedStockBoxCount"].DisplayIndex = 4;
                dataGridStockDetails.Columns["DamagedStockNotInBoxCount"].DisplayIndex = 5;
                dataGridStockDetails.Columns["ChalanNo"].DisplayIndex = 6;
                dataGridStockDetails.Columns["StockEntryDate"].DisplayIndex = 7;
            }

            ResetStockEntry();
        }

        private void ResetStockEntry() {

            txtItemIdInStock.Text = string.Empty;
            txtStockId.Text = string.Empty;
            txtCurrentStockTotal.Text = string.Empty;
            numStockQuantityInBox.Value = 0;
            numStockQuantity.Value = 0;
            //txtStockItemName.Text = string.Empty;
            txtItemPerBoxInStock.Text = string.Empty;
            txtCurrentStockInBox.Text = string.Empty;
            txtCurrentStockWithOutBox.Text = string.Empty;
            txtCurrentDamagedStockInBox.Text = string.Empty;
            txtCurrentDamagedStockWithoutBox.Text = string.Empty;

            numDeductBox.Value = 0;
            numDeductExtraQuantity.Value = 0;

            chkResetAllFromCompany.Checked = false;
        }

        #endregion

        #region Item Management

        private void btnSaveItem_Click(object sender, EventArgs e)
        {
            if (comboCompanies.SelectedValue != null && !string.IsNullOrEmpty(txtItemName.Text.Trim()))
            {
                Item item = new Item();
                item.CompanyId = comboCompanies.SelectedValue.ToString();
                item.ItemId = txtItemId.Text.Trim();
                item.ItemName = txtItemName.Text.Trim();
                item.CountPerBox = Convert.ToInt32(numItemsPerBox.Value);
                item.Price = Convert.ToDouble(numPricePerItem.Value);
                SavingState saveState = ItemManager.Instance.SaveItem(item);
                if (saveState.Equals(SavingState.Success))
                {
                    if (!string.IsNullOrEmpty(txtItemId.Text.Trim()))
                        checkAddAsNewItem.Checked = false;
                    DisplayAllItemsOfCompany();
                    MessageBox.Show("Item updated.");
                }
                else if (saveState.Equals(SavingState.DuplicateExists))
                {
                    MessageBox.Show("Item already exist. Please review the item name.");
                }
                else if (saveState.Equals(SavingState.Failed))
                {
                    MessageBox.Show("Failed to update item information.");
                }
            }
            else {
                MessageBox.Show("Company Or Item Name is Required.");
            }
        }

        private void btnRemoveSelectedItem_Click(object sender, EventArgs e)
        {
            if (comboCompanies.SelectedValue != null && !string.IsNullOrEmpty(txtItemName.Text.Trim()))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the Item?", "Remove Product", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SavingState deleteState = ItemManager.Instance.DeleteItem(txtItemId.Text.Trim());

                    if (deleteState.Equals(SavingState.Success))
                    {
                        checkAddAsNewItem.Checked = true;
                        DisplayAllItemsOfCompany();
                        MessageBox.Show("Item Removed Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Item Remove Failed. Please contact to the Administrator.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            else
            {
                MessageBox.Show("Please select an item to remove.");
            }
        }

        private void dataGridAllItemsOfCompany_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridAllItemsOfCompany.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridAllItemsOfCompany.SelectedRows[0];
                comboCompanies.SelectedValue = row.Cells["CompanyId"].Value;
                txtItemId.Text = row.Cells["ItemId"].Value.ToString();
                txtItemName.Text = row.Cells["ItemName"].Value.ToString();
                numItemsPerBox.Value = Convert.ToDecimal(row.Cells["CountPerBox"].Value);
                numPricePerItem.Value = Convert.ToDecimal(row.Cells["Price"].Value);
                checkAddAsNewItem.Checked = false;
            }
        }

        private void checkAddAsNewItem_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAddAsNewItem.Checked)
            {
                txtItemId.Text = string.Empty;
            }
        }

        private void comboCompanies_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkAddAsNewItem.Checked = true;
            txtItemId.Text = string.Empty;
            DisplayAllItemsOfCompany();
        }

        private void LoadCompaniesIntoCompanyCombo()
        {
            comboCompanies.ValueMember = "companyId";
            comboCompanies.DisplayMember = "companyName";
            comboCompanies.DataSource = CompanyManager.Instance.GetAllCompany();
        }

        private void DisplayAllItemsOfCompany()
        {
            ResetItemEntry();

            if (comboCompanies.SelectedValue != null)
            {
                dataGridAllItemsOfCompany.DataSource = ItemManager.Instance.GetAllItems(comboCompanies.SelectedValue.ToString());
                dataGridAllItemsOfCompany.Columns[0].Visible = false;
                dataGridAllItemsOfCompany.Columns[1].Visible = false;
            }
        }

        private void ResetItemEntry()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            numItemsPerBox.Value = 0;
            numPricePerItem.Value = 0;
        }

        #endregion

        #region Company Management
        private void btnAddCompany_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCompanyName.Text.Trim()))
            {
                Company currentCompany = new Company();
                currentCompany.companyId = txtCompanyId.Text.Trim();
                currentCompany.companyName = txtCompanyName.Text.Trim();

                SavingState saveState = CompanyManager.Instance.SaveCompany(currentCompany);

                if (saveState.Equals(SavingState.Success))
                {
                    if (!string.IsNullOrEmpty(txtCompanyId.Text.Trim()))
                        checkIsNewCompany.Checked = false;
                    DisplayCompanies();
                    MessageBox.Show("Company updated.");
                }
                else if (saveState.Equals(SavingState.DuplicateExists))
                {
                    MessageBox.Show("Company already exist. Please review the company name.");
                }
                else if (saveState.Equals(SavingState.Failed))
                {
                    MessageBox.Show("Failed to update company information.");
                }
            }
            else {

                MessageBox.Show("Company Name required.");
            }
        }

        private void btnRemoveSelectedCompany_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCompanyId.Text.Trim()))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the Company?", "Remove Company", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SavingState deleteState = CompanyManager.Instance.DeleteCompany(txtCompanyId.Text.Trim());

                    if (deleteState.Equals(SavingState.Success))
                    {
                        checkIsNewCompany.Checked = true;
                        DisplayCompanies();
                        MessageBox.Show("Company Removed Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Company Remove Failed. Please contact to the Administrator.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            else
            {
                MessageBox.Show("Please select a company to remove.");
            }
        }

        private void dataGridAllCompanies_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridAllCompanies.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridAllCompanies.SelectedRows[0];
                txtCompanyId.Text = row.Cells["companyId"].Value.ToString();
                txtCompanyName.Text = row.Cells["companyName"].Value.ToString();
                checkIsNewCompany.Checked = false;
            }
        }

        private void checkIsNewCompany_CheckedChanged(object sender, EventArgs e)
        {
            if (checkIsNewCompany.Checked) {
                txtCompanyId.Text = string.Empty;
            }
        }

        private void DisplayCompanies()
        {
            ResetConpanyEntry();

            dataGridAllCompanies.DataSource = CompanyManager.Instance.GetAllCompany();
            dataGridAllCompanies.Columns[0].Visible = false;
        }

        private void ResetConpanyEntry()
        {
            txtCompanyId.Text = string.Empty;
            txtCompanyName.Text = string.Empty;
        }
        #endregion

        #region Sr/Dsr Management
        private void btnAddSr_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSrDsrName.Text.Trim()) && !string.IsNullOrEmpty(txtCellNo.Text.Trim()))
            {
                SRDSR srDsr = new SRDSR();
                srDsr.Id = txtSrDsrId.Text.Trim();
                srDsr.Name = txtSrDsrName.Text.Trim();
                srDsr.Type = (SRType)Convert.ToInt32(comboSrType.SelectedValue);
                srDsr.CellNo = txtCellNo.Text.Trim();
                SavingState saveState = SRDSRManager.Instance.SaveSrDsr(srDsr);
                if (saveState.Equals(SavingState.Success))
                {
                    if (!string.IsNullOrEmpty(txtSrDsrId.Text.Trim()))
                        checkAddAsNewSr.Checked = false;
                    DisplaySrDsrs();
                    MessageBox.Show("Sr/Dsr updated.");
                }
                else if (saveState.Equals(SavingState.DuplicateExists))
                {
                    MessageBox.Show("Dr/Dsr already exist. Please review the Sr/Dsr name.");
                }
                else if (saveState.Equals(SavingState.Failed))
                {
                    MessageBox.Show("Failed to update Sr/Dsr information.");
                }
            }
            else {
                MessageBox.Show("Name and Phone number are required.");
            }
        }

        private void btnRemoveSelectedSrDsr_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSrDsrId.Text.Trim()))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the SR/DSR?", "Remove SR/DSR", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SavingState deleteState = SRDSRManager.Instance.DeleteSrDsr(txtSrDsrId.Text.Trim());

                    if (deleteState.Equals(SavingState.Success))
                    {
                        checkAddAsNewSr.Checked = true;
                        DisplaySrDsrs();
                        MessageBox.Show("Sr/Dsr Removed Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Sr/Dsr Remove Failed. Please contact to the Administrator.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            else
            {
                MessageBox.Show("Please select a Sr/Dsr to remove.");
            }
        }

        private void dataGridAllSrDsr_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridAllSrDsr.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridAllSrDsr.SelectedRows[0];
                txtSrDsrId.Text = row.Cells["Id"].Value.ToString();
                txtSrDsrName.Text = row.Cells["Name"].Value.ToString();

                if (row.Cells["Type"].Value.ToString().Trim().Equals("SR"))
                {
                    comboSrType.SelectedIndex = 0;
                }
                else {
                    comboSrType.SelectedIndex = 1;
                }

                txtCellNo.Text = row.Cells["CellNo"].Value.ToString();

                checkIsNewCompany.Checked = false;
            }
        }

        private void checkAddAsNewSr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkAddAsNewSr.Checked)
            {
                txtSrDsrId.Text = string.Empty;
            }
        }

        private void InitializeTheSrType()
        {
            var dict = new Dictionary<int, string>();
            dict.Add((int)SRType.SR, "SR");
            dict.Add((int)SRType.DSR, "DSR");
            dict.Add((int)SRType.Individual, "Individual");

            comboSrType.DataSource = new BindingSource(dict, null);
            comboSrType.DisplayMember = "Value";
            comboSrType.ValueMember = "Key";
        }

        private void DisplaySrDsrs()
        {
            ResetSrSdsEntry();

            dataGridViewOrder.AutoGenerateColumns = true;
            dataGridAllSrDsr.DataSource = SRDSRManager.Instance.GetAllSrDsrWithDues();
            dataGridViewOrder.AutoGenerateColumns = false;

            dataGridAllSrDsr.Columns["Id"].Visible = false;

            dataGridAllSrDsr.Columns["Due"].DisplayIndex = 4;
        }

        private void ResetSrSdsEntry()
        {
            txtSrDsrId.Text = string.Empty;
            txtSrDsrName.Text = string.Empty;
            txtCellNo.Text = string.Empty;
        }
        #endregion

        #region Sr/Dsr Payment History

        private void btnFilterSrDsrPaymentHistory_Click(object sender, EventArgs e)
        {
            srDsrDetailPaymentId.Text = string.Empty;

            if (comboDetailPaymentSrDsr.SelectedValue != null && comboDetailPaymentCompany.SelectedValue != null) {

                dataGridViewSrDsrDetailPayments.AutoGenerateColumns = true;
                dataGridViewSrDsrDetailPayments.DataSource = DetailPaymentManager.Instance.GetAllPaymentsOfSrDsr(comboDetailPaymentSrDsr.SelectedValue.ToString(), comboDetailPaymentCompany.SelectedValue.ToString(), dtDetailPaymentFromDate.Value, dtDetailPaymentToDate.Value);
                dataGridViewSrDsrDetailPayments.AutoGenerateColumns = false;

                
                dataGridViewSrDsrDetailPayments.Columns["Id"].Visible = false;
                dataGridViewSrDsrDetailPayments.Columns["Name"].Visible = false;
                dataGridViewSrDsrDetailPayments.Columns["Type"].Visible = false;
                dataGridViewSrDsrDetailPayments.Columns["CellNo"].Visible = false;
                dataGridViewSrDsrDetailPayments.Columns["Due"].Visible = false;
                dataGridViewSrDsrDetailPayments.Columns["PaymentId"].Visible = false;
                dataGridViewSrDsrDetailPayments.Columns["CompanyId"].Visible = false;

                dataGridViewSrDsrDetailPayments.Columns["PaymentDate"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["TotalPayment"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["ThousendCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["FiveHundredCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["OneHundredCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["FiftyCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["TwentyCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["TenCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["FiveCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["TwoCount"].DisplayIndex = 0;
                dataGridViewSrDsrDetailPayments.Columns["OneCount"].DisplayIndex = 0;

            }
        }

        private void dataGridViewSrDsrDetailPayments_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewSrDsrDetailPayments.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridViewSrDsrDetailPayments.SelectedRows[0];
                srDsrDetailPaymentId.Text = row.Cells["PaymentId"].Value.ToString();
            }
        }

        private void btnDeleteSelectedSrDsrPaymentHistory_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(srDsrDetailPaymentId.Text.Trim()))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the selected payment history?", "Remove Payment History", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SavingState deleteState = DetailPaymentManager.Instance.DeletePayment(srDsrDetailPaymentId.Text.Trim());

                    if (deleteState.Equals(SavingState.Success))
                    {
                        btnFilterSrDsrPaymentHistory_Click(null, null);
                        MessageBox.Show("Selected payment history removed successfully");
                    }
                    else
                    {
                        MessageBox.Show("Selected payment history remove failed. Please contact to the Administrator.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }

            }
            else
            {
                MessageBox.Show("Please select a payment history to remove.");
            }
        }

        private void LoadSrDsrIntoSRDSRPaymentHistoryCombo()
        {
            comboDetailPaymentSrDsr.ValueMember = "Id";
            comboDetailPaymentSrDsr.DisplayMember = "Name";
            comboDetailPaymentSrDsr.DataSource = SRDSRManager.Instance.GetAllSrDsr();
        }

        private void LoadCompaniesIntoSRDSRPaymentHistoryCombo()
        {
            comboDetailPaymentCompany.ValueMember = "companyId";
            comboDetailPaymentCompany.DisplayMember = "companyName";
            comboDetailPaymentCompany.DataSource = CompanyManager.Instance.GetAllCompany();
        }
        #endregion

        #region Market Management
        private void btnModifyMarket_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMarketName.Text.Trim()))
            {
                Market market = new Market();
                market.Id = txtMarketId.Text.Trim();
                market.Name = txtMarketName.Text.Trim();

                SavingState saveState = MarketManager.Instance.SaveMarket(market);
                if (saveState.Equals(SavingState.Success))
                {
                    if (!string.IsNullOrEmpty(txtMarketId.Text.Trim()))
                        checkAddAsNewSr.Checked = false;
                    DisplayAllMarkets();
                    MessageBox.Show("Market updated.");
                }
                else if (saveState.Equals(SavingState.DuplicateExists))
                {
                    MessageBox.Show("Market already exist. Please review the Sr/Dsr name.");
                }
                else if (saveState.Equals(SavingState.Failed))
                {
                    MessageBox.Show("Failed to update Market information.");
                }
            }
            else
            {
                MessageBox.Show("Name is required.");
            }
        }

        private void checkIsNewMarket_CheckedChanged(object sender, EventArgs e)
        {
            if (checkIsNewMarket.Checked)
            {
                txtMarketId.Text = string.Empty;
            }
        }

        private void dataGridViewAllMarkets_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAllMarkets.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridViewAllMarkets.SelectedRows[0];
                txtMarketId.Text = row.Cells["Id"].Value.ToString();
                txtMarketName.Text = row.Cells["Name"].Value.ToString();
                
                checkIsNewMarket.Checked = false;
            }
        }

        private void btnRemoveSelectedMarket_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMarketId.Text.Trim()))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the market?", "Remove Market", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    SavingState deleteState = MarketManager.Instance.DeleteMarket(txtMarketId.Text.Trim());

                    if (deleteState.Equals(SavingState.Success))
                    {
                        checkIsNewMarket.Checked = true;
                        DisplayAllMarkets();
                        MessageBox.Show("Market Removed Successfully");
                    }
                    else
                    {
                        MessageBox.Show("Market Remove Failed. Please contact to the Administrator.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }

            }
            else
            {
                MessageBox.Show("Please select a Market to remove.");
            }
        }

        private void DisplayAllMarkets()
        {
            ResetMarketEntry();
            dataGridViewAllMarkets.DataSource = MarketManager.Instance.GetAllMarket();
            dataGridViewAllMarkets.Columns[0].Visible = false;
        }

        private void ResetMarketEntry()
        {
            txtMarketId.Text = string.Empty;
            txtMarketName.Text = string.Empty;
        }
        #endregion

        #region User Management
        private void btnSaveUser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUserId.Text.Trim()) && !string.IsNullOrEmpty(txtUserName.Text.Trim()) && !string.IsNullOrEmpty(txtUserPassword.Text.Trim()))
            {
                User user = new User();
                user.userID = txtUserId.Text.Trim();
                user.userName = txtUserName.Text.Trim();
                user.password = txtUserPassword.Text.Trim();
                user.roleType = RoleType.Administrator;
                user.active = true;

                SavingState saveState = UserManager.Instance.SaveUser(user);
                if (saveState.Equals(SavingState.Success))
                {
                    if (!string.IsNullOrEmpty(txtUserId.Text.Trim()))
                        checkIsNewUser.Checked = false;
                    DisplayAllUsers();
                    MessageBox.Show("User updated.");
                }
                else if (saveState.Equals(SavingState.DuplicateExists))
                {
                    MessageBox.Show("User already exist. Please review the name.");
                }
                else if (saveState.Equals(SavingState.Failed))
                {
                    MessageBox.Show("Failed to update user information.");
                }
            }
            else
            {
                MessageBox.Show("User Id, User name and password is required.");
            }
            
        }

        private void btnRemoveUser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUserId.Text.Trim()))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the user?", "Remove User", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    /// do remove operation
                    if (UserManager.Instance.DeleteUser(txtUserId.Text.Trim()))
                    {
                        MessageBox.Show("Selected user has been removed successfully");
                        checkIsNewUser.Checked = true;
                        DisplayAllUsers();
                    }
                    else
                    {
                        MessageBox.Show("Removal of current user failed or this user is the default admin user");
                    }
                }
                else
                {

                }
            }
            else {
                MessageBox.Show("Please select an user to remove.");
            }
        }

        private void checkIsNewUser_CheckedChanged(object sender, EventArgs e)
        {
            if (checkIsNewUser.Checked)
            {
                txtUserId.Text = string.Empty;
                txtUserId.Enabled = true;
            }
            else
            {
                txtUserId.Enabled = false;
            }
        }

        private void dataGridViewAllUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAllUsers.SelectedRows.Count > 0)
            {
                txtUserId.Enabled = false;
                DataGridViewRow row = this.dataGridViewAllUsers.SelectedRows[0];
                txtUserId.Text = row.Cells["userID"].Value.ToString();
                txtUserName.Text = row.Cells["userName"].Value.ToString();
                checkIsNewUser.Checked = false;
            }
        }

        private void DisplayAllUsers()
        {
            ResetUserEntry();

            dataGridViewAllUsers.DataSource = UserManager.Instance.GetAllUsers();
            dataGridViewAllUsers.Columns[0].Visible = false;
            dataGridViewAllUsers.Columns[3].Visible = false;
            dataGridViewAllUsers.Columns[4].Visible = false;
            dataGridViewAllUsers.Columns[5].Visible = false;
        }

        private void ResetUserEntry()
        {
            txtUserId.Text = string.Empty;
            txtUserName.Text = string.Empty;
            txtUserPassword.Text = string.Empty;
        }

        #endregion

        #region Chalan Activities

        private void btnChalanActivityFilter_Click(object sender, EventArgs e)
        {
            DisplayAllChalans();
        }

        private void btnRemoveChalanActivity_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtChalanId.Text.Trim()))
            {
                DialogResult dialogResult = MessageBox.Show("Would you really like to remove the chalan activity?", "Remove Chalan Activity", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    /// do remove operation
                    if (ChalanManager.Instance.DeleteChalan(txtChalanId.Text.Trim()).Equals(SavingState.Success))
                    {
                        MessageBox.Show("Selected chalan activity has been removed successfully");
                        DisplayAllChalans();
                    }
                    else
                    {
                        MessageBox.Show("Removal of current chalan failed. Please contract to Administrator.");
                    }
                }
                else
                {

                }
            }
            else
            {
                MessageBox.Show("Please select a chalan to remove.");
            }
        }

        private void dataGridViewChalanActivities_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewChalanActivities.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridViewChalanActivities.SelectedRows[0];
                txtChalanId.Text = row.Cells["ChalanId"].Value.ToString();
            }
        }

        private void LoadCompaniesIntoChalanActivityCombo()
        {
            comboChalanActivityCompany.ValueMember = "companyId";
            comboChalanActivityCompany.DisplayMember = "companyName";
            comboChalanActivityCompany.DataSource = CompanyManager.Instance.GetAllCompany();
        }

        private void DisplayAllChalans()
        {
            txtChalanId.Text = string.Empty;

            if (!string.IsNullOrEmpty(NullHandler.GetString(comboChalanActivityCompany.SelectedValue)))
            {
                dataGridViewChalanActivities.AutoGenerateColumns = true;
                dataGridViewChalanActivities.DataSource = ChalanManager.Instance.GetAllChalan(comboChalanActivityCompany.SelectedValue.ToString(), dtChalanActivityFromDate.Value, dtChalanActivityToDate.Value, txtChalanActivityChalanNo.Text.Trim());
                dataGridViewChalanActivities.AutoGenerateColumns = false;

                dataGridViewChalanActivities.Columns["CompanyId"].Visible = false;
                dataGridViewChalanActivities.Columns["ItemId"].Visible = false;
                dataGridViewChalanActivities.Columns["Price"].Visible = false;
                dataGridViewChalanActivities.Columns["ChalanId"].Visible = false;
                dataGridViewChalanActivities.Columns["EntryCount"].Visible = false;
            }
        }

        #endregion

    }
}
