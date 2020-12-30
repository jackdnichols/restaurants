using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class SettingsModel
    {
        Int32 _settingId;
        String _settingkey;
        String _settingValue;

        public int SettingId { get => _settingId; set => _settingId = value; }
        public string Settingkey { get => _settingkey; set => _settingkey = value; }
        public string SettingValue { get => _settingValue; set => _settingValue = value; }
    }
}