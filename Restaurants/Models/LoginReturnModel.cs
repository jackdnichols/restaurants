using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class LoginReturnModel
    {
        String _code;
        String _message;
        String _token;
        Int32 _systemUserId;
        String _role;

        public string Code { get => _code; set => _code = value; }
        public string Token { get => _token; set => _token = value; }
        public string Message { get => _message; set => _message = value; }
        public int SystemUserId { get => _systemUserId; set => _systemUserId = value; }
        public string Role { get => _role; set => _role = value; }
    }
}