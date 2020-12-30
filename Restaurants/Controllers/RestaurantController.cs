using Restaurants.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web.Http;

namespace Restaurants.Controllers
{
    [Authorize]
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/[controller]")]
    public class RestaurantController : ApiController
    {
        [HttpGet]
        [Route("api/getrestaurants")]
        public System.Web.Http.Results.OkNegotiatedContentResult<List<Restaurant>> GetRestaurants()
        {
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from Restaurant";
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            List<Restaurant> restaurantList = new List<Restaurant>();
            Restaurant restaurant = null;
            while (reader.Read())
            {
                /*
                 * [Restaurant_Id]
                  ,[Restaurant_Name]
                  ,[Address]
                  ,[City]
                  ,[State]
                  ,[Zip_Code]
                  ,[Email_Address]
                  ,[Restaurant_Image_URL]
                  ,[End_Date]
                 */
                restaurant = new Restaurant();
                restaurant.RestaurantId = Convert.ToInt32(reader.GetValue(0));
                restaurant.RestaurantName = Convert.ToString(reader.GetValue(1));
                restaurant.Address = Convert.ToString(reader.GetValue(2));
                restaurant.City = Convert.ToString(reader.GetValue(3));
                restaurant.State = Convert.ToString(reader.GetValue(4));
                restaurant.ZipCode = Convert.ToString(reader.GetValue(5));
                restaurant.EmailAddress = Convert.ToString(reader.GetValue(6));
                restaurant.RestaurantImageURL = Convert.ToString(reader.GetValue(7));
                restaurant.EndDate = reader.IsDBNull(8) ? (DateTime?)null : (DateTime?)reader.GetDateTime(8);                
                restaurantList.Add(restaurant);
            }
            myConnection.Close();
            return Ok(content: restaurantList);
        }

        public void SaveTemporaryPassword(String loginId, String temporaryPassword)
        {
            try
            {
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spSaveTemporaryPassword";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Login_Id";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 50;
                parameter.Value = loginId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Temporary_Password";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 50;
                parameter.Value = temporaryPassword;
                sqlCmd.Parameters.Add(parameter);

                myConnection.Open();
                sqlCmd.ExecuteNonQuery();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return;
            }
        }

        [HttpPost]
        [Route("api/getuserrole")]
        public System.Web.Http.Results.OkNegotiatedContentResult<SystemUserModel> GetUserRole([FromBody] LoginIdModel loginIdModel)
        {
            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();

                sqlCmd.CommandText = "spGetUserRole";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Login_Id";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 50;
                parameter.Value = loginIdModel.LoginId;
                sqlCmd.Parameters.Add(parameter);

                myConnection.Open();
                reader = sqlCmd.ExecuteReader();

                int systemUserIdOrdinal = reader.GetOrdinal("System_User_Id");
                int firstNameOrdinal = reader.GetOrdinal("First_Name");
                int lastNameOrdinal = reader.GetOrdinal("Last_Name");
                int emailAddressOrdinal = reader.GetOrdinal("Email_Address");
                int loginIdOrdinal = reader.GetOrdinal("Login_Id");
                int roleOrdinal = reader.GetOrdinal("Role");
                int endDateOrdinal = reader.GetOrdinal("End_Date");

                SystemUserModel systemUserModel = new SystemUserModel();

                if (reader.Read())
                {
                    systemUserModel.SystemUserId = reader.GetInt32(systemUserIdOrdinal);
                    systemUserModel.FirstName = reader.GetString(firstNameOrdinal);
                    systemUserModel.LastName = reader.GetString(lastNameOrdinal);
                    systemUserModel.EmailAddress = reader.GetString(emailAddressOrdinal);
                    systemUserModel.LoginId = reader.GetString(loginIdOrdinal);
                    systemUserModel.Role = reader.GetString(roleOrdinal);
                    systemUserModel.EndDate = (reader.IsDBNull(endDateOrdinal) ? (DateTime?)null : (DateTime?)reader.GetDateTime(endDateOrdinal));
                }

                myConnection.Close();

                return Ok(content: systemUserModel);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        [HttpPost]
        [Route("api/sendemail")]
        public void SendEmail([FromBody] EmailModel emailModel)
        {
            try
            {
                String smtpServerName = "";
                int smtpPort = 0;
                Boolean smtpSSL = false;
                Boolean smtpIgnoreBadCertificate = false;
                Boolean smtpAllowInsecure = false;
                String smtpUserName = "";
                String smtpPassword = "";

                List<SettingsModel> settingsList = GetSettingsAll().Content;
                if (settingsList != null && settingsList.Count > 0)
                {
                    foreach (SettingsModel setting in settingsList)
                    {
                        switch (setting.Settingkey)
                        {
                            case "SMTP_Server_Name":
                                smtpServerName = setting.SettingValue;
                                break;
                            case "SMTP_Port":
                                _ = int.TryParse(setting.SettingValue, out smtpPort);
                                break;
                            case "SMTP_SSL":
                                smtpSSL = (setting.SettingValue == "true") ? true : false;
                                break;
                            case "SMTP_Ignore_Bad_Certificate":
                                smtpIgnoreBadCertificate = (setting.SettingValue == "true") ? true : false;
                                break;
                            case "SMTP_Allow_Insecure":
                                smtpAllowInsecure = (setting.SettingValue == "true") ? true : false;
                                break;
                            case "SMTP_UserName":
                                smtpUserName = setting.SettingValue;
                                break;
                            case "SMTP_Password":
                                smtpPassword = setting.SettingValue;
                                break;
                        }
                    }
                }

                SmtpClient smtpClient = new SmtpClient(smtpServerName, smtpPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
                // smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = smtpSSL;

                MailMessage mail = new MailMessage();

                mail.IsBodyHtml = true;

                //Setting From , To and CC
                mail.From = new MailAddress(emailModel.EmailFrom, emailModel.EmailFrom);
                mail.To.Add(new MailAddress(emailModel.EmailTo));
                //mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));
                mail.Subject = emailModel.EmailSubject;
                mail.Body = emailModel.EmailBody;

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return;
            }
        }

        [HttpGet]
        [Route("api/getsettingsall")]
        public System.Web.Http.Results.OkNegotiatedContentResult<List<SettingsModel>> GetSettingsAll()
        {
            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spGetSettingsAll";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                myConnection.Open();
                reader = sqlCmd.ExecuteReader();

                List<SettingsModel> settingList = new List<SettingsModel>();
                SettingsModel setting = null;
                while (reader.Read())
                {
                    setting = new SettingsModel();
                    setting.SettingId = reader.GetInt32(0);
                    setting.Settingkey = reader.GetString(1);
                    setting.SettingValue = reader.GetString(2);

                    settingList.Add(setting);
                }
                myConnection.Close();
                return Ok(content: settingList);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        // GET: api/Restaurant
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Restaurant/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Restaurant
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Restaurant/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Restaurant/5
        public void Delete(int id)
        {
        }
    }
}
