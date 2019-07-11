using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;

//using Common;

namespace DataAccess
{
    public static class GenericDataAccess
    {
        //static constructor
        static GenericDataAccess()
        {
            // TODO: Add constructor logic here
        }


        /// <summary>
        /// Creates and prepares a new DbCommand object on a new connection
        /// </summary>
        /// <returns>DbCommand</returns>
        public static DbCommand CreateCommand()
        {
            // Obtain the database provider name
            string dataProviderName = DataAccessConfiguration.DbProviderName;
            // Obtain the database connection string
            string connectionString = DataAccessConfiguration.DbConnectionString;
            // Create a new data provider factory
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProviderName);
            // Obtain a database specific connection object
            DbConnection conn = factory.CreateConnection();
            // Set the connection string
            conn.ConnectionString = connectionString;
            // Create a database specific command object
            DbCommand comm = conn.CreateCommand();
            // Set the command type to stored procedure
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandTimeout = 180;
            // Return the initialized command object
            return comm;
        }

        /// <summary>
        /// Creates and prepares a new DbCommand object on a new connection for specific Command Type
        /// </summary>
        /// <param name="cmdType">System.Data.CommandType</param>
        /// <returns>DbCommand</returns>
        public static DbCommand CreateCommand(CommandType cmdType)
        {
            // Obtain the database provider name
            string dataProviderName = DataAccessConfiguration.DbProviderName;
            // Obtain the database connection string
            string connectionString = DataAccessConfiguration.DbConnectionString;
            // Create a new data provider factory
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataProviderName);
            // Obtain a database specific connection object
            DbConnection conn = factory.CreateConnection();
            // Set the connection string
            conn.ConnectionString = connectionString;
            // Create a database specific command object
            DbCommand comm = conn.CreateCommand();
            // Set the command type to stored procedure
            comm.CommandType = cmdType;
            comm.CommandTimeout = 180;
            // Return the initialized command object
            return comm;
        }

        public static DbCommand CreateCommand(bool toIntermediate, CommandType cmdType)
        {
            if (!toIntermediate)
                return CreateCommand(cmdType);
            else
            {
                // Obtain the database provider name
                string dataProviderName = DataAccessConfiguration.SyncDbProviderName;
                // Obtain the database connection string
                string connectionString = DataAccessConfiguration.SyncDbConnectionString;
                // Create a new data provider factory
                DbProviderFactory factory = DbProviderFactories.GetFactory(dataProviderName);
                // Obtain a database specific connection object
                DbConnection conn = factory.CreateConnection();
                // Set the connection string
                conn.ConnectionString = connectionString;
                // Create a database specific command object
                DbCommand comm = conn.CreateCommand();
                // Set the command type to stored procedure
                comm.CommandType = cmdType;
                comm.CommandTimeout = 180;
                // Return the initialized command object
                return comm;
            }
        }

        /// <summary>
        /// Executes a command and returns the results as a DataTable object
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataTable ExecuteSelectCommand(DbCommand command)
        {
            // The DataTable to be returned 
            DataTable table;
            try
            {
                // Open the data connection 
                command.Connection.Open();

                // Execute the command and save the esults in a DataTable
                DbDataReader reader = command.ExecuteReader();
                table = new DataTable();
                table.Load(reader);
                // Close the reader 
                reader.Close();
            }
            catch (Exception ex)
            {
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();

                throw ex;
            }
            finally
            {
                // Close the connection
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
            return table;
        }

        /// <summary>
        /// Executes a command and returns the results as a DataTable object.
        /// Note: After executeing the command make sure the connection gets closed in the end.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataTable ExecuteSelectCommandTransection(DbCommand command)
        {
            // The DataTable to be returned 
            DataTable table;
            try
            {
                // Execute the command and save the results in a DataTable
                DbDataReader reader = command.ExecuteReader();
                table = new DataTable();
                table.Load(reader);
                // Close the reader 
                reader.Close();
            }
            catch (Exception ex)
            {
                //Utilities.LogError(ex);
                command.Connection.Close();
                throw ex;

            }
            finally
            {
                //if (command.Connection.State != ConnectionState.Closed)
                //    command.Connection.Close();
            }

            return table;
        }

        /// <summary>
        /// Executes a command and returns the results as a DataSet object.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataSet ExecuteSelectCommandDataSet(SqlCommand command)
        {
            // The DataTable to be returned 
            DataSet dSet = new DataSet();
            try
            {
                // Open the data connection 
                command.Connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dSet);
                // Close the reader 

            }
            catch (Exception ex)
            {
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
                throw ex;
            }
            finally
            {
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
            return dSet;
        }

        /// <summary>
        /// Execute an update, delete, or insert command and returns the number of affected rows.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(DbCommand command)
        {
            // The number of affected rows 
            int affectedRows = -1;
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the connection of the command
                if (command.Connection.State != ConnectionState.Open)
                     command.Connection.Open();
                // Execute the command and get the number of affected rows
                affectedRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                command.Connection.Close();
                throw ex;
            }
            finally
            {
                // Close the connection
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
            // return the number of affected rows
            return affectedRows;
        }

        public static void OpenDBConnection(DbCommand command)
        {
            // Open the connection of the command
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
        }

        public static void CloseDBConnection(DbCommand command)
        {
            // Open the connection of the command
            if (command != null && command.Connection.State != ConnectionState.Closed)
                command.Connection.Close();
        }
        /// <summary>
        /// Executes an update, delete, or insert command and returns the number of affected rows.
        /// Note: After executeing the command make sure the connection gets closed in the end.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int ExecuteNonQueryTransaction(DbCommand command)
        {
            // The number of affected rows 
            int affectedRows = -1;
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Execute the command and get the number of affected rows
                affectedRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                // Close the connection
                //command.Connection.Close();
            }

            // return the number of affected rows
            return affectedRows;
        }

        /// <summary>
        /// Executes a select command and return a single result as a string.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string ExecuteScalar(DbCommand command)
        {
            // The value to be returned 
            object value = new object();

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the connection of the command
                command.Connection.Open();
                // Execute the command and get the number of affected rows
                value = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                // Log eventual errors and rethrow them
                //Utilities.LogError(ex);
                throw ex;
            }
            finally
            {
                // Close the connection
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
            if (value == null)
            {
                value = string.Empty;
            }
            return value.ToString();
        }

        /// <summary>
        /// Executes a select command and return a single result as a string.
        /// Note: Execute the command making sure the connection gets closed in the end.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string ExecuteScalarTransection(DbCommand command)
        {

            // The value to be returned 
            object value = new object();

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Execute the command and get the number of affected rows
                value = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the connection
                //command.Connection.Close();
            }
            if (value == null)
            {
                value = string.Empty;
            }
            return value.ToString();
        }

        /// <summary>
        /// Execute a select command and returns the data in the form of a datareader object.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
		
        public static DbDataReader ExecuteQuery(DbCommand command)
        {
            DbDataReader result;

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the connection of the command
                command.Connection.Open();
                // Execute the command and get the result
                result = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                // Close the connection
                command.Connection.Close();
                throw ex;
            }
            finally
            {
                // Close the connection
                //if (command.Connection.State != ConnectionState.Closed)
                //    command.Connection.Close();
            }

            // Return the datareader 
            return result;
        }

        /// <summary>
        /// Executes an insert command and returns the new identity number.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="returnID"></param>
        /// <returns></returns>
        public static int ExecuteInsertCommand(DbCommand command, bool returnID)
        {
            int affectedRows = -1;
            int newID = -1;
            try
            {
                command.Connection.Open();
                // Execute the command and get the number of affected rows
                affectedRows = command.ExecuteNonQuery();
                if (returnID)
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT @@IDENTITY";
                    newID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                command.Connection.Close();
                throw ex;
            }
            finally
            {
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
            return (returnID) ? newID : affectedRows;
        }

        /// <summary>
        /// Note: Execute the command making sure the connection gets closed in the end.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="returnID"></param>
        /// <returns></returns>
        public static int ExecuteInsertCommandTransaction(DbCommand command, bool returnID)
        {
            int affectedRows = -1;
            int newID = -1;
            try
            {
                // Execute the command and get the number of affected rows
                affectedRows = command.ExecuteNonQuery();
                if (returnID)
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT @@IDENTITY";
                    newID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (returnID) ? newID : affectedRows;
        }


        //public static string GetIntermediateConnectionStrings()
        //{
        //    return ConfigurationManager.ConnectionStrings["IntermediateDBConnection"].ConnectionString;
        //}

        public static string GetConnectionStrings()
        {
            return DataAccessConfiguration.DbConnectionString;
        }

        public static bool BulkWriteToServer(DbCommand command, SqlBulkCopy sqlBulkInsersion, DataTable table)
        {
            bool isInserted = false;
            try
            {
                command.Connection.Open();
                sqlBulkInsersion.WriteToServer(table);
                isInserted = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command.Connection.State != ConnectionState.Closed)
                    command.Connection.Close();
            }
            return isInserted;
        }
    }
}
