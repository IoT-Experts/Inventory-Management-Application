using DataAccess;
using InventoryManagement.Entities;
using InventoryManagement.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagement.Managers
{
    public class ItemManager
    {
        private static ItemManager _Instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// to instantiate the ItemManager 
        /// </summary>
        public ItemManager()
        {

        }

        /// <summary>
        /// Public accessor to get ItemManager object
        /// </summary>
        public static ItemManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    //to avoid creation of multiple instance in a multithread environment
                    lock (padlock)
                    {
                        if (_Instance == null)
                            _Instance = new ItemManager();
                    }
                }
                return _Instance;
            }
        }

        public SavingState SaveItem(Item item)
        {
            SavingState svState = SavingState.Failed;

            if (!string.IsNullOrEmpty(item.CompanyId) && !string.IsNullOrEmpty(item.ItemName))
            {
                DbCommand thisCommand = null;
                try
                {

                    thisCommand = GenericDataAccess.CreateCommand();
                    thisCommand.CommandType = CommandType.Text;
                    /// if new Item
                    if (string.IsNullOrEmpty(item.ItemId))
                    {
                        if (!IsItemExist(item.CompanyId, item.ItemName))
                        {
                            thisCommand.CommandText = "INSERT INTO IM_Items (CompanyId, ItemId, ItemName, CountPerBox, Price) VALUES(@CompanyId, @ItemId, @ItemName, @CountPerBox, @Price)";
                            CreateParameter.AddParam(thisCommand, "@ItemId", Guid.NewGuid().ToString(), DbType.String);
                        }
                        else return SavingState.DuplicateExists;
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_Items SET CompanyId = @CompanyId, ItemName = @ItemName, CountPerBox = @CountPerBox, Price = @Price WHERE ItemId = @ItemId";
                        CreateParameter.AddParam(thisCommand, "@ItemId", item.ItemId, DbType.String);
                    }

                    CreateParameter.AddParam(thisCommand, "@CompanyId", item.CompanyId, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@ItemName", item.ItemName, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@CountPerBox", item.CountPerBox, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@Price", item.Price, DbType.Double);

                    GenericDataAccess.ExecuteNonQuery(thisCommand);
                    thisCommand.Parameters.Clear();

                    svState = SavingState.Success;

                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("duplicate key"))
                        svState = SavingState.DuplicateExists;
                }
                finally
                {
                    if (thisCommand != null && thisCommand.Connection.State != ConnectionState.Closed)
                        thisCommand.Connection.Close();
                }
            }
            return svState;
        }

        public SavingState SaveChalan(ItemStock itemStock)
        {
            return SavingState.Failed;
        }

        public SavingState SaveStockItem(ItemStock itemStock, DbCommand command = null, bool isTransection = false) {

            SavingState svState = SavingState.Failed;

            if (!string.IsNullOrEmpty(itemStock.ItemId))
            {
                DbCommand thisCommand = null;
                try
                {
                    if (!isTransection)
                    {
                        thisCommand = GenericDataAccess.CreateCommand();
                        thisCommand.CommandType = CommandType.Text;
                    }
                    else {
                        thisCommand = command;
                    }
                    /// if new Item
                    if (string.IsNullOrEmpty(itemStock.StockId))
                    {
                        if (!isTransection)
                        {
                            if (!IsItemExistInStock(itemStock.ItemId))
                            {
                                thisCommand.CommandText = "INSERT INTO IM_Items_Stock (StockId, ItemId, TotalStock, DamagedStock, ChalanNo, StockEntryDate) VALUES(@StockId, @ItemId, @TotalStock, @DamagedStock, @ChalanNo, @StockEntryDate)";
                                CreateParameter.AddParam(thisCommand, "@StockId", Guid.NewGuid().ToString(), DbType.String);
                            }
                            else return SavingState.DuplicateExists;
                        }
                        else {

                            if (!IsItemExistInStock(itemStock.ItemId, thisCommand, isTransection))
                            {
                                thisCommand.CommandText = "INSERT INTO IM_Items_Stock (StockId, ItemId, TotalStock, DamagedStock, ChalanNo, StockEntryDate) VALUES(@StockId, @ItemId, @TotalStock, @DamagedStock, @ChalanNo, @StockEntryDate)";
                                CreateParameter.AddParam(thisCommand, "@StockId", Guid.NewGuid().ToString(), DbType.String);
                            }
                            else return SavingState.DuplicateExists;
                        }
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_Items_Stock SET TotalStock = @TotalStock, DamagedStock = @DamagedStock, ChalanNo = @ChalanNo, StockEntryDate = @StockEntryDate  WHERE StockId = @StockId AND ItemId = @ItemId";
                        CreateParameter.AddParam(thisCommand, "@StockId", itemStock.StockId, DbType.String);
                    }

                    CreateParameter.AddParam(thisCommand, "@ItemId", itemStock.ItemId, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@TotalStock", itemStock.CurrentStockTotal, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@DamagedStock", itemStock.CurrentDamagedStockTotal, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@ChalanNo", itemStock.ChalanNo, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@StockEntryDate", itemStock.StockEntryDate.Date, DbType.Date);

                    if (!itemStock.StockEntryDate.Date.Equals(DateTime.MinValue))
                    {
                        if (isTransection)
                            GenericDataAccess.ExecuteNonQueryTransaction(thisCommand);
                        else GenericDataAccess.ExecuteNonQuery(thisCommand);
                    }
                    thisCommand.Parameters.Clear();

                    svState = SavingState.Success;

                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("duplicate key"))
                        svState = SavingState.DuplicateExists;
                }
                finally
                {
                    if (!isTransection)
                        GenericDataAccess.CloseDBConnection(thisCommand);
                }
            }
            return svState;
        }

        public SavingState SaveItemsOrder(DataGridView dataGridViewOrder, string companyId, string srDsrId, string marketId, DateTime orderDate, SRDSRDue srDsrDue, bool isFromTemplate = false, bool isItemReturnedFromOrder = false, bool isDamagedItemReturnedFromOrder = false)
        {
            SavingState svState = SavingState.Failed;

            DbCommand thisCommand = null;
            try
            {

                thisCommand = GenericDataAccess.CreateCommand();
                thisCommand.CommandType = CommandType.Text;
                GenericDataAccess.OpenDBConnection(thisCommand);

                if ((isFromTemplate && !IsSrDsrOrderExist(companyId, srDsrId, marketId, orderDate)) || (!isFromTemplate && IsSrDsrOrderExist(companyId, srDsrId, marketId, orderDate)))
                {
                    foreach (DataGridViewRow row in dataGridViewOrder.Rows)
                    {
                        int stockAvailableInStore = 0;
                        int damagedItemAvailableInStock = 0;
                        if (string.IsNullOrEmpty(NullHandler.GetString(row.Cells["OrderId"].Value)))
                        {
                            /// insert the item order
                            thisCommand.CommandText = "INSERT INTO IM_Orders (OrderId, CompanyId, SrId, MarketId, ItemId, Date, OrderCount, ReturnCount, SoldCount, DamagedCount) VALUES(@OrderId, @CompanyId, @SrId, @MarketId, @ItemId, @Date, @OrderCount, @ReturnCount, @SoldCount, @DamagedCount)";
                            CreateParameter.AddParam(thisCommand, "@OrderId", Guid.NewGuid().ToString(), DbType.String);
                            CreateParameter.AddParam(thisCommand, "@CompanyId", companyId, DbType.String);
                            CreateParameter.AddParam(thisCommand, "@SrId", srDsrId, DbType.String);
                            CreateParameter.AddParam(thisCommand, "@MarketId", marketId, DbType.String);
                            CreateParameter.AddParam(thisCommand, "@ItemId", NullHandler.GetString(row.Cells["ItemId"].Value), DbType.String);
                            CreateParameter.AddParam(thisCommand, "@Date", orderDate.Date, DbType.Date);
                            stockAvailableInStore = NullHandler.GetInt32(row.Cells["CurrentStockTotal"].Value) - NullHandler.GetInt32(row.Cells["SellsCount"].Value);

                            damagedItemAvailableInStock = NullHandler.GetInt32(row.Cells["CurrentDamagedStockTotal"].Value);
                        }
                        else
                        {
                            /// update the item order based on OrderId
                            thisCommand.CommandText = "UPDATE IM_Orders SET OrderCount = @OrderCount, ReturnCount = @ReturnCount, SoldCount = @SoldCount, DamagedCount = @DamagedCount WHERE OrderId = @OrderId";
                            CreateParameter.AddParam(thisCommand, "@OrderId", row.Cells["OrderId"].Value.ToString(), DbType.String);

                            stockAvailableInStore = isItemReturnedFromOrder ? (NullHandler.GetInt32(row.Cells["CurrentStockTotal"].Value) + NullHandler.GetInt32(row.Cells["ReturnCount"].Value)) : NullHandler.GetInt32(row.Cells["CurrentStockTotal"].Value);

                            damagedItemAvailableInStock = isDamagedItemReturnedFromOrder? (NullHandler.GetInt32(row.Cells["CurrentDamagedStockTotal"].Value) + NullHandler.GetInt32(row.Cells["DamageCount"].Value)): NullHandler.GetInt32(row.Cells["CurrentDamagedStockTotal"].Value);
                        }

                        CreateParameter.AddParam(thisCommand, "@OrderCount", row.Cells["OrderCount"].Value, DbType.Int32);
                        CreateParameter.AddParam(thisCommand, "@ReturnCount", row.Cells["ReturnCount"].Value, DbType.Int32);
                        CreateParameter.AddParam(thisCommand, "@DamagedCount", row.Cells["DamageCount"].Value, DbType.Int32);
                        CreateParameter.AddParam(thisCommand, "@SoldCount", row.Cells["SellsCount"].Value, DbType.Int32);

                        GenericDataAccess.ExecuteNonQueryTransaction(thisCommand);
                        thisCommand.Parameters.Clear();

                        if (!string.IsNullOrEmpty(NullHandler.GetString(row.Cells["StockId"].Value)))
                        {
                            ItemStock stock = new ItemStock();
                            stock.StockId = NullHandler.GetString(row.Cells["StockId"].Value);
                            stock.ItemId = NullHandler.GetString(row.Cells["ItemId"].Value);
                            stock.ChalanNo = NullHandler.GetString(row.Cells["ChalanNo"].Value);
                            stock.StockEntryDate = NullHandler.GetDateTime(row.Cells["StockEntryDate"].Value);
                            stock.CurrentStockTotal = stockAvailableInStore;//NullHandler.GetInt32(row.Cells["CurrentStockTotal"].Value);
                            stock.CurrentDamagedStockTotal = damagedItemAvailableInStock;

                            svState = SaveStockItem(stock, thisCommand, true);
                        }

                    }
                }
                else svState = SavingState.DuplicateExists;

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                GenericDataAccess.CloseDBConnection(thisCommand);
            }

            if (svState.Equals(SavingState.Success) && !string.IsNullOrEmpty(srDsrDue.Id))
            {
                svState = SRDSRManager.Instance.SaveSrDsrDue(srDsrDue);
            }

            return svState;
        }

        public SavingState DeleteItem(string itemId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Items WHERE ItemId = @ItemId";

                CreateParameter.AddParam(comm, "@ItemId", itemId, DbType.String);
                GenericDataAccess.ExecuteNonQuery(comm);
                comm.Parameters.Clear();

                svState = SavingState.Success;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public SavingState DeleteItemsStock(string itemId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Items_Stock WHERE ItemId = @ItemId";

                CreateParameter.AddParam(comm, "@ItemId", itemId, DbType.String);
                GenericDataAccess.ExecuteNonQuery(comm);
                comm.Parameters.Clear();

                svState = SavingState.Success;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public SavingState DeleteAllItemsStockOfCompany(string companyId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Items_Stock WHERE ItemId IN (SELECT ItemId FROM IM_Items WHERE CompanyId = @CompanyId)";

                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                GenericDataAccess.ExecuteNonQuery(comm);
                comm.Parameters.Clear();

                svState = SavingState.Success;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public SavingState DeductItemsStock(int stockNeedToDeduct,string stockId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "UPDATE IM_Items_Stock SET TotalStock = @TotalStock WHERE StockId = @StockId";

                CreateParameter.AddParam(comm, "@TotalStock", stockNeedToDeduct, DbType.String);
                CreateParameter.AddParam(comm, "@StockId", stockId, DbType.String);
                GenericDataAccess.ExecuteNonQuery(comm);
                comm.Parameters.Clear();

                svState = SavingState.Success;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public SavingState DeductDamagedItemsFromStock(string stockId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "UPDATE IM_Items_Stock SET DamagedStock = @DamagedStock WHERE StockId = @StockId";

                CreateParameter.AddParam(comm, "@DamagedStock", 0, DbType.Int32);
                CreateParameter.AddParam(comm, "@StockId", stockId, DbType.String);
                GenericDataAccess.ExecuteNonQuery(comm);
                comm.Parameters.Clear();

                svState = SavingState.Success;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public SavingState DeductAllDamagedItemsStockOfCompany(string companyId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "UPDATE IM_Items_Stock SET DamagedStock = @DamagedStock WHERE ItemId IN (SELECT ItemId FROM IM_Items WHERE CompanyId = @CompanyId)";

                CreateParameter.AddParam(comm, "@DamagedStock", 0, DbType.Int32);
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                GenericDataAccess.ExecuteNonQuery(comm);
                comm.Parameters.Clear();

                svState = SavingState.Success;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public SavingState DeleteItemsOrder(string orderId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Orders WHERE OrderId = @OrderId";

                CreateParameter.AddParam(comm, "@OrderId", orderId, DbType.String);
                GenericDataAccess.ExecuteNonQuery(comm);
                comm.Parameters.Clear();

                svState = SavingState.Success;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public SavingState DeleteSrDSrOrder(string companyId, string srDsrId, string marketId, DateTime orderDate)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Orders WHERE CompanyId = @CompanyId AND SrId = @SrId AND MarketId = @MarketId AND Date = @Date";

                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                CreateParameter.AddParam(comm, "@SrId", srDsrId, DbType.String);
                CreateParameter.AddParam(comm, "@MarketId", marketId, DbType.String);
                CreateParameter.AddParam(comm, "@Date", orderDate.Date, DbType.Date);

                if (GenericDataAccess.ExecuteNonQuery(comm)> 0)
                {
                    svState = SavingState.Success;
                }

                comm.Parameters.Clear();

                
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate key"))
                    svState = SavingState.DuplicateExists;
            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return svState;
        }

        public List<Item> GetAllItems(string companyId)
        {
            DbCommand comm = null;
            List<Item> items = new List<Item>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_Items WHERE CompanyId = @CompanyId ORDER BY ItemName";
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    items.Add(MapItem(dr));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return items;
        }

        public List<ItemStock> GetAllStockItem(string companyId, string stockItemName)
        {
            DbCommand comm = null;
            List<ItemStock> stockItems = new List<ItemStock>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                string sql = @"SELECT itm.CompanyId, itm.ItemId, itm.ItemName, itm.CountPerBox, itms.StockId, itms.TotalStock, itms.DamagedStock, itms.ChalanNo, itms.StockEntryDate
                                    FROM IM_Items itm LEFT JOIN IM_Items_Stock itms 
                                    ON itm.ItemId = itms.ItemId 
                                    WHERE CompanyId = @CompanyId";

                if (!string.IsNullOrEmpty(stockItemName))
                {
                    sql += " AND ItemName LIKE @ItemName";
                    CreateParameter.AddParam(comm, "@ItemName", stockItemName + "%", DbType.String);
                }

                string orderBySQL = "ORDER BY ItemName";
                comm.CommandText = string.Format("{0} {1}", sql, orderBySQL);

                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);

                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    stockItems.Add(MapStockItem(dr));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return stockItems;
        }

        public List<ItemStock> GetAllStockItem(string companyId)
        {
            DbCommand comm = null;
            List<ItemStock> stockItems = new List<ItemStock>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT itm.CompanyId, itm.ItemId, itm.ItemName, itm.CountPerBox, itms.StockId, itms.TotalStock, itms.DamagedStock, itms.ChalanNo, itms.StockEntryDate
                                    FROM IM_Items itm LEFT JOIN IM_Items_Stock itms 
                                    ON itm.ItemId = itms.ItemId 
                                    WHERE CompanyId = @CompanyId ORDER BY ItemName";
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    stockItems.Add(MapStockItem(dr));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return stockItems;
        }

        public List<ItemOrder> GetOrderTemplate(string companyId)
        {
            DbCommand comm = null;
            List<ItemOrder> itemsOrder = new List<ItemOrder>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT itm.CompanyId, itm.ItemId, itm.ItemName, itm.CountPerBox, itm.Price, itms.StockId, itms.TotalStock, itms.DamagedStock, itms.ChalanNo, itms.StockEntryDate  
                                    FROM IM_Items itm LEFT JOIN IM_Items_Stock itms 
                                    ON itm.ItemId = itms.ItemId 
                                    WHERE itm.CompanyId = @CompanyId ORDER BY ItemName";
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    itemsOrder.Add(MapItemOrderTemplate(dr));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return itemsOrder;
        }
        public List<ItemOrder> GetAllOrders(string companyId, string srDsrId, string marketId, DateTime filterDate) {
            DbCommand comm = null;
            List<ItemOrder> itemsOrder = new List<ItemOrder>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT ord.OrderId, ord.SrId, ord.MarketId, ord.Date, ord.OrderCount, ord.ReturnCount, ord.DamagedCount, ord.SoldCount, itm.CompanyId, itm.ItemId, itm.ItemName, itm.CountPerBox, itm.Price, itms.StockId, itms.TotalStock, itms.DamagedStock, itms.ChalanNo, itms.StockEntryDate  
                                    FROM IM_Items itm LEFT JOIN IM_Items_Stock itms 
                                    ON itm.ItemId = itms.ItemId 
									LEFT JOIN IM_Orders ord ON itm.ItemId = ord.ItemId
                                    WHERE itm.CompanyId = @CompanyId AND ord.SrId = @SrId AND ord.MarketId = @MarketId AND ord.Date = @Date ORDER BY ItemName";
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                CreateParameter.AddParam(comm, "@SrId", srDsrId, DbType.String);
                CreateParameter.AddParam(comm, "@MarketId", marketId, DbType.String);
                CreateParameter.AddParam(comm, "@Date", filterDate.Date, DbType.Date);

                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    itemsOrder.Add(MapItemOrder(dr));
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (comm != null && comm.Connection.State != ConnectionState.Closed)
                    comm.Connection.Close();
            }

            return itemsOrder;
        }

        public bool IsItemExist(string companyId, string itemName)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(ItemId) From IM_Items WHERE CompanyId = @CompanyId AND ItemName = @ItemName";
            CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
            CreateParameter.AddParam(comm, "@ItemName", itemName, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        public bool IsItemExistInStock(string itemId, DbCommand command = null, bool isTransection = false)
        {
            bool ret = false;
            DbCommand comm = null;
            if (!isTransection)
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
            }
            else comm = command;
            comm.CommandText = @"Select count(StockId) From IM_Items_Stock WHERE ItemId = @ItemId";
            CreateParameter.AddParam(comm, "@ItemId", itemId, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;

            if (!isTransection)
            {
                GenericDataAccess.CloseDBConnection(comm);
            }
            return ret;
        }

        public bool IsSrDsrOrderExist(string companyId, string srDsrId, string marketId, DateTime orderDate)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(OrderId) From IM_Orders WHERE CompanyId = @CompanyId AND SrId = @SrId AND MarketId = @MarketId AND Date = @Date";
            CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
            CreateParameter.AddParam(comm, "@SrId", srDsrId, DbType.String);
            CreateParameter.AddParam(comm, "@MarketId", marketId, DbType.String);
            CreateParameter.AddParam(comm, "@Date", orderDate.Date, DbType.Date);

            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        #region private section
        private Item MapItem(DbDataReader dr)
        {
            Item item = new Item();

            item.CompanyId = NullHandler.GetString(dr["CompanyId"]);
            item.ItemId = NullHandler.GetString(dr["ItemId"]);
            item.ItemName = NullHandler.GetString(dr["ItemName"]);
            item.CountPerBox = NullHandler.GetInt32(dr["CountPerBox"]);
            item.Price = NullHandler.GetDouble(dr["Price"]);

            return item;
        }

        private ItemStock MapStockItem(DbDataReader dr)
        {
            ItemStock stockItem = new ItemStock();

            stockItem.CompanyId = NullHandler.GetString(dr["CompanyId"]);
            stockItem.ItemId = NullHandler.GetString(dr["ItemId"]);
            stockItem.ItemName = NullHandler.GetString(dr["ItemName"]);
            stockItem.CountPerBox = NullHandler.GetInt32(dr["CountPerBox"]);
            stockItem.StockId = NullHandler.GetString(dr["StockId"]);
            stockItem.CurrentStockTotal = NullHandler.GetInt32(dr["TotalStock"]);
            stockItem.CurrentDamagedStockTotal = NullHandler.GetInt32(dr["DamagedStock"]);
            stockItem.ChalanNo = NullHandler.GetString(dr["ChalanNo"]);
            stockItem.StockEntryDate = NullHandler.GetDateTime(dr["StockEntryDate"]);
            stockItem.CalculateBoxesFromTotalStock();
            stockItem.CalculateBoxesFromDamagedTotalStock();
            return stockItem;
        }

        private ItemOrder MapItemOrderTemplate(DbDataReader dr)
        {
            ItemOrder itemOrderTemplate = new ItemOrder();

            itemOrderTemplate.OrderId = string.Empty;
            itemOrderTemplate.SrId = string.Empty;
            itemOrderTemplate.MarketId = string.Empty;
            itemOrderTemplate.OrderDate = DateTime.Now;

            itemOrderTemplate.OrderCount = 0;
            itemOrderTemplate.OrderBoxCount = 0;
            itemOrderTemplate.OrderExtraCount = 0;

            itemOrderTemplate.ReturnCount = 0;
            itemOrderTemplate.ReturnBoxCount = 0;
            itemOrderTemplate.ReturnExtraCount = 0;

            itemOrderTemplate.DamageCount = 0;
            itemOrderTemplate.DamageBoxCount = 0;
            itemOrderTemplate.DamageExtraCount = 0;

            itemOrderTemplate.SellsCount = 0;
            itemOrderTemplate.SellsBoxCount = 0;
            itemOrderTemplate.SellsExtraCount = 0;


            itemOrderTemplate.CompanyId = NullHandler.GetString(dr["CompanyId"]);
            itemOrderTemplate.ItemId = NullHandler.GetString(dr["ItemId"]);
            itemOrderTemplate.ItemName = NullHandler.GetString(dr["ItemName"]);
            itemOrderTemplate.CountPerBox = NullHandler.GetInt32(dr["CountPerBox"]);
            itemOrderTemplate.Price = NullHandler.GetDouble(dr["Price"]);

            itemOrderTemplate.StockId = NullHandler.GetString(dr["StockId"]);
            itemOrderTemplate.CurrentStockTotal = NullHandler.GetInt32(dr["TotalStock"]);
            itemOrderTemplate.CurrentDamagedStockTotal = NullHandler.GetInt32(dr["DamagedStock"]);
            itemOrderTemplate.ChalanNo = NullHandler.GetString(dr["ChalanNo"]);
            itemOrderTemplate.StockEntryDate = NullHandler.GetDateTime(dr["StockEntryDate"]);

            itemOrderTemplate.CalculateBoxesFromTotalStock();
            //itemOrderTemplate.CalculateOrdersInBox();
            return itemOrderTemplate;
        }

        private ItemOrder MapItemOrder(DbDataReader dr)
        {
            ItemOrder itemOrder = new ItemOrder();

            itemOrder.OrderId = NullHandler.GetString(dr["OrderId"]);
            itemOrder.SrId = NullHandler.GetString(dr["SrId"]);
            itemOrder.MarketId = NullHandler.GetString(dr["MarketId"]);
            itemOrder.OrderDate = NullHandler.GetDateTime(dr["Date"]);
            itemOrder.OrderCount = NullHandler.GetInt32(dr["OrderCount"]);
            itemOrder.ReturnCount = NullHandler.GetInt32(dr["ReturnCount"]);
            itemOrder.DamageCount = NullHandler.GetInt32(dr["DamagedCount"]);
            itemOrder.SellsCount = NullHandler.GetInt32(dr["SoldCount"]);


            itemOrder.CompanyId = NullHandler.GetString(dr["CompanyId"]);
            itemOrder.ItemId = NullHandler.GetString(dr["ItemId"]);
            itemOrder.ItemName = NullHandler.GetString(dr["ItemName"]);
            itemOrder.CountPerBox = NullHandler.GetInt32(dr["CountPerBox"]);
            itemOrder.Price = NullHandler.GetDouble(dr["Price"]);
            itemOrder.TotalPrice = Math.Round(itemOrder.Price * itemOrder.SellsCount, 2);

            itemOrder.StockId = NullHandler.GetString(dr["StockId"]);
            itemOrder.CurrentStockTotal = NullHandler.GetInt32(dr["TotalStock"]);
            itemOrder.CurrentDamagedStockTotal = NullHandler.GetInt32(dr["DamagedStock"]);
            itemOrder.ChalanNo = NullHandler.GetString(dr["ChalanNo"]);
            itemOrder.StockEntryDate = NullHandler.GetDateTime(dr["StockEntryDate"]);

            itemOrder.CalculateBoxesFromTotalStock();
            itemOrder.CalculateOrdersInBox();

            return itemOrder;
        }
        #endregion
    }
}
