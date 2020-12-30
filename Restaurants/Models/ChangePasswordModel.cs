using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class ChangePasswordModel
    {
        private String _loginId;
        private String _oldPassword;
        private String _newPassword;

        public string LoginId { get => _loginId; set => _loginId = value; }
        public string OldPassword { get => _oldPassword; set => _oldPassword = value; }
        public string NewPassword { get => _newPassword; set => _newPassword = value; }
    }
}