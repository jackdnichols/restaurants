using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Restaurants.Models;

namespace Restaurants.Controllers
{
    //[ApiController]
    //[Authorize]
    [Route("api/[controller]")]
    public class RestaurantController : ApiController
    {
        [HttpGet]
        [Route("api/getrestaurants")]
        public List<Restaurant> GetRestaurants()
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
            return restaurantList;
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
