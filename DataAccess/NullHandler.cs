using System;
using System.Data;

namespace DataAccess
{
	#region Null Handler
	public class NullHandler
	{
        /// <summary>
        /// Returns Null if the value is less than or equal to 0
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
		public static object GetNullValue(int Value)
		{
			if(Value<=0)
			{
				return null;
			}
			else
			{
				return Value; 
			}
		}
		
        /// <summary>
        /// Returns Null if value is equal to DateTime.MinValue
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
		public static object GetNullValue(DateTime Value)
		{
			if(DateTime.MinValue==Value)
			{
				return null;
			}
			else
			{
				return Value; 
			}
		}
		
        /// <summary>
        /// Returns Null value if empty
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
		public static object GetNullValue(string Value)
		{
			if(Value.Length<=0)
			{
				return null;
			}
			else
			{
				return Value; 
			}
		}
		

		#region Default Null Values
        /// <summary>
        /// Default value 0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public static int GetInt32(object obj)
		{
            return obj == DBNull.Value || obj is string && (string)obj == "" ? 0 : Convert.ToInt32(obj);
		}

        public static Int64 GetInt64(object obj) 
        {
            return obj == DBNull.Value || obj is string && (string)obj == "" ? 0 : Convert.ToInt64(obj);
        }

        /// <summary>
        /// Default value Convert.ToByte(0)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte GetByte(object obj)
		{
            return obj == DBNull.Value ? Convert.ToByte(0) : Convert.ToByte(obj);
		}

        /// <summary>
        /// Default value 0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal GetDecimal(object obj)
		{
            return obj == DBNull.Value || obj is string && (string)obj == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(obj);
		}

        /// <summary>
        /// Default value 0
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetDouble(object obj)
		{
            return obj == DBNull.Value || obj is string && (string)obj == "" ? 0 : Convert.ToDouble(obj);
		}

        /// <summary>
        /// Default value false
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetBoolean(object obj)
		{
            return obj == DBNull.Value || obj is string && (string)obj == "" ? false : Convert.ToBoolean(obj);
		}

        /// <summary>
        /// Default value DateTime.MinValue
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(object obj)
		{
            return obj == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(obj);
		}

        /// <summary>
        /// Default value string.Empty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetString(object obj)
		{
            return obj == DBNull.Value || obj is string && (string)obj == "" ? string.Empty : Convert.ToString(obj);
		}

        /// <summary>
        /// Default value new char()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static char GetChar(object obj)
		{
            return obj == DBNull.Value || obj is string && (string)obj == "" ? new char() : Convert.ToChar(obj);
		}
		
		
		#endregion

	}
	#endregion
}