using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class ForgotPasswordModel
    {
        String _loginId;

        public string LoginId { get => _loginId; set => _loginId = value; }
    }
}