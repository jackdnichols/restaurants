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
        public System.Web.Http.Results.OkNegotiatedContentResult<List<RestaurantModel>> GetRestaurants()
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

            List<RestaurantModel> restaurantList = new List<RestaurantModel>();
            RestaurantModel restaurant = null;
            while (reader.Read())
            {                
                restaurant = new RestaurantModel();
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

        [HttpGet]
        [Route("api/getrestaurantsbyname/{restaurantName}")]
        public System.Web.Http.Results.OkNegotiatedContentResult<List<RestaurantModel>> GetRestaurantsByName(String restaurantName)
        {
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "spGetRestaurants_By_Name";
            sqlCmd.Connection = myConnection;

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@Restaurant_Name";
            parameter.SqlDbType = SqlDbType.VarChar;
            parameter.Size = 100;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = restaurantName;
            sqlCmd.Parameters.Add(parameter);

            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            List<RestaurantModel> restaurantList = new List<RestaurantModel>();
            RestaurantModel restaurant = null;
            while (reader.Read())
            {                
                restaurant = new RestaurantModel();
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

        [HttpPost]
        [Route("api/saverestaurant")]
        public System.Web.Http.Results.OkNegotiatedContentResult<ReturnCodeModel> SaveRestaurant([FromBody] RestaurantModel restaurant)
        {
            try
            {
                Int32 recordId = 0;

                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spSaveRestaurant";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;


                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.RestaurantId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Name";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 100;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value =  restaurant.RestaurantName;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Address";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.Address;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@City";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.City;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@State";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.State;
                sqlCmd.Parameters.Add(parameter);
                	
                parameter = new SqlParameter();
                parameter.ParameterName = "@Zip_Code";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 20;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.ZipCode;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Email_Address";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.EmailAddress;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Image_URL";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 1000;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.RestaurantImageURL;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Sales_Tax_Percent";
                parameter.SqlDbType = SqlDbType.Decimal;
                parameter.Size = 10;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.SalesTaxPercentage;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@End_Date";
                parameter.SqlDbType = SqlDbType.DateTime;
                parameter.Size = 8;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurant.EndDate;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Return_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Output;
                sqlCmd.Parameters.Add(parameter);


                myConnection.Open();
                sqlCmd.ExecuteNonQuery();
                recordId = (Int32)parameter.Value;
                myConnection.Close();

                ReturnCodeModel returnCodeModel = new ReturnCodeModel();
                returnCodeModel.RecordId = recordId;
                returnCodeModel.ReturnCode = "success";
                returnCodeModel.Message = "The record has been saved successfully";

                return Ok(content: returnCodeModel);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        [HttpPost]
        [Route("api/getrestaurantmenu")]
        public System.Web.Http.Results.OkNegotiatedContentResult<List<RestaurantMenuModel>> GetRestaurantMenu([FromBody] RestaurantMenuCriteriaModel restaurantMenuCriteria)
        {
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "spGetRestaurantMenu";
            sqlCmd.Connection = myConnection;

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@Restaurant_Id";
            parameter.SqlDbType = SqlDbType.Int;
            parameter.Size = 4;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = restaurantMenuCriteria.RestaurantId;
            sqlCmd.Parameters.Add(parameter);

            parameter = new SqlParameter();
            parameter.ParameterName = "@Restaurant_Menu_Category_Id";
            parameter.SqlDbType = SqlDbType.Int;
            parameter.Size = 4;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = restaurantMenuCriteria.RestaurantMenuCategoryId;
            sqlCmd.Parameters.Add(parameter);

            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            int restaurantMenuIdOrdinal = reader.GetOrdinal("Restaurant_Menu_Id");
            int restaurantIdOrdinal = reader.GetOrdinal("Restaurant_Id");
            int restaurantMenuCategoryIdOrdinal = reader.GetOrdinal("Restaurant_Menu_Category_Id");
            int menuCategoryOrdinal = reader.GetOrdinal("Menu_Category");
            int menuItemOrdinal = reader.GetOrdinal("Menu_Item");
            int menuCostOrdinal = reader.GetOrdinal("Menu_Cost");
            int sortIndexOrdinal = reader.GetOrdinal("Sort_Index");
            int endDateOrdinal = reader.GetOrdinal("End_Date");

            List<RestaurantMenuModel> restaurantMenuList = new List<RestaurantMenuModel>();
            RestaurantMenuModel restaurantMenu = null;
            while (reader.Read())
            {
                restaurantMenu = new RestaurantMenuModel();
                restaurantMenu.RestaurantMenuId = reader.GetInt32(restaurantMenuIdOrdinal);
                restaurantMenu.RestaurantId = reader.GetInt32(restaurantIdOrdinal);
                restaurantMenu.RestaurantMenuCategoryId = reader.GetInt32(restaurantMenuCategoryIdOrdinal);
                restaurantMenu.MenuCategory = reader.GetString(menuCategoryOrdinal);
                restaurantMenu.MenuItem = reader.GetString(menuItemOrdinal);
                restaurantMenu.MenuCost = reader.GetDecimal(menuCostOrdinal);
                restaurantMenu.SortIndex = reader.GetInt32(sortIndexOrdinal);
                restaurantMenu.EndDate = reader.IsDBNull(endDateOrdinal) ? (DateTime?)null : (DateTime?)reader.GetDateTime(endDateOrdinal);
                restaurantMenuList.Add(restaurantMenu);
            }
            myConnection.Close();
            return Ok(content: restaurantMenuList);
        }

        [HttpPost]
        [Route("api/saverestaurantmenu")]
        public System.Web.Http.Results.OkNegotiatedContentResult<ReturnCodeModel> SaveRestaurantMenu([FromBody] RestaurantMenuModel restaurantMenu)
        {
            try
            {
                Int32 recordId = 0;

                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spSaveRestaurantMenu";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;


                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Menu_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.RestaurantMenuId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.RestaurantId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Menu_Category_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.RestaurantMenuCategoryId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Menu_Category";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.MenuCategory;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Menu_Item";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.MenuItem;
                sqlCmd.Parameters.Add(parameter);
                	
                parameter = new SqlParameter();
                parameter.ParameterName = "@Menu_Cost";
                parameter.SqlDbType = SqlDbType.Decimal;
                parameter.Size = 8;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.MenuCost;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Sort_Index";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.SortIndex;
                sqlCmd.Parameters.Add(parameter);
                
                parameter = new SqlParameter();
                parameter.ParameterName = "@End_Date";
                parameter.SqlDbType = SqlDbType.DateTime;
                parameter.Size = 8;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenu.EndDate;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Return_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Output;
                sqlCmd.Parameters.Add(parameter);


                myConnection.Open();
                sqlCmd.ExecuteNonQuery();
                recordId = (Int32)parameter.Value;
                myConnection.Close();

                ReturnCodeModel returnCodeModel = new ReturnCodeModel();
                returnCodeModel.RecordId = recordId;
                returnCodeModel.ReturnCode = "success";
                returnCodeModel.Message = "The record has been saved successfully";

                return Ok(content: returnCodeModel);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        [HttpGet]
        [Route("api/getrestaurantmenucategories/{restaurantId}")]
        public System.Web.Http.Results.OkNegotiatedContentResult<List<RestaurantMenuCategoryModel>> GetRestaurantsMenuCategories(Int32 restaurantId)
        {
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "spGetRestaurantMenuCategory";
            sqlCmd.Connection = myConnection;

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@Restaurant_Id";
            parameter.SqlDbType = SqlDbType.Int;
            parameter.Size = 3;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = restaurantId;
            sqlCmd.Parameters.Add(parameter);

            myConnection.Open();
            reader = sqlCmd.ExecuteReader();

            int restaurantMenuCategoryIdOrdinal = reader.GetOrdinal("Restaurant_Menu_Category_Id");
            int restaurantIdOrdinal = reader.GetOrdinal("Restaurant_Id");
            int restaurantMenuCategoryOrdinal = reader.GetOrdinal("Restaurant_Menu_Category");
            int sortIndexOrdinal = reader.GetOrdinal("Sort_Index");
            int endDateOrdinal = reader.GetOrdinal("End_Date");

            List <RestaurantMenuCategoryModel> restaurantList = new List<RestaurantMenuCategoryModel>();
            RestaurantMenuCategoryModel restaurant = null;
            while (reader.Read())
            {
                restaurant = new RestaurantMenuCategoryModel();
                restaurant.RestaurantMenuCategoryId = reader.GetInt32(restaurantMenuCategoryIdOrdinal);
                restaurant.RestaurantId = reader.GetInt32(restaurantIdOrdinal);
                restaurant.RestaurantMenuCategory = reader.GetString(restaurantMenuCategoryOrdinal);
                restaurant.SortIndex = reader.GetInt32(sortIndexOrdinal);
                restaurant.EndDate = reader.IsDBNull(endDateOrdinal) ? (DateTime?)null : (DateTime?)reader.GetDateTime(endDateOrdinal);
                restaurantList.Add(restaurant);
            }
            myConnection.Close();
            return Ok(content: restaurantList);
        }

        [HttpPost]
        [Route("api/saverestaurantmenucategory")]
        public System.Web.Http.Results.OkNegotiatedContentResult<ReturnCodeModel> SaveRestaurantMenuCategory([FromBody] RestaurantMenuCategoryModel restaurantMenuCategory)
        {
            try
            {
                Int32 recordId = 0;

                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spSaveRestaurantMenuCategory";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Menu_Category_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenuCategory.RestaurantMenuCategoryId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenuCategory.RestaurantId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Menu_Category";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 150;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenuCategory.RestaurantMenuCategory;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Sort_Index";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenuCategory.SortIndex;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@End_Date";
                parameter.SqlDbType = SqlDbType.DateTime;
                parameter.Size = 8;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = restaurantMenuCategory.EndDate;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Return_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Size = 4;
                parameter.Direction = ParameterDirection.Output;
                sqlCmd.Parameters.Add(parameter);


                myConnection.Open();
                sqlCmd.ExecuteNonQuery();
                recordId = (Int32)parameter.Value;
                myConnection.Close();

                ReturnCodeModel returnCodeModel = new ReturnCodeModel();
                returnCodeModel.RecordId = recordId;
                returnCodeModel.ReturnCode = "success";
                returnCodeModel.Message = "The record has been saved successfully";

                return Ok(content: returnCodeModel);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
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

        [HttpPost]
        [Route("api/getrestaurantmenuitems")]
        public System.Web.Http.Results.OkNegotiatedContentResult<List<RestaurantMenuModel>> GetRestaurantMenuItems([FromBody] RestaurantMenuItemParameterModel restaurantMenuItemParameterModel)
        {
            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();

                sqlCmd.CommandText = "spGetRestaurantMenuItems";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Restaurant_Id";
                parameter.SqlDbType = SqlDbType.Int;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 4;
                parameter.Value = restaurantMenuItemParameterModel.RestaurantId;
                sqlCmd.Parameters.Add(parameter);

                myConnection.Open();
                reader = sqlCmd.ExecuteReader();

                int restaurantMenuIdOrdinal = reader.GetOrdinal("Restaurant_Menu_Id");
                int restaurantIdOrdinal = reader.GetOrdinal("Restaurant_Id");
                int restaurantMenuCategoryIdOrdinal = reader.GetOrdinal("Restaurant_Menu_Category_Id");
                int menuCategoryOrdinal = reader.GetOrdinal("Menu_Category");
                int menuItemOrdinal = reader.GetOrdinal("Menu_Item");
                int menuCostOrdinal = reader.GetOrdinal("Menu_Cost");
                int sortIndexOrdinal = reader.GetOrdinal("Sort_Index");
                int endDateOrdinal = reader.GetOrdinal("End_Date");

                List<RestaurantMenuModel> restaurantMenuModelList = new List<RestaurantMenuModel>();
                RestaurantMenuModel restaurantMenuModel = null;

                while (reader.Read())
                {
                    restaurantMenuModel = new RestaurantMenuModel();
                    restaurantMenuModel.RestaurantMenuId = reader.GetInt32(restaurantMenuIdOrdinal);
                    restaurantMenuModel.RestaurantId = reader.GetInt32(restaurantIdOrdinal);
                    restaurantMenuModel.RestaurantMenuCategoryId = reader.GetInt32(restaurantMenuCategoryIdOrdinal);
                    restaurantMenuModel.MenuCategory = reader.GetString(menuCategoryOrdinal);
                    restaurantMenuModel.MenuItem = reader.GetString(menuItemOrdinal);
                    restaurantMenuModel.MenuCost = reader.GetDecimal(menuCostOrdinal);
                    restaurantMenuModel.SortIndex = reader.GetInt32(sortIndexOrdinal);                    
                    restaurantMenuModel.EndDate = (reader.IsDBNull(endDateOrdinal) ? (DateTime?)null : (DateTime?)reader.GetDateTime(endDateOrdinal));
                    restaurantMenuModelList.Add(restaurantMenuModel);
                }

                myConnection.Close();

                return Ok(content: restaurantMenuModelList);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }
        
    }
}
