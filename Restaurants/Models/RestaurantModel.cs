using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class RestaurantModel
    {
        private Int32 restaurantId;
        private String restaurantName;
        private String address;
        private String city;
        private String state;
        private String zipCode;
        private String emailAddress;
        private String restaurantImageURL;
        private Decimal salesTaxPercentage;
        private DateTime? endDate;

        public Int32 RestaurantId { get => restaurantId; set => restaurantId = value; }
        public string RestaurantName { get => restaurantName; set => restaurantName = value; }
        public string Address { get => address; set => address = value; }
        public string City { get => city; set => city = value; }
        public string State { get => state; set => state = value; }
        public string ZipCode { get => zipCode; set => zipCode = value; }
        public string EmailAddress { get => emailAddress; set => emailAddress = value; }
        public string RestaurantImageURL { get => restaurantImageURL; set => restaurantImageURL = value; }
        public DateTime? EndDate { get => endDate; set => endDate = value; }
        public decimal SalesTaxPercentage { get => salesTaxPercentage; set => salesTaxPercentage = value; }
    }
}