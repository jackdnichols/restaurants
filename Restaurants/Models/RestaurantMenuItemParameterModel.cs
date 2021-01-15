using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class RestaurantMenuItemParameterModel
    {
        int _restaurantId;

        public int RestaurantId { get => _restaurantId; set => _restaurantId = value; }
    }
}