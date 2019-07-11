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
    public class DetailPaymentManager
    {
        private static DetailPaymentManager _Instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// to instantiate the DetailPaymentManager 
        /// </summary>
        public DetailPaymentManager()
        {

        }

        /// <summary>
        /// Public accessor to get DetailPaymentManager object
        /// </summary>
        public static DetailPaymentManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    //to avoid creation of multiple instance in a multithread environment
                    lock (padlock)
                    {
                        if (_Instance == null)
                            _Instance = new DetailPaymentManager();
                    }
                }
                return _Instance;
            }
        }

        public SavingState SaveSrPaymentDetails(DetailPaymentInfo paymentInfo)
        {
            SavingState svState = SavingState.Failed;

            /// paymentInfo.Id is SrId
            if (paymentInfo != null && !string.IsNullOrEmpty(paymentInfo.Id) && paymentInfo.PaymentDate != null && !paymentInfo.PaymentDate.Equals(DateTime.MinValue))
            {
                DbCommand thisCommand = null;
                try
                {

                    thisCommand = GenericDataAccess.CreateCommand();
                    thisCommand.CommandType = CommandType.Text;
                    /// if new payment
                    if (!IsPaymentInfoExist(paymentInfo.Id, paymentInfo.CompanyId, paymentInfo.PaymentDate))
                    {
                        thisCommand.CommandText = "INSERT INTO IM_SR_DSR_PAYMENT_DETAILS (PaymentId, SrId, CompanyId, PaymentDate, ThousendCount, FiveHundredCount, OneHundredCount, FiftyCount, TwentyCount, TenCount, FiveCount, TwoCount, OneCount, TotalPayment) VALUES(@PaymentId, @SrId, @CompanyId, @PaymentDate, @ThousendCount, @FiveHundredCount, @OneHundredCount, @FiftyCount, @TwentyCount, @TenCount, @FiveCount, @TwoCount, @OneCount, @TotalPayment)";
                        CreateParameter.AddParam(thisCommand, "@PaymentId", Guid.NewGuid().ToString(), DbType.String);
                    }
                    else
                    {
                        thisCommand.CommandText = "UPDATE IM_SR_DSR_PAYMENT_DETAILS SET PaymentDate = @PaymentDate, ThousendCount = @ThousendCount, FiveHundredCount = @FiveHundredCount, OneHundredCount = @OneHundredCount, FiftyCount = @FiftyCount, TwentyCount = @TwentyCount, TenCount = @TenCount, FiveCount = @FiveCount, TwoCount = @TwoCount, OneCount = @OneCount, TotalPayment = @TotalPayment WHERE SrId = @SrId AND CompanyId = @CompanyId AND PaymentDate = @PaymentDate";
                    }

                    CreateParameter.AddParam(thisCommand, "@SrId", paymentInfo.Id, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@CompanyId", paymentInfo.CompanyId, DbType.String);
                    CreateParameter.AddParam(thisCommand, "@PaymentDate", paymentInfo.PaymentDate, DbType.DateTime);
                    CreateParameter.AddParam(thisCommand, "@ThousendCount", paymentInfo.ThousendCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@FiveHundredCount", paymentInfo.FiveHundredCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@OneHundredCount", paymentInfo.OneHundredCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@FiftyCount", paymentInfo.FiftyCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@TwentyCount", paymentInfo.TwentyCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@TenCount", paymentInfo.TenCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@FiveCount", paymentInfo.FiveCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@TwoCount", paymentInfo.TwoCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@OneCount", paymentInfo.OneCount, DbType.Int32);
                    CreateParameter.AddParam(thisCommand, "@TotalPayment", Math.Round(paymentInfo.TotalPayment, 2), DbType.Decimal);

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

        public SavingState DeletePayment(string paymentId)
        {
            SavingState svState = SavingState.Failed;

            DbCommand comm = null;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "DELETE FROM IM_SR_DSR_PAYMENT_DETAILS WHERE PaymentId=@PaymentId";

                CreateParameter.AddParam(comm, "@PaymentId", paymentId, DbType.String);
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

        public List<DetailPaymentInfo> GetAllPaymentsOfSrDsr(string srDsrId, string companyId, DateTime fromDate, DateTime toDate)
        {
            DbCommand comm = null;
            List<DetailPaymentInfo> detailPayments = new List<DetailPaymentInfo>();
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT * FROM IM_SR_DSR_PAYMENT_DETAILS WHERE SrId = @SrId AND CompanyId = @CompanyId AND PaymentDate BETWEEN @FromDate AND @ToDate ORDER BY PaymentDate desc";
                CreateParameter.AddParam(comm, "@SrId", srDsrId, DbType.String);
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                CreateParameter.AddParam(comm, "@FromDate", fromDate.Date, DbType.DateTime);
                CreateParameter.AddParam(comm, "@ToDate", toDate.Date, DbType.DateTime);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                while (dr.Read())
                {
                    detailPayments.Add(MapPayment(dr));
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

            return detailPayments;
        }

        public decimal GetTotalPaymentsOf(string srDsrId, string companyId, DateTime paymentDate)
        {
            DbCommand comm = null;
            decimal totalPayment = 0;
            try
            {
                comm = GenericDataAccess.CreateCommand();
                comm.CommandType = CommandType.Text;
                comm.CommandText = @"SELECT TotalPayment FROM IM_SR_DSR_PAYMENT_DETAILS WHERE SrId = @SrId AND CompanyId = @CompanyId AND PaymentDate = @PaymentDate";
                CreateParameter.AddParam(comm, "@SrId", srDsrId, DbType.String);
                CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
                CreateParameter.AddParam(comm, "@PaymentDate", paymentDate.Date, DbType.DateTime);
                DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
                if (dr.Read())
                {
                   totalPayment = NullHandler.GetDecimal(dr["TotalPayment"]); 
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

            return totalPayment;
        }

        public bool IsPaymentInfoExist(string paymentId)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(PaymentId) From IM_SR_DSR_PAYMENT_DETAILS WHERE PaymentId=@PaymentId";
            CreateParameter.AddParam(comm, "@PaymentId", paymentId, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        public bool IsPaymentInfoExist(string srId, string companyId, DateTime paymentDate)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(PaymentId) From IM_SR_DSR_PAYMENT_DETAILS WHERE SrId=@SrId AND CompanyId = @CompanyId AND PaymentDate=@PaymentDate";
            CreateParameter.AddParam(comm, "@SrId", srId, DbType.String);
            CreateParameter.AddParam(comm, "@CompanyId", companyId, DbType.String);
            CreateParameter.AddParam(comm, "@PaymentDate", paymentDate, DbType.DateTime);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        #region private section
        private DetailPaymentInfo MapPayment(DbDataReader dr)
        {
            DetailPaymentInfo detailPaymentInfo = new DetailPaymentInfo();

            detailPaymentInfo.PaymentId = NullHandler.GetString(dr["PaymentId"]);
            detailPaymentInfo.Id = NullHandler.GetString(dr["SrId"]);
            detailPaymentInfo.CompanyId = NullHandler.GetString(dr["CompanyId"]);
            detailPaymentInfo.PaymentDate = NullHandler.GetDateTime(dr["PaymentDate"]);

            detailPaymentInfo.ThousendCount = NullHandler.GetInt32(dr["ThousendCount"]);
            detailPaymentInfo.FiveHundredCount = NullHandler.GetInt32(dr["FiveHundredCount"]);
            detailPaymentInfo.OneHundredCount = NullHandler.GetInt32(dr["OneHundredCount"]);
            detailPaymentInfo.FiftyCount = NullHandler.GetInt32(dr["FiftyCount"]);
            detailPaymentInfo.TwentyCount = NullHandler.GetInt32(dr["TwentyCount"]);
            detailPaymentInfo.TenCount = NullHandler.GetInt32(dr["TenCount"]);
            detailPaymentInfo.FiveCount = NullHandler.GetInt32(dr["FiveCount"]);
            detailPaymentInfo.TwoCount = NullHandler.GetInt32(dr["TwoCount"]);
            detailPaymentInfo.OneCount = NullHandler.GetInt32(dr["OneCount"]);

            detailPaymentInfo.TotalPayment = NullHandler.GetDecimal(dr["TotalPayment"]);

            return detailPaymentInfo;
        }
        #endregion
    }
}
