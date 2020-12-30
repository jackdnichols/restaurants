using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class TemporaryPasswordModel
    {
        Int32 _temporaryPasswordId;
        String _loginId;
        String _temporaryPassword;
        DateTime? _createdDate;

        public int TemporaryPasswordId { get => _temporaryPasswordId; set => _temporaryPasswordId = value; }
        public string LoginId { get => _loginId; set => _loginId = value; }
        public string TemporaryPassword { get => _temporaryPassword; set => _temporaryPassword = value; }
        public DateTime? CreatedDate { get => _createdDate; set => _createdDate = value; }
    }
}