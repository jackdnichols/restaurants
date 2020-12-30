using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class SystemUserModel
    {
        private Int32 _systemUserId;
        private String _firstName;
        private String _lastName;
        private String _emailAddress;
        private String _loginId;
        private String _role;
        private DateTime? _endDate;

        public int SystemUserId { get => _systemUserId; set => _systemUserId = value; }
        public string FirstName { get => _firstName; set => _firstName = value; }
        public string LastName { get => _lastName; set => _lastName = value; }
        public string EmailAddress { get => _emailAddress; set => _emailAddress = value; }
        public string LoginId { get => _loginId; set => _loginId = value; }
        public string Role { get => _role; set => _role = value; }
        public DateTime? EndDate { get => _endDate; set => _endDate = value; }
    }
}