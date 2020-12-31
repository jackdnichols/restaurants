using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class RestaurantMenuCriteriaModel
    {
        Int32 _restaurantId;
        Int32 _restaurantMenuCategoryId;

        public int RestaurantId { get => _restaurantId; set => _restaurantId = value; }
        public int RestaurantMenuCategoryId { get => _restaurantMenuCategoryId; set => _restaurantMenuCategoryId = value; }
    }
}