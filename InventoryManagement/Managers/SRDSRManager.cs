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
    public class SRDSRManager
    {
        private static SRDSRManager _Instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// to instantiate the SRDSRManager 
        /// </summary>
        public SRDSRManager()
        {

        }

        /// <summary>
        /// Public accessor to get SRDSRManager object
        /// </summary>
        public static SRDSRManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    //to avoid creation of multiple instance in a multithread environment
                    lock (padlock)
                    {
                        if (_Instance == null)
                            _Instance = new SRDSRManager();
                    }
                }
                return _Instance;
            }
        }


        public SavingState SaveSrDsr(SRDSR srDsr)
        {
            SavingState svState = SavingState.Failed;

            if (!string.IsNullOrEmpty(srDsr.Name) && !string.IsNullOrEmpty(srDsr.CellNo))
            {
                DbCommand thisCommand = null;
                try
                {

                    thisCommand = GenericDataAccess.CreateCommand();
                    thisCommand.CommandType = CommandType.Text;
                    /// if new sr
                    if (string.IsNullOrEmpty(srDsr.Id))
                    {
                        if (!IsSRDSRExist(srDsr.Name, srDsr.CellNo))
                        {
                            thisCommand.CommandText = "INSERT INTO IM_SR_DSR (Id, Name, Type, CellNo) VALUES(@Id, @Name, @Type, @CellNo)";
                            CreateParameter.AddParam(thisCommand, "@Id", Guid.NewGuid().ToString(), DbType.String);
                        }
                        else return SavingState.DuplicateExists;
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_SR_DSR SET Name = @Name, Type = @Type, CellNo = @CellNo WHERE Id = @Id";
                        CreateParameter.AddParam(thisCommand, "@Id", srDsr.Id, DbType.String);
                    }
                    CreateParameter.AddParam(thisCommand, "@Name", srDsr.Name, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@Type", (int)srDsr.Type, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@CellNo", srDsr.CellNo, DbType.String);

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

        public SavingState SaveSrDsrDue(SRDSRDue srDsrDue)
        {
            SavingState svState = SavingState.Failed;

            if (!string.IsNullOrEmpty(srDsrDue.Id))
            {
                DbCommand thisCommand = null;
                try
                {

                    thisCommand = GenericDataAccess.CreateCommand();
                    thisCommand.CommandType = CommandType.Text;
                    /// if new sr
                    if (!IsSRDSRDueExist(srDsrDue.Id))
                    {
                        thisCommand.CommandText = "INSERT INTO IM_SR_DSR_ORDER_DUE (SrId, TotalDue) VALUES(@SrId, @TotalDue)";
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_SR_DSR_ORDER_DUE SET TotalDue = @TotalDue WHERE SrId = @SrId";
                    }
                    CreateParameter.AddParam(thisCommand, "@SrId", srDsrDue.Id, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@TotalDue", Math.Round(srDsrDue.Due, 2), DbType.Double);

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

        public SavingState DeleteSrDsr(string Id)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_SR_DSR WHERE Id = @Id";

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

        public List<SRDSR> GetAllSrDsr()
        {
            DbCommand comm = null;
            List<SRDSR> srDsrs = new List<SRDSR>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_SR_DSR";
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    srDsrs.Add(MapSrDsr(dr));
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

            return srDsrs;
        }

        public List<SRDSRDue> GetAllSrDsrWithDues()
        {
            DbCommand comm = null;
            List<SRDSRDue> srDsrs = new List<SRDSRDue>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_SR_DSR srDsr LEFT JOIN IM_SR_DSR_ORDER_DUE srDsrDue ON srDsr.Id = srDsrDue.SrId ORDER BY Name";
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    srDsrs.Add(MapSrDsrWithDue(dr));
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

            return srDsrs;
        }

        public SRDSRDue GetSrDsrInfoWithDues(string Id)
        {
            DbCommand comm = null;
            SRDSRDue srDsr = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_SR_DSR srDsr LEFT JOIN IM_SR_DSR_ORDER_DUE srDsrDue ON srDsr.Id = srDsrDue.SrId WHERE srDsr.Id = @Id";
                CreateParameter.AddParam(comm, "@Id", Id, DbType.String);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                if(dr.Read())
                {
                    srDsr = MapSrDsrWithDue(dr);
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

            return srDsr;
        }

        public SRDSRDue GetSrDsrDue(string Id)
        {
            DbCommand comm = null;
            SRDSRDue srDsrDue = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_SR_DSR_ORDER_DUE WHERE SrId = @SrId";
                CreateParameter.AddParam(comm, "@SrId", Id, DbType.String);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                if(dr.Read())
                {
                    srDsrDue = MapSrDsrDue(dr);
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

            return srDsrDue;
        }

        public bool IsSRDSRExist(string name, string cellNo)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(Id) From IM_SR_DSR WHERE Name=@Name AND CellNo = @CellNo";
            CreateParameter.AddParam(comm, "@Name", name, DbType.String);
            CreateParameter.AddParam(comm, "@CellNo", cellNo, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        public bool IsSRDSRDueExist(string Id)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(SrId) From IM_SR_DSR_ORDER_DUE WHERE SrId=@SrId";
            CreateParameter.AddParam(comm, "@SrId", Id, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        #region private section
        private SRDSR MapSrDsr(DbDataReader dr)
        {
            SRDSR srDsr = new SRDSR();

            srDsr.Id = NullHandler.GetString(dr["Id"]);
            srDsr.Name = NullHandler.GetString(dr["Name"]);
            srDsr.Type = (SRType) NullHandler.GetInt32(dr["Type"]);
            srDsr.CellNo = NullHandler.GetString(dr["CellNo"]);

            return srDsr;
        }

        private SRDSRDue MapSrDsrWithDue(DbDataReader dr)
        {
            SRDSRDue srDsr = new SRDSRDue();

            srDsr.Id = NullHandler.GetString(dr["Id"]);
            srDsr.Name = NullHandler.GetString(dr["Name"]);
            srDsr.Type = (SRType)NullHandler.GetInt32(dr["Type"]);
            srDsr.CellNo = NullHandler.GetString(dr["CellNo"]);
            srDsr.Due = NullHandler.GetDouble(dr["TotalDue"]);
            return srDsr;
        }

        private SRDSRDue MapSrDsrDue(DbDataReader dr)
        {
            SRDSRDue srDsrDue = new SRDSRDue();

            srDsrDue.Id = NullHandler.GetString(dr["SrId"]);
            srDsrDue.Due = NullHandler.GetDouble(dr["TotalDue"]);

            return srDsrDue;
        }
        #endregion

    }
}
