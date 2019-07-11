using System;
using System.Data;
using System.Data.Common;
using System.Web;

namespace DataAccess
{
    public class CreateParameter
    {
        CreateParameter()
        {
        }

        /// <summary>
        /// Create Parameter for comm with name, value, type and parameter direction
        /// </summary>
        /// <param name="comm">DbCommand</param>
        /// <param name="ParamName">Parameter Name</param>
        /// <param name="objValue">Parameter Value</param>
        /// <param name="Paramtype">Parameter Type of DbType</param>
        /// <param name="ParamDir">Parameter Direction</param>
        /// <returns></returns>
        public static DbParameter AddParameter(DbCommand comm, string ParamName, Object objValue, DbType Paramtype, ParameterDirection ParamDir)
        {
            DbParameter param = comm.CreateParameter();
            param.ParameterName = ParamName;
            param.DbType = Paramtype;
            param.Direction = ParamDir;
           
            if (param.Direction == ParameterDirection.Input)
            {
                param.Value = objValue;
            }
            comm.Parameters.Add(param);
            return param;
        }
               
        /// <summary>
        /// Create Parameter for comm with no value
        /// </summary>
        /// <param name="comm">DbCommand</param>
        /// <param name="ParamName">Parameter Name</param>
        /// <param name="Paramtype">Parameter Type of DbType</param>
        /// <param name="ParamDir">ParameterDirection</param>
        /// <returns></returns>
        public static DbParameter AddParameter(DbCommand comm, string ParamName, DbType Paramtype, ParameterDirection ParamDir)
        {
            DbParameter param = comm.CreateParameter();
            param.ParameterName = ParamName;
            param.DbType = Paramtype;
            param.Direction = ParamDir;
            comm.Parameters.Add(param);
            return param;
        }

        /// <summary>
        /// Create Parameter for comm with name, non null value and type 
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="ParamName">Parameter Name</param>
        /// <param name="objValue">Parameter Value</param>
        /// <param name="Paramtype">Parameter Type of DbType</param>
        public static void AddParam(DbCommand comm, string ParamName, Object objValue, DbType Paramtype)
        {
            if (Paramtype == DbType.DateTime)
            {
                DateTime org = (DateTime) objValue;
                objValue = new DateTime(org.Year, org.Month, org.Day, org.Hour, org.Minute, org.Second);
            }
            DbParameter param = comm.CreateParameter();
            param.ParameterName = ParamName;
            param.DbType = Paramtype;
            param.Direction = ParameterDirection.Input;
            param.Value = objValue;
            comm.Parameters.Add(param);
        }

        /// <summary>
        /// Create Parameter for comm with name, value allowing null and type
        /// </summary>
        /// <param name="comm"></param>
        /// <param name="ParamName"></param>
        /// <param name="objValue"></param>
        /// <param name="nullValue"></param>
        /// <param name="Paramtype"></param>
        public static void AddParamAllowNull(DbCommand comm, string ParamName, Object objValue, Object nullValue, DbType Paramtype)
        {
            DbParameter param = comm.CreateParameter();
            param.ParameterName = ParamName;
            param.DbType = Paramtype;
            param.Direction = ParameterDirection.Input;
            param.Value = (objValue==null || objValue.Equals(nullValue)) ? DBNull.Value : objValue;
            comm.Parameters.Add(param);
        }

        /// <summary>
        /// Generates where clause
        /// </summary>
        /// <param name="whereClause"> where clause to be appended (starting with no spaces)</param>
        /// <param name="newClauseToAppend">new clause to be appended</param>
        /// <returns></returns>
        public static string CreateWhereClause(string whereClause, string newClauseToAppend)
        {
            if (!whereClause.ToUpper().Trim().StartsWith("WHERE "))
            {
                whereClause += " WHERE ";
                whereClause += newClauseToAppend;
            }
            else
            {
                whereClause += " AND ";
                whereClause += newClauseToAppend;
            }
            return whereClause;
        }

        /// <summary>
        ///Generate OrderBy caluse 
        /// </summary>
        /// <param name="orderByClause"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDirection"></param>
        /// <returns></returns>
        public static string CreateOrderByClause(string orderByClause, string fieldName, string fieldDirection)
        {
            if (!orderByClause.ToUpper().StartsWith("ORDER BY "))
            {
                orderByClause = string.Format(" ORDER BY {0} {1}", fieldName, fieldDirection);
            }
            else
            {
                orderByClause += string.Format(" , {0} {1}", fieldName, fieldDirection);
            }
            return orderByClause;
        }

    }
}
