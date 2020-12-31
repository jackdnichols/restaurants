using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class RestaurantMenuModel
    {
		Int32 _restaurantMenuId;
		Int32 _restaurantId;
		Int32 _restaurantMenuCategoryId;
		String _menuCategory;
		String _menuItem;
		Decimal _menuCost;
		Int32 _sortIndex;
		DateTime? _endDate;

		public int RestaurantMenuId { get => _restaurantMenuId; set => _restaurantMenuId = value; }
		public int RestaurantId { get => _restaurantId; set => _restaurantId = value; }
		public int RestaurantMenuCategoryId { get => _restaurantMenuCategoryId; set => _restaurantMenuCategoryId = value; }
		public string MenuCategory { get => _menuCategory; set => _menuCategory = value; }
		public string MenuItem { get => _menuItem; set => _menuItem = value; }
		public decimal MenuCost { get => _menuCost; set => _menuCost = value; }
		public int SortIndex { get => _sortIndex; set => _sortIndex = value; }
		public DateTime? EndDate { get => _endDate; set => _endDate = value; }
	}
}