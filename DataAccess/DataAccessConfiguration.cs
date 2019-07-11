using System;
using System.Configuration;

namespace DataAccess
{
    public static class DataAccessConfiguration
    {
        //Caches the connection string
        private readonly static string _dbConnectionString;        
        //Caches the dbpassword Key string
        private readonly static string _dbPasswordString;
        //Caches the data provider name
        private readonly static string _dbProviderName;
        //Caches the data provider name
        private readonly static SQLSyntax _dbServerType;

        //Caches the connection string
        private readonly static string _SyncdbConnectionString;        
        //Caches the data provider name
        private readonly static string _SyncdbProviderName;

        static DataAccessConfiguration()
        {

            _dbConnectionString = ConfigurationManager.ConnectionStrings["SqlServerDBConnectionString"].ConnectionString;
            _dbProviderName = ConfigurationManager.ConnectionStrings["SqlServerDBConnectionString"].ProviderName;
            //dbServerType = (SQLSyntax)Enum.Parse(typeof(SQLSyntax), Convert.ToString(ConfigurationManager.AppSettings["DBServerType"]));

            //if (dbProviderName == "System.Data.OleDb")
            //{
            //    //DataAccess.DataAccessConfiguration.AppPath = System.Windows.Forms.Application.StartupPath;
            //    string dbName = string.Empty, beforeDataSource = string.Empty;
            //    string[] pairs = dbConnectionString.Split(';');
            //    foreach (string param in pairs)
            //    {
            //        string[] value = new string[2];
            //        if (param.Contains("Data Source"))
            //        {
            //            value = param.Split('=');
            //            if (value.Length == 2)
            //                dbName = value[1].Trim();
            //        }
            //    }

            //    if (dbName.Length > 0)
            //    {
            //        beforeDataSource = dbConnectionString.Substring(0, dbConnectionString.IndexOf("Data Source"));
            //        //dbConnectionString = beforeDataSource + "Data Source = "
            //        //    + DataAccess.DataAccessConfiguration.AppPath + "\\" + dbName + ";";
            //        dbConnectionString = beforeDataSource + "Data Source = " + dbName + ";";
            //    }

            //    dbPasswordString = ConfigurationManager.AppSettings.Get("DBPassword");
            //    if (!string.IsNullOrEmpty(dbPasswordString))
            //        dbPasswordString = EncryptionManager.Decrypt(dbPasswordString, EncryptionManager.DefaultSalt);
            //    dbConnectionString += string.Format("Jet OLEDB:Database Password={0};", dbPasswordString);
            //}

            //// Secondary Sync Database connectionstring
            //if (ConfigurationManager.ConnectionStrings["IntermediateDBConnection"] != null)
            //{
            //    SyncdbConnectionString = ConfigurationManager.ConnectionStrings["IntermediateDBConnection"].ConnectionString;
            //    SyncdbProviderName = ConfigurationManager.ConnectionStrings["IntermediateDBConnection"].ProviderName;
            //}

        }

        /// <summary>
        /// Return the connection string for the database
        /// </summary>
        public static string DbConnectionString
        {
            get
            {
                return _dbConnectionString;
            }
        }

        /// <summary>
        /// Return the data provider name
        /// </summary>
        public static string DbProviderName
        {
            get
            {
                return _dbProviderName;
            }
        }

        /// <summary>
        /// Return the Database Server Type defined by appsettings
        /// </summary>
        public static SQLSyntax DbServerType
        {
            get
            {
                return _dbServerType;
            }
        }

        //Return the connection string for the secondary database
        public static string SyncDbConnectionString
        {
            get
            {
                return _SyncdbConnectionString;
            }
        }
        //Return the data provider name
        public static string SyncDbProviderName
        {
            get
            {
                return _SyncdbProviderName;
            }
        }

        private static string _appPath = string.Empty;
        public static string AppPath
        {
            get
            {
                return _appPath;
            }
            set
            {
                _appPath = value;
            }
        }
    }
}
