using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class RestaurantMenuCategoryModel
    {
        Int32 _restaurantMenuCategoryId;
	    Int32 _restaurantId;
	    String _restaurantMenuCategory;
	    Int32 _sortIndex;
        DateTime? _endDate;

        public int RestaurantMenuCategoryId { get => _restaurantMenuCategoryId; set => _restaurantMenuCategoryId = value; }
        public int RestaurantId { get => _restaurantId; set => _restaurantId = value; }
        public string RestaurantMenuCategory { get => _restaurantMenuCategory; set => _restaurantMenuCategory = value; }
        public int SortIndex { get => _sortIndex; set => _sortIndex = value; }
        public DateTime? EndDate { get => _endDate; set => _endDate = value; }
    }
}