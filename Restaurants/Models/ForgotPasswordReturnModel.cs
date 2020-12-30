using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class ForgotPasswordReturnModel
    {
        String _code;
        String _message;
        String _temporaryPassword;

        public string Code { get => _code; set => _code = value; }
        public string TemporaryPassword { get => _temporaryPassword; set => _temporaryPassword = value; }
        public string Message { get => _message; set => _message = value; }
    }
}