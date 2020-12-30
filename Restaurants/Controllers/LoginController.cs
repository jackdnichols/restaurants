using Restaurants.Models;
using Restaurants.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace Restaurants.Controllers
{
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        public IHttpActionResult Authenticate(LoginRequest login)
        {
            LoginReturnModel loginReturnModel = new LoginReturnModel();

            try
            {
                Int32 systemUserId = 0;
                String role = "";

                var loginResponse = new LoginResponse { };
                LoginRequest loginrequest = new LoginRequest { };
                loginrequest.Username = login.Username.ToLower();
                loginrequest.Password = login.Password;

                // If this is a temporary password, return the temporary login info.
                TemporaryPasswordModel temporaryPasswordModel = new TemporaryPasswordModel();
                temporaryPasswordModel = GetTemporaryPasswordByLoginId(login);
                if (temporaryPasswordModel != null && temporaryPasswordModel.LoginId != null && temporaryPasswordModel.TemporaryPassword != null)
                {
                    return Ok<TemporaryPasswordModel>(temporaryPasswordModel);
                }

                //IHttpActionResult response;
                HttpResponseMessage responseMsg = new HttpResponseMessage();
                bool isUsernamePasswordValid = false;
                               
                isUsernamePasswordValid = IsValidLogin(loginrequest.Username, loginrequest.Password);
                systemUserId = GetSystemUserIdFromLogin(loginrequest.Username);
                role = GetRole(loginrequest.Username);

                // if credentials are valid
                if (isUsernamePasswordValid)
                {
                    //return the token
                    string token = createToken(loginrequest.Username);
                    loginReturnModel.Code = "success";
                    loginReturnModel.Message = "The login was successful";
                    loginReturnModel.Token = token;
                    loginReturnModel.SystemUserId = systemUserId;
                    loginReturnModel.Role = role;
                    return Ok<LoginReturnModel>(loginReturnModel);
                }
                else
                {
                    loginReturnModel.Code = "error";
                    loginReturnModel.Message = "The login was unsuccessful";
                    loginReturnModel.Token = "";
                    loginReturnModel.SystemUserId = -1;
                    loginReturnModel.Role = "";
                    return Ok<LoginReturnModel>(loginReturnModel);
                }
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                loginReturnModel.Code = "error";
                loginReturnModel.Message = "The login was unsuccessful";
                loginReturnModel.Token = "";
                return Ok<LoginReturnModel>(loginReturnModel);
            }
        }

        private Int32 GetSystemUserIdFromLogin(String loginId)
        {
            SystemUserModel systemUserModel = new SystemUserModel();
            

            try
            {
                SqlDataReader reader = null;
                Int32 systemUserId = 0;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spGetSystemUserIdFromLogin";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Login_Id";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 50;
                parameter.Value = loginId;
                sqlCmd.Parameters.Add(parameter);

                myConnection.Open();
                reader = sqlCmd.ExecuteReader();

                int systemUserIdOrdinal = reader.GetOrdinal("System_User_Id");

                if (reader.Read())
                {
                    systemUserId = reader.GetInt32(systemUserIdOrdinal);
                }

                myConnection.Close();

                return systemUserId;
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return -1;
            }
        }

        private String GetRole(String username)
        {
            try
            {
                RestaurantController restaurantController = new RestaurantController();
                LoginIdModel loginIdModel = new Restaurants.Models.LoginIdModel();

                loginIdModel.LoginId = username;
                return restaurantController.GetUserRole(loginIdModel).Content.Role;
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return "";
            }
        }

        private string createToken(string username)
        {
            try
            {
                //http://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
                
                DateTime issuedAt = DateTime.UtcNow; //Set issued at date
                DateTime expires = DateTime.UtcNow.AddDays(7); //set the time when it expires
                String tokenIssuer = "";
                String tokenAudience = "";

                var tokenHandler = new JwtSecurityTokenHandler();

                //create a identity and add claims to the user which we want to log in
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                });

                string sec = "";

                if (ConfigurationManager.AppSettings["TokenSecret"] != null)
                {
                    sec = ConfigurationManager.AppSettings["TokenSecret"].ToString();
                }

                var now = DateTime.UtcNow;
                var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
                var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

                if (ConfigurationManager.AppSettings["TokenIssuer"] != null &&
                    ConfigurationManager.AppSettings["TokenAudience"] != null)
                {
                    tokenIssuer = ConfigurationManager.AppSettings["TokenIssuer"].ToString();
                    tokenAudience = ConfigurationManager.AppSettings["TokenAudience"].ToString();
                }

                //create the jwt
                var token =
                    (JwtSecurityToken)
                        tokenHandler.CreateJwtSecurityToken(
                            issuer: tokenIssuer,
                            audience: tokenAudience,
                            subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);

                SystemUser systemUser = GetSystemUser(username);
                if (systemUser != null)
                {
                    token.Payload["FirstName"] = systemUser.FirstName;
                    token.Payload["LastName"] = systemUser.LastName;
                    token.Payload["EmailAddress"] = systemUser.EmailAddress;
                    token.Payload["EndDate"] = systemUser.EndDate;
                }

                var tokenString = tokenHandler.WriteToken(token);

                return tokenString;
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        [HttpPost]
        [Route("api/forgotpassword")]
        public System.Web.Http.Results.OkNegotiatedContentResult<ForgotPasswordReturnModel> ForgotPassword([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            ForgotPasswordReturnModel forgotPasswordReturnModel = new ForgotPasswordReturnModel();

            try
            {
                // Generate a temporary password
                String temporaryPassword = GeneratePassword(8);

                // Save the temporary password in the db
                TemporaryPasswordModel temporaryPasswordModel = new TemporaryPasswordModel();
                temporaryPasswordModel.LoginId = forgotPasswordModel.LoginId;
                temporaryPasswordModel.TemporaryPassword = temporaryPassword;
                temporaryPasswordModel.CreatedDate = DateTime.Now.AddDays(1);

                RestaurantController restaurantController = new RestaurantController();
                restaurantController.SaveTemporaryPassword(forgotPasswordModel.LoginId, temporaryPassword);                             

                // Get the users first and last name for the email
                String emailTo = "";
                String firstName = "";
                String lastName = "";

                SystemUser systemUser = GetSystemUser(forgotPasswordModel.LoginId);
                if (systemUser != null)
                {
                    emailTo = systemUser.EmailAddress;
                    firstName = systemUser.FirstName;
                    lastName = systemUser.LastName;


                    // Send an email to the user
                    EmailModel emailModel = new EmailModel();
                    emailModel.EmailFrom = "support@nicholssoftware.com";
                    emailModel.EmailTo = emailTo;
                    emailModel.EmailSubject = "Restaurants - Temporary password for " + firstName + " " + lastName;
                    emailModel.EmailBody = "<html><body><table> " +
                        "<tr><td>Name:</td><td>" + firstName + " " + lastName + "</td><td>&nbsp;</td></tr>" +
                        "<tr><td>Temporary Password:</td><td>" + temporaryPassword + "</td><td>&nbsp;</td></tr>" +
                        "<tr><td colspan='3'>&nbsp;</td></tr>" +
                        "<tr><td colspan='3'>Your request for a tempory password has been processed.<br />This password will allow you to reset your lost password and is valid for 24 hours.</td></tr>" +
                        "<tr><td colspan='3'>&nbsp;</td></tr>" +
                        "<tr><td colspan='3'>Please log in to <a href='https://restaurants.nicholssoftware.com/passwordreset'>https://restaurants.nicholssoftware.com/passwordreset</a> using your temporary password and update your login information.</td></tr>" +
                        "</table></body></html>";

                    restaurantController.SendEmail(emailModel);
                }

                forgotPasswordReturnModel.Code = "success";
                forgotPasswordReturnModel.Message = "The forgot password generation was successful";
                forgotPasswordReturnModel.TemporaryPassword = temporaryPassword;

                return Ok(content: forgotPasswordReturnModel);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                forgotPasswordReturnModel.Code = "error";
                forgotPasswordReturnModel.Message = "The forgot password generation was unsuccessful";
                forgotPasswordReturnModel.TemporaryPassword = "";
                return Ok(content: forgotPasswordReturnModel);
            }
        }

        public static string GeneratePassword(int numberOfCharacters)
        {
            try
            {
                char[] chars = new char[62];
                chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                byte[] data = new byte[1];

                /*__________________________________________________________________________________________
                 * RNGCryptoServiceProvider
                 * Implements a cryptographic Random Number Generator (RNG) using the implementation 
                 * provided by the cryptographic service provider (CSP). 
                 __________________________________________________________________________________________*/
                RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                crypto.GetNonZeroBytes(data);
                data = new byte[numberOfCharacters];
                crypto.GetNonZeroBytes(data);
                StringBuilder result = new StringBuilder(numberOfCharacters);

                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length)]);
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        public TemporaryPasswordModel GetTemporaryPasswordByLoginId(LoginRequest login)
        {
            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spGetTemporaryPasswordByLogin";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Login_Id";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = login.Username;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Temporary_Password";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = login.Password;
                sqlCmd.Parameters.Add(parameter);

                myConnection.Open();
                reader = sqlCmd.ExecuteReader();

                TemporaryPasswordModel temporaryPasswordModel = new TemporaryPasswordModel();

                int temporaryPasswordIdOrdinal = reader.GetOrdinal("Temporary_Password_Id");
                int loginIdOrdinal = reader.GetOrdinal("Login_Id");
                int temporaryPasswordOrdinal = reader.GetOrdinal("Temporary_Password");
                int createdDateOrdinal = reader.GetOrdinal("Created_Date");

                if (reader.Read())
                {
                    temporaryPasswordModel.TemporaryPasswordId = reader.GetInt32(temporaryPasswordIdOrdinal);
                    temporaryPasswordModel.LoginId = reader.GetString(loginIdOrdinal);
                    temporaryPasswordModel.TemporaryPassword = reader.GetString(temporaryPasswordOrdinal);
                    temporaryPasswordModel.CreatedDate = reader.GetDateTime(createdDateOrdinal);
                }
                myConnection.Close();
                return temporaryPasswordModel;
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        [HttpPost]
        [Route("api/changepassword")]
        public System.Web.Http.Results.OkNegotiatedContentResult<ResponseModel> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                SaltedHash SH = new SaltedHash();
                string strHash = "";
                string strSalt = "";
                byte[] hash = null;
                byte[] salt = null;
                ResponseModel response = new ResponseModel();

                // See if this is a temporary password
                if (IsTemporaryPassword(changePasswordModel) == true)
                {
                    // Dont check for a valid login
                }
                else if (IsValidLogin(changePasswordModel.LoginId, changePasswordModel.OldPassword) == false)
                {
                    response.Code = "error";
                    response.Message = "Invalid Login or Password";
                    return Ok(content: response);
                }

                SH.GetHashAndSaltString(changePasswordModel.NewPassword, out strHash, out strSalt);
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                hash = encoding.GetBytes(strHash);
                salt = encoding.GetBytes(strSalt);
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spChangePassword";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Login_Id";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 50;
                parameter.Value = changePasswordModel.LoginId;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Hash";
                parameter.SqlDbType = SqlDbType.VarBinary;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 250;
                parameter.Value = hash;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Salt";
                parameter.SqlDbType = SqlDbType.VarBinary;
                parameter.Direction = ParameterDirection.Input;
                parameter.Size = 250;
                parameter.Value = salt;
                sqlCmd.Parameters.Add(parameter);

                parameter = new SqlParameter();
                parameter.ParameterName = "@Results";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Size = 50;
                parameter.Direction = ParameterDirection.Output;
                sqlCmd.Parameters.Add(parameter);

                myConnection.Open();
                sqlCmd.ExecuteNonQuery();

                if (parameter.Value.ToString().Equals("0"))
                {
                    response.Code = "success";
                    response.Message = "Your Password Has Been Changed Successfully";
                }
                else
                {
                    response.Code = "error";
                    response.Message = "Invalid Login or Password";
                }
                myConnection.Close();

                return Ok(content: response);
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        private Boolean IsTemporaryPassword(ChangePasswordModel changePasswordModel)
        {
            try
            {
                Boolean isTemporaryPassword = false;
                LoginController loginController = new LoginController();

                LoginRequest login = new LoginRequest();
                login.Username = changePasswordModel.LoginId;
                login.Password = changePasswordModel.OldPassword;
                TemporaryPasswordModel temporaryPasswordModel = loginController.GetTemporaryPasswordByLoginId(login);
                if (temporaryPasswordModel != null && temporaryPasswordModel.LoginId != null)
                {
                    isTemporaryPassword = true;
                }
                return isTemporaryPassword;
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return false;
            }
        }

        public bool IsValidLogin(String userName, String password)
        {
            try
            {
                bool PasswordMatch = false;

                /*_________________________________________________________________________
                 * Compare the user's hash and salt with the password entered.
                 _________________________________________________________________________*/
                SystemUser systemUser = GetSystemUser(userName);

                if (systemUser != null && systemUser.Hash != null && systemUser.Salt != null)
                {
                    SaltedHash SH = new SaltedHash();
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    string dbHash = encoding.GetString(systemUser.Hash);
                    string dbSalt = encoding.GetString(systemUser.Salt);
                    PasswordMatch = SH.VerifyHashString(password, dbHash, dbSalt);
                }
                else
                {
                    PasswordMatch = false;
                }

                return PasswordMatch;
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return false;
            }
        }

        public SystemUser GetSystemUser(String userName)
        {
            try
            {
                SqlDataReader reader = null;
                SqlConnection myConnection = new SqlConnection();
                myConnection.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnection"];
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "spGetSystemUsersByLogin";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Connection = myConnection;

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@Login_Id";
                parameter.SqlDbType = SqlDbType.VarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = userName;
                sqlCmd.Parameters.Add(parameter);

                myConnection.Open();
                reader = sqlCmd.ExecuteReader();

                int systemUserIdOrdinal = reader.GetOrdinal("System_User_Id");
                int firstNameOrdinal = reader.GetOrdinal("First_Name");
                int lastNameOrdinal = reader.GetOrdinal("Last_Name");
                int emailAddressOrdinal = reader.GetOrdinal("Email_Address");
                int loginIdOrdinal = reader.GetOrdinal("Login_Id");
                int hashOrdinal = reader.GetOrdinal("Hash");
                int saltOrdinal = reader.GetOrdinal("Salt");
                int endDateOrdinal = reader.GetOrdinal("End_Date");

                List<SystemUser> systemUserList = new List<SystemUser>();
                SystemUser systemUser = null;
                while (reader.Read())
                {
                    systemUser = new SystemUser();
                    systemUser.SystemUserId = reader.GetInt32(systemUserIdOrdinal);
                    systemUser.FirstName = reader.GetString(firstNameOrdinal);
                    systemUser.LastName = reader.GetString(lastNameOrdinal);
                    systemUser.EmailAddress = reader.GetString(emailAddressOrdinal);
                    systemUser.LoginId = reader.GetString(loginIdOrdinal);
                    systemUser.Hash = reader.IsDBNull(hashOrdinal) ? null : ReadBytes(reader, hashOrdinal);
                    systemUser.Salt = reader.IsDBNull(saltOrdinal) ? null : ReadBytes(reader, saltOrdinal);
                    systemUser.EndDate = reader.IsDBNull(endDateOrdinal) ? (DateTime?)null : (DateTime?)reader.GetDateTime(endDateOrdinal);
                    systemUserList.Add(systemUser);
                }
                myConnection.Close();

                if (systemUserList != null && systemUserList.Count > 0)
                {
                    return systemUserList[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }

        public virtual byte[] ReadBytes(DbDataReader dataReader, int ordinalPosition)
        {
            try
            {
                Byte[] bytes = new Byte[(int)(dataReader.GetBytes(ordinalPosition, 0, null, 0, Int32.MaxValue))];
                dataReader.GetBytes(ordinalPosition, 0, bytes, 0, bytes.Length);

                return bytes;
            }
            catch (Exception ex)
            {
                ExceptionModel.SaveException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString(), System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return null;
            }
        }
    }
}