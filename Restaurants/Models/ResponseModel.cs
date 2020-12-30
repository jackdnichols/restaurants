using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class ResponseModel
    {
        String _code;
        String _message;

        public string Code { get => _code; set => _code = value; }
        public string Message { get => _message; set => _message = value; }
    }
}