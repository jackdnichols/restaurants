using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Models
{
    public class EmailModel
    {
        String _emailFrom;
        String _emailTo;
        String _emailSubject;
        String _emailBody;

        public string EmailFrom { get => _emailFrom; set => _emailFrom = value; }
        public string EmailTo { get => _emailTo; set => _emailTo = value; }
        public string EmailSubject { get => _emailSubject; set => _emailSubject = value; }
        public string EmailBody { get => _emailBody; set => _emailBody = value; }
    }
}