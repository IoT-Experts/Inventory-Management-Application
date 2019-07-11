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

namespace InventoryManagement.Managers
{
    public class ChalanManager
    {
        private static ChalanManager _Instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// to instantiate the ChalanManager 
        /// </summary>
        public ChalanManager()
        {

        }

        /// <summary>
        /// Public accessor to get ChalanManager object
        /// </summary>
        public static ChalanManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    //to avoid creation of multiple instance in a multithread environment
                    lock (padlock)
                    {
                        if (_Instance == null)
                            _Instance = new ChalanManager();
                    }
                }
                return _Instance;
            }
        }

        public SavingState SaveChalan(Chalan chalan)
        {
            SavingState svState = SavingState.Failed;

            if (!string.IsNullOrEmpty(chalan.ChalanNo))
            {
                DbCommand thisCommand = null;
                try
                {

                    thisCommand = GenericDataAccess.CreateCommand();
                    thisCommand.CommandType = CommandType.Text;

                    /// if new chalan
                    if (!IsItemChalanExist(chalan.ChalanNo, chalan.ItemId, chalan.ChalanDate))
                    {
                        thisCommand.CommandText = "INSERT INTO IM_Chalan_Activity (ChalanId, ChalanNo, ItemId, EntryCount, ChalanDate) VALUES(@ChalanId, @ChalanNo, @ItemId, @EntryCount, @ChalanDate)";
                        CreateParameter.AddParam(thisCommand, "@ChalanId", Guid.NewGuid().ToString(), DbType.String);
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_Chalan_Activity SET EntryCount = @EntryCount WHERE ItemId = @ItemId AND ChalanNo = @ChalanNo AND ChalanDate = @ChalanDate";
                    }
                    CreateParameter.AddParam(thisCommand, "@ChalanNo", chalan.ChalanNo, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@ItemId", chalan.ItemId, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@EntryCount", chalan.EntryCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@ChalanDate", chalan.ChalanDate.Date, DbType.Date);

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

        public SavingState DeleteChalan(string chalanId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Chalan_Activity WHERE ChalanId = @ChalanId";

                CreateParameter.AddParam(comm, "@ChalanId", chalanId, DbType.String);
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

        public List<Chalan> GetAllChalan(string companyId, DateTime fromDate, DateTime toDate, string chalanNo = "")
        {
            DbCommand comm = null;
            List<Chalan> chalans = new List<Chalan>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                string sql = @"SELECT ca.*, itms.ItemName, itms.CountPerBox FROM IM_Chalan_Activity ca INNER JOIN IM_Items itms ON ca.ItemId = itms.ItemId";
                string whereSql = "WHERE itms.CompanyId = @CompanyId AND ChalanDate BETWEEN @FromDate AND @ToDate";
                if (!string.IsNullOrEmpty(chalanNo)) {
                    whereSql = CreateParameter.CreateWhereClause(whereSql, "ca.ChalanNo LIKE @ChalanNo");
                    CreateParameter.AddParam(comm, "@ChalanNo", chalanNo + "%", DbType.String);
                }
                string orderBySql = "ORDER BY ChalanDate desc";
                sql = string.Format("{0} {1} {2}", sql, whereSql, orderBySql); 
                comm.CommandText = sql;
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                CreateParameter.AddParam(comm, "@FromDate", fromDate.Date, DbType.Date);
                CreateParameter.AddParam(comm, "@ToDate", toDate.Date, DbType.Date);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    chalans.Add(MapChalan(dr));
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

            return chalans;
        }

        public bool IsItemChalanExist(string chalanNo, string itemId, DateTime chalanDate)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(ChalanId) From IM_Chalan_Activity WHERE ChalanNo=@ChalanNo AND ItemId=@ItemId AND ChalanDate=@ChalanDate";
            CreateParameter.AddParam(comm, "@ChalanNo", chalanNo, DbType.String);
            CreateParameter.AddParam(comm, "@ItemId", itemId, DbType.String);
            CreateParameter.AddParam(comm, "@ChalanDate", chalanDate.Date, DbType.Date);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        #region private section
        private Chalan MapChalan(DbDataReader dr)
        {
            Chalan chalan = new Chalan();

            chalan.ChalanId = NullHandler.GetString(dr["ChalanId"]);
            chalan.ChalanNo = NullHandler.GetString(dr["ChalanNo"]);
            chalan.ItemId = NullHandler.GetString(dr["ItemId"]);
            chalan.ItemName = NullHandler.GetString(dr["ItemName"]);
            chalan.CountPerBox = NullHandler.GetInt32(dr["CountPerBox"]);
            chalan.EntryCount = NullHandler.GetInt32(dr["EntryCount"]);
            chalan.ChalanDate = NullHandler.GetDateTime(dr["ChalanDate"]);
            chalan.CalculateBoxesFromTotalChalan();
            return chalan;
        }
        #endregion
    }
}
