using System;
using System.Data;
using System.Data.SqlClient;

namespace Restaurants.Models
{
    public class ExceptionModel
    {
        int _exceptionLogId;
        String _exceptionMessage;
        String _nameSpace;
        String _method;
        DateTime _createdDate;

        public int ExceptionLogId { get => _exceptionLogId; set => _exceptionLogId = value; }
        public string ExceptionMessage { get => _exceptionMessage; set => _exceptionMessage = value; }
        public string NameSpace { get => _nameSpace; set => _nameSpace = value; }
        public string Method { get => _method; set => _method = value; }
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }

        public static void SaveException(String exceptionMessage, String nameSpace, String method)
        {
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandText = "spSaveExceptionLog";
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Connection = myConnection;

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@Exception_Message";
            parameter.SqlDbType = SqlDbType.VarChar;
            parameter.Size = 0;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = exceptionMessage;
            sqlCmd.Parameters.Add(parameter);

            parameter = new SqlParameter();
            parameter.ParameterName = "@Name_Space";
            parameter.SqlDbType = SqlDbType.VarChar;
            parameter.Size = 150;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = nameSpace;
            sqlCmd.Parameters.Add(parameter);

            parameter = new SqlParameter();
            parameter.ParameterName = "@Method";
            parameter.SqlDbType = SqlDbType.VarChar;
            parameter.Size = 150;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = method;
            sqlCmd.Parameters.Add(parameter);

            myConnection.Open();
            sqlCmd.ExecuteNonQuery();
            myConnection.Close();
        }
    }
}