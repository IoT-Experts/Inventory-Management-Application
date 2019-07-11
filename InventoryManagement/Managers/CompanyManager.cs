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
    public class CompanyManager
    {
        private static CompanyManager _Instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// to instantiate the CompanyManager 
        /// </summary>
        public CompanyManager()
        {

        }

        /// <summary>
        /// Public accessor to get CompanyManager object
        /// </summary>
        public static CompanyManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    //to avoid creation of multiple instance in a multithread environment
                    lock (padlock)
                    {
                        if (_Instance == null)
                            _Instance = new CompanyManager();
                    }
                }
                return _Instance;
            }
        }

        public SavingState SaveCompany(Company company)
        {
            SavingState svState = SavingState.Failed;

            if (!string.IsNullOrEmpty(company.companyName))
            {
                DbCommand thisCommand = null;
                try
                {

                    thisCommand = GenericDataAccess.CreateCommand();
                    thisCommand.CommandType = CommandType.Text;
                    /// if new company
                    if (string.IsNullOrEmpty(company.companyId))
                    {
                        if (!IsCompanyExist(company.companyName))
                        {
                            thisCommand.CommandText = "INSERT INTO IM_Company (CompanyId, CompanyName) VALUES(@CompanyId, @CompanyName)";
                            CreateParameter.AddParam(thisCommand, "@CompanyId", Guid.NewGuid().ToString(), DbType.String);
                        }
                        else return SavingState.DuplicateExists;
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_Company SET CompanyName = @CompanyName WHERE CompanyId = @CompanyId";
                        CreateParameter.AddParam(thisCommand, "@CompanyId", company.companyId, DbType.String);
                    }
                    CreateParameter.AddParam(thisCommand, "@CompanyName", company.companyName, DbType.String);

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

        public SavingState DeleteCompany(string companyId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_Company WHERE CompanyId=@CompanyId";

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

        public List<Company> GetAllCompany()
        {
            DbCommand comm = null;
            List<Company> companies = new List<Company>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_Company ORDER BY CompanyName";
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    companies.Add(MapCompany(dr));
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

            return companies;
        }

        public bool IsCompanyExist(string companyName)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(CompanyId) From IM_Company WHERE CompanyName=@CompanyName";
            CreateParameter.AddParam(comm, "@CompanyName", companyName, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        #region private section
        private Company MapCompany(DbDataReader dr)
        {
            Company company = new Company();

            company.companyId = NullHandler.GetString(dr["CompanyId"]);
            company.companyName = NullHandler.GetString(dr["CompanyName"]);

            return company;
        }
        #endregion
    }
}
