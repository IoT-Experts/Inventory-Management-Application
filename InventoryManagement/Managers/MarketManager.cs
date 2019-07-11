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
    public class MarketManager
    {
        private static MarketManager _Instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// to instantiate the MarketManager 
        /// </summary>
        public MarketManager()
        {

        }

        /// <summary>
        /// Public accessor to get MarketManager object
        /// </summary>
        public static MarketManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    //to avoid creation of multiple instance in a multithread environment
                    lock (padlock)
                    {
                        if (_Instance == null)
                            _Instance = new MarketManager();
                    }
                }
                return _Instance;
            }
        }

        public SavingState SaveMarket(Market market)
        {
            SavingState svState = SavingState.Failed;

            if (!string.IsNullOrEmpty(market.Name))
            {
                DbCommand thisCommand = null;
                try
                {

                    thisCommand = GenericDataAccess.CreateCommand();
                    thisCommand.CommandType = CommandType.Text;
                    /// if new sr
                    if (string.IsNullOrEmpty(market.Id))
                    {
                        if (!IsMarketExist(market.Name))
                        {
                            thisCommand.CommandText = "INSERT INTO IM_Markets (Id, Name) VALUES(@Id, @Name)";
                            CreateParameter.AddParam(thisCommand, "@Id", Guid.NewGuid().ToString(), DbType.String);
                        }
                        else return SavingState.DuplicateExists;
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_Markets SET Name = @Name WHERE Id = @Id";
                        CreateParameter.AddParam(thisCommand, "@Id", market.Id, DbType.String);
                    }
                    CreateParameter.AddParam(thisCommand, "@Name", market.Name, DbType.String);

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

        public SavingState DeleteMarket(string Id)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Markets WHERE Id = @Id";

                CreateParameter.AddParam(comm, "@Id", Id, DbType.String);
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

        public List<Market> GetAllMarket()
        {
            DbCommand comm = null;
            List<Market> market = new List<Market>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_Markets ORDER BY Name";
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    market.Add(MapMarket(dr));
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

            return market;
        }

        public bool IsMarketExist(string name)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(Id) From IM_Markets WHERE Name=@Name";
            CreateParameter.AddParam(comm, "@Name", name, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        #region private section
        private Market MapMarket(DbDataReader dr)
        {
            Market market = new Market();

            market.Id = NullHandler.GetString(dr["Id"]);
            market.Name = NullHandler.GetString(dr["Name"]);

            return market;
        }
        #endregion

    }
}
