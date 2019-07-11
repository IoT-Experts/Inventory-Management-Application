using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using InventoryManagement.Entities;
using DataAccess;
using InventoryManagement.Utilities;
using InventoryManagement.Enums;

namespace InventoryManagement.Managers
{
    public class UserManager
    {
        private static UserManager _Instance = null;
        private static readonly object padlock = new object();

        /// <summary>
        /// to instantiate the UserManager 
        /// </summary>
        public UserManager()
        {

        }

        /// <summary>
        /// Public accessor to get UserManager object
        /// </summary>
        public static UserManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    //to avoid creation of multiple instance in a multithread environment
                    lock (padlock)
                    {
                        if (_Instance == null)
                            _Instance = new UserManager();
                    }
                }
                return _Instance;
            }
        }

        private User Mapobject(DbDataReader dr)
        {
            User user =new User();

            user.userID = NullHandler.GetString(dr["USER_ID"]);
            user.userName= NullHandler.GetString(dr["USER_NAME"]);
            user.password = NullHandler.GetString(dr["PASSWORD"]);
            user.creationDate= NullHandler.GetDateTime(dr["CREATION_DATE"]);
            user.roleType = (RoleType)NullHandler.GetInt32(dr["ROLE_TYPE"]);
            user.active= NullHandler.GetBoolean(dr["ACTIVE"]);
           
            return user;
        }

        public User AuthenticateUser(string userId, string passwordAsPlainText)
        {   
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"SELECT * FROM IM_USERS WHERE USER_ID=@USER_ID AND PASSWORD=@PASSWORD";
            CreateParameter.AddParam(comm, "@USER_ID", userId, DbType.String);
            CreateParameter.AddParam(comm, "@PASSWORD", passwordAsPlainText, DbType.String);

            DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
            User user = null;
            if (dr.Read())
            {
                user = Mapobject(dr);                
            }
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();

            return user;
        }

        public List<User> GetAllUsers()
        {
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"SELECT * FROM IM_USERS ORDER BY USER_NAME";
            
            DbDataReader dr = GenericDataAccess.ExecuteQuery(comm);
            List<User> users = new List<User>();
            while(dr.Read())
            {
                users.Add(Mapobject(dr));
            }
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();

            return users;
        }

        public SavingState SaveUser(User objUser)
        {
            SavingState svState = SavingState.Failed;

            if (objUser != null)
            {
                if (!IsUserExist(objUser.userName))
                {
                    if (!IsUserIdExist(objUser.userID))
                    {
                        DbCommand thisCommand = null;
                        try
                        {
                            thisCommand = GenericDataAccess.CreateCommand();
                            thisCommand.CommandType = CommandType.Text;

                            thisCommand.CommandText = "INSERT INTO IM_USERS (USER_ID, USER_NAME, PASSWORD, CREATION_DATE, ROLE_TYPE, ACTIVE) VALUES(@USER_ID, @USER_NAME, @PASSWORD, @CREATION_DATE, @ROLE_TYPE, @ACTIVE)";
                            CreateParameter.AddParam(thisCommand, "@USER_ID", objUser.userID, DbType.String);
                            CreateParameter.AddParam(thisCommand, "@USER_NAME", objUser.userName, DbType.String);
                            CreateParameter.AddParam(thisCommand, "@PASSWORD", objUser.password, DbType.String);
                            CreateParameter.AddParam(thisCommand, "@CREATION_DATE", DateTime.Now, DbType.DateTime);
                            CreateParameter.AddParam(thisCommand, "@ROLE_TYPE", (int)objUser.roleType, DbType.Int32);
                            CreateParameter.AddParam(thisCommand, "@ACTIVE", objUser.active, DbType.Boolean);

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
                    else svState = UpdateUser(objUser);
                }
                else svState = SavingState.DuplicateExists;
            }
            else svState = SavingState.Failed;

            return svState;
        }

        public SavingState UpdateUser(User objUser)
        {
            SavingState svState = SavingState.Failed;

            DbCommand thisCommand = null;
            try
            {
                thisCommand = GenericDataAccess.CreateCommand();
                thisCommand.CommandType = CommandType.Text;

                thisCommand.CommandText = "UPDATE IM_USERS SET USER_NAME=@USER_NAME, PASSWORD=@PASSWORD, ACTIVE=@ACTIVE WHERE USER_ID=@USER_ID";
                CreateParameter.AddParam(thisCommand, "@USER_ID", objUser.userID, DbType.String);
                CreateParameter.AddParam(thisCommand, "@USER_NAME", objUser.userName, DbType.String);
                CreateParameter.AddParam(thisCommand, "@PASSWORD", objUser.password, DbType.String);
                CreateParameter.AddParam(thisCommand, "@ACTIVE", objUser.active, DbType.Boolean);

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

            return svState;
        }

        public bool DeactiveUser(string userName)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"UPDATE IM_USERS SET ACTIVE = @ACTIVE WHERE USER_NAME=@USER_NAME";
            CreateParameter.AddParam(comm, "@USER_NAME", userName, DbType.String);
            CreateParameter.AddParam(comm, "@ACTIVE", false, DbType.Boolean);
            if (GenericDataAccess.ExecuteNonQuery(comm) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        public bool DeleteUser(string userId)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"DELETE FROM IM_USERS WHERE USER_ID <> 'admin' AND USER_ID = @USER_ID";
            CreateParameter.AddParam(comm, "@USER_ID", userId, DbType.String);
            if (GenericDataAccess.ExecuteNonQuery(comm) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        public bool IsUserDeactivated(string userName)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select ACTIVE From IM_USERS WHERE USER_NAME=@USER_NAME";
            CreateParameter.AddParam(comm, "@USER_NAME", userName, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            
                
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            if (strRet.Equals("0"))
                ret = true;

            return ret;
        }

        public static bool IsUserExist(string userName)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(USER_NAME) From IM_USERS WHERE USER_NAME=@USER_NAME";
            CreateParameter.AddParam(comm, "@USER_NAME", userName, DbType.String);
            string strRet=GenericDataAccess.ExecuteScalar(comm);
            if ( Convert.ToInt16(strRet)> 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }

        public static bool IsUserIdExist(string userId)
        {
            bool ret = false;
            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(USER_ID) From IM_USERS WHERE USER_ID=@USER_ID";
            CreateParameter.AddParam(comm, "@USER_ID", userId, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }


        public static bool IsNewPasswordSameAsOld(string userID, string newPassword)
        {
            bool ret = false;

            DbCommand comm = GenericDataAccess.CreateCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = @"Select count(*) From IM_USERS WHERE USER_ID=@USER_ID";
            CreateParameter.AddParam(comm, "@USER_ID", userID, DbType.String);
            CreateParameter.AddParam(comm, "@PASSWORD", newPassword, DbType.String);
            string strRet = GenericDataAccess.ExecuteScalar(comm);
            if (Convert.ToInt16(strRet) > 0)
                ret = true;
            if (comm.Connection.State != ConnectionState.Closed)
                comm.Connection.Close();
            return ret;
        }
    }
}
